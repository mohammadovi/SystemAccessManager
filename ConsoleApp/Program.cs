using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Threading;

class Program
{
    const string RegistryKeyName = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
    const string RegistryValueName = "DisableTaskMgr";
    const string AppName = "CustomRegister"; // نام برنامه شما برای استفاده در رجیستری

    static void Main()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Choose your option:");
        Console.WriteLine("1. Choose custom settings");
        Console.WriteLine("2. Use predefined settings");
        Console.WriteLine("Type 'EXIT' to quit.");

        // بررسی برای اولین اجرا و افزودن به startup
        bool isFirstRun = CheckFirstRun();

        if (isFirstRun)
        {
            CreateStartupRegistryKey();
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

            if (int.TryParse(input, out int choice) && (choice == 1 || choice == 2))
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
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input. Please enter 1, 2, or EXIT.");
                Console.ResetColor();
            }
        }
    }

    static int GetUserChoice()
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

    static void SetTaskManagerStatus(int status)
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
                Thread.Sleep(100); // Delay to show progress bar gradually
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

    static bool CheckFirstRun()
    {
        // چک کردن برای وجود کلید رجیستری
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
        {
            if (key != null)
            {
                string value = (string)key.GetValue(AppName);
                return value == null; // اگر کلید وجود نداشته باشد، true برمی‌گرداند
            }
        }
        return false; // در صورت خطا یا مشکل، false برمی‌گرداند
    }

    static void CreateStartupRegistryKey()
    {
        // ایجاد کلید رجیستری برای اجرا خودکار
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
        {
            var exe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            key.SetValue(AppName, exe);
        }
        Console.WriteLine("KeyAddTOregistery");
    }
}
