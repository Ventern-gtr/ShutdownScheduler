using System;
using System.Diagnostics;

internal enum LogType
{
    Info,
    Warning,
    Error,
    Success,
    Debug,
    None
}

public class Program
{
    public static void Main(string[] args)
    {
        string[] options =
        {
            "Schedule Shutdown",
            "Cancel Shutdown",
            "Exit"
        };

        int selectedIndex = 0;
        ConsoleKey key;

        while (true)
        {
            Console.Clear();

            PrintLine($"         Shutdown Scheduler", LogType.None);
            PrintLine("-----------------------------------", LogType.None);

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {options[i]}");
                }
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = options.Length - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                selectedIndex++;
                if (selectedIndex >= options.Length)
                    selectedIndex = 0;
            }
            else if (key == ConsoleKey.Enter)
            {
                Console.Clear();

                switch (selectedIndex)
                {
                    case 0:
                        {
                            int[] shutdownTimes = { 15, 25, 35, 45, 60, 120 };
                            int subIndex = 0;
                            ConsoleKey subKey;

                            while (true)
                            {
                                Console.Clear();

                                PrintLine("         Schedule Shutdown", LogType.None);
                                PrintLine("-----------------------------------", LogType.None);

                                for (int i = 0; i < shutdownTimes.Length; i++)
                                {
                                    if (i == subIndex)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine($"> {shutdownTimes[i]} minutes");
                                        Console.ResetColor();
                                    }
                                    else
                                    {
                                        Console.WriteLine($"  {shutdownTimes[i]} minutes");
                                    }
                                }

                                Console.WriteLine();
                                Console.WriteLine("Press ESC to go back");

                                subKey = Console.ReadKey(true).Key;

                                if (subKey == ConsoleKey.UpArrow)
                                {
                                    subIndex--;
                                    if (subIndex < 0)
                                        subIndex = shutdownTimes.Length - 1;
                                }
                                else if (subKey == ConsoleKey.DownArrow)
                                {
                                    subIndex++;
                                    if (subIndex >= shutdownTimes.Length)
                                        subIndex = 0;
                                }
                                else if (subKey == ConsoleKey.Enter)
                                {
                                    Console.Clear();
                                    ScheduleShutdown(shutdownTimes[subIndex]);
                                    Thread.Sleep(500);
                                    break;
                                }
                                else if (subKey == ConsoleKey.Escape)
                                {
                                    break;
                                }
                            }

                            break;
                        }


                    case 1:
                        PrintLine("Canceling shutdown...", LogType.Warning);
                        AbortShutdown();
                        break;

                    case 2:
                        PrintLine("Exiting application.", LogType.Success);
                        Environment.Exit(0);
                        return;
                }

                Thread.Sleep(1000);
            }
        }
    }

    static void ScheduleShutdown(int minutes)
    {
        try
        {
            int seconds = minutes * 60;

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = $"-s -t {seconds}",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(psi);
            PrintLine($"Shutdown scheduled in {minutes} minutes.", LogType.Success);
        }
        catch (Exception ex)
        {
            PrintLine($"Failed to schedule shutdown: {ex.Message}", LogType.Error);
        }
    }


    internal static void AbortShutdown()
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = "-a",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(psi);
            PrintLine("Shutdown aborted successfully.", LogType.Success);
        }
        catch (Exception ex)
        {
            PrintLine($"Failed to abort shutdown: {ex.Message}", LogType.Error);
        }
    }

    #region Basic Methdods
    private static readonly object consoleLock = new object();
    internal static void PrintLine(string message, LogType type, int typingDelay = 0, bool noline = false)
    {
        try
        {
            lock (consoleLock)
            {
                string prefix = "";
                ConsoleColor color;

                switch (type)
                {
                    case LogType.Info:
                        prefix = "[INFO] ";
                        color = ConsoleColor.Cyan;
                        break;
                    case LogType.Warning:
                        prefix = "[WARN] ";
                        color = ConsoleColor.Yellow;
                        break;
                    case LogType.Error:
                        prefix = "[ERROR] ";
                        color = ConsoleColor.Red;
                        break;
                    case LogType.Success:
                        prefix = "[SUCCESS] ";
                        color = ConsoleColor.Green;
                        break;
                    case LogType.Debug:
                        prefix = "[DEBUG] ";
                        color = ConsoleColor.Magenta;
                        break;
                    case LogType.None:
                        prefix = "";
                        color = ConsoleColor.White;
                        break;
                    default:
                        prefix = "";
                        color = ConsoleColor.Gray;
                        break;
                }

                Console.ForegroundColor = color;
                foreach (char c in prefix)
                {
                    Console.Write(c);
                    if (typingDelay > 0) Thread.Sleep(typingDelay);
                }

                Console.ResetColor();
                foreach (char c in message)
                {
                    Console.Write(c);
                    if (typingDelay > 0) Thread.Sleep(typingDelay);
                }

                if (!noline)
                    Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            lock (consoleLock)
            {
                Console.ResetColor();
                Console.WriteLine($"Logger error:\n{ex.Message}\n{ex.StackTrace}");
                Console.WriteLine(message);
            }
        }
    }
    #endregion
}