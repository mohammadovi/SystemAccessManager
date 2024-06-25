/*
 * Task Manager Settings Manager
 * This program allows users to manage Task Manager settings via console interface and optionally sets up a startup task using Task Scheduler.
 * The settings are stored in the registry under "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System".
 * Author: VDtem
 */

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Threading;
using Microsoft.Win32.TaskScheduler;
using System.Reflection;

namespace SystemAccessManager.ConsoleAppPlus
{
    public class Program
    {
        public const string RegistryKeyName = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
        public const string RegistryValueName = "DisableTaskMgr";
        public const string StartMenuRegistryKeyName = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";
        public const string StartMenuRegistryValueName = "NoStartMenuMorePrograms";
        public const string TaskbarRegistryValueName = "NoTrayItemsDisplay";
        public const string AppName = "MyApp"; // Your app name for use in the registry

        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        public static void Main()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine($"Current version: {version}");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Choose your option:");
            Console.WriteLine("1. Choose custom settings for Task Manager");
            Console.WriteLine("2. Use predefined settings for Task Manager");
            Console.WriteLine("3. Disable Start Menu");
            Console.WriteLine("4. Enable Start Menu");
            Console.WriteLine("5. Hide Start Menu");
            Console.WriteLine("6. Unhide Start Menu");
            Console.WriteLine("7. Disable Taskbar");
            Console.WriteLine("8. Enable Taskbar");
            Console.WriteLine("9. Hide Taskbar");
            Console.WriteLine("10. Unhide Taskbar");
            Console.WriteLine("Type 'EXIT' to quit.");

            // Check for first run and add to startup
            bool isFirstRun = CheckFirstRun();

            if (isFirstRun)
            {
                CreateStartupTask();
            }

            while (true)
            {
                string input = Console.ReadLine();

                if (string.Equals(input, "EXIT", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Exiting...");
                    Console.ResetColor();
                    break;
                }

                if (int.TryParse(input, out int choice) && (choice >= 1 && choice <= 10))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Processing...");
                    Console.ResetColor();

                    if (choice == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Enter 0 for active or 1 for not active:");
                        Console.ResetColor();

                        int userChoice = GetUserChoice();
                        SetTaskManagerStatus(userChoice);
                    }
                    else if (choice == 2)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Using predefined settings...");
                        Console.ResetColor();
                        SetTaskManagerStatus(1); // Disables Task Manager
                    }
                    else if (choice == 3)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Disabling Start Menu...");
                        Console.ResetColor();
                        SetStartMenuStatus(1);
                    }
                    else if (choice == 4)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Enabling Start Menu...");
                        Console.ResetColor();
                        SetStartMenuStatus(0);
                    }
                    else if (choice == 5)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Hiding Start Menu...");
                        Console.ResetColor();
                        HideStartMenu(true);
                    }
                    else if (choice == 6)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Unhiding Start Menu...");
                        Console.ResetColor();
                        HideStartMenu(false);
                    }
                    else if (choice == 7)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Disabling Taskbar...");
                        Console.ResetColor();
                        SetTaskbarStatus(1);
                    }
                    else if (choice == 8)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Enabling Taskbar...");
                        Console.ResetColor();
                        SetTaskbarStatus(0);
                    }
                    else if (choice == 9)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Hiding Taskbar...");
                        Console.ResetColor();
                        HideTaskbar(true);
                    }
                    else if (choice == 10)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Unhiding Taskbar...");
                        Console.ResetColor();
                        HideTaskbar(false);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 10, or EXIT.");
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// Gets the user's choice for Task Manager status.
        /// </summary>
        /// <returns>The user's choice (0 for active, 1 for not active).</returns>
        public static int GetUserChoice()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int userChoice) && (userChoice == 0 || userChoice == 1))
                    return userChoice;
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Please enter 0 or 1.");
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// Sets the Task Manager status based on user input.
        /// </summary>
        /// <param name="status">The status to set (0 for active, 1 for not active).</param>
        public static void SetTaskManagerStatus(int status)
        {
            try
            {
                ShowProgressBar(true);

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKeyName))
                {
                    key.SetValue(RegistryValueName, status, RegistryValueKind.DWord);
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Task Manager status set to: {(status == 0 ? "Active" : "Not Active")}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                ShowProgressBar(false);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Sets the Start Menu status.
        /// </summary>
        /// <param name="status">The status to set (0 for enable, 1 for disable).</param>
        public static void SetStartMenuStatus(int status)
        {
            try
            {
                ShowProgressBar(true);

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(StartMenuRegistryKeyName))
                {
                    key.SetValue(StartMenuRegistryValueName, status, RegistryValueKind.DWord);
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Start Menu status set to: {(status == 0 ? "Enabled" : "Disabled")}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                ShowProgressBar(false);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Hides or unhides the Start Menu.
        /// </summary>
        /// <param name="hide">True to hide, false to unhide.</param>
        public static void HideStartMenu(bool hide)
        {
            try
            {
                ShowProgressBar(true);

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(StartMenuRegistryKeyName))
                {
                    key.SetValue("NoStartMenu", hide ? 1 : 0, RegistryValueKind.DWord);
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Start Menu {(hide ? "hidden" : "unhidden")}.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                ShowProgressBar(false);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Sets the Taskbar status.
        /// </summary>
        /// <param name="status">The status to set (0 for enable, 1 for disable).</param>
        public static void SetTaskbarStatus(int status)
        {
            try
            {
                ShowProgressBar(true);

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(StartMenuRegistryKeyName))
                {
                    key.SetValue(TaskbarRegistryValueName, status, RegistryValueKind.DWord);
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Taskbar status set to: {(status == 0 ? "Enabled" : "Disabled")}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                ShowProgressBar(false);
            }
            finally
            {
                Console.ResetColor();


            }
        }

        /// <summary>
        /// Hides or unhides the Taskbar.
        /// </summary>
        /// <param name="hide">True to hide, false to unhide.</param>
        public static void HideTaskbar(bool hide)
        {
            try
            {
                ShowProgressBar(true);

                // Get the handle of the Taskbar window
                IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);
                if (taskbarHandle != IntPtr.Zero)
                {
                    ShowWindow(taskbarHandle, hide ? SW_HIDE : SW_SHOW);
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Taskbar {(hide ? "hidden" : "unhidden")}.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                ShowProgressBar(false);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        // PInvoke declarations
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        /// <summary>
        /// Shows a progress bar or spinner on the console.
        /// </summary>
        /// <param name="success">True if the operation succeeded; false otherwise.</param>
        static void ShowProgressBar(bool success)
        {
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("["); // Start progress bar

            if (success)
            {
                for (int i = 0; i <= 50; i++)
                {
                    Console.Write("=");
                    Thread.Sleep(10); // Delay to show progress bar gradually
                }
            }
            else
            {
                char[] spinner = { '|', '/', '-', '\\' };
                int counter = 0;

                while (!Console.KeyAvailable)
                {
                    Console.Write(spinner[counter]);
                    Thread.Sleep(100);
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    counter = (counter + 1) % spinner.Length;
                }
            }

            Console.WriteLine("] Done");
            Console.ResetColor();
            Console.CursorVisible = true;
        }

        /// <summary>
        /// Checks if the program is run for the first time by checking registry.
        /// </summary>
        /// <returns>True if it's the first run; false otherwise.</returns>
        public static bool CheckFirstRun()
        {
            // Check for the existence of the registry key
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (key != null)
                {
                    string value = (string)key.GetValue(AppName);
                    return value == null; // Returns true if the key does not exist
                }
            }
            return false; // Returns false in case of an error or issue
        }

        /// <summary>
        /// Creates a startup task in Task Scheduler to run the application with admin access on startup.
        /// </summary>
        static void CreateStartupTask()
        {
            // Create a task in Task Scheduler to run the program with admin access on startup
            using (TaskService ts = new TaskService())
            {
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "MyApp startup task";

                // Create a trigger to run at user logon
                td.Triggers.Add(new LogonTrigger());

                // Define an action to run your program
                td.Actions.Add(new ExecAction(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, null, null));

                // Run the task with highest privileges
                td.Principal.RunLevel = TaskRunLevel.Highest;

                // Register the task in Task Scheduler
                ts.RootFolder.RegisterTaskDefinition(@"MyAppStartupTask", td);

                Console.WriteLine("Task created successfully.");
            }
        }
    }
    // Author VDtem
}
