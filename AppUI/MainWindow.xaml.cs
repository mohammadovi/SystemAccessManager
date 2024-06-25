using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Win32;

namespace AppUI
{
    public partial class MainWindow : Window
    {
        const string RegistryKeyName = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
        const string RegistryValueName = "DisableTaskMgr";
        const string AppName = "MyApp"; // نام برنامه شما برای استفاده در رجیستری

        public MainWindow()
        {
            InitializeComponent();

            // بررسی برای اولین اجرا و افزودن به startup
            bool isFirstRun = CheckFirstRun();

            if (isFirstRun)
            {
                CreateStartupRegistryKey();
            }

            // اجرای برنامه به عنوان ادمین
            if (!IsRunAsAdmin())
            {
                ElevateToAdministrator();
            }
            else
            {
                // برنامه‌های اصلی را اینجا اجرا کنید
                // به عنوان مثال:
                SetTaskManagerStatus(1); // Disables Task Manager by default
            }
        }

        private void CustomSettings_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Enter 0 for active or 1 for not active:", "Custom Settings", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                int userChoice = GetUserChoice();
                SetTaskManagerStatus(userChoice);
            }
        }

        private void PredefinedSettings_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBlock.Text = "Using predefined settings...";
            SetTaskManagerStatus(1); // Disables Task Manager
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private int GetUserChoice()
        {
            // ایجاد یک پنجره ورودی برای دریافت انتخاب کاربر
            var inputDialog = new InputDialog("Enter 0 for active or 1 for not active:", "0");
            if (inputDialog.ShowDialog() == true)
            {
                if (int.TryParse(inputDialog.Answer, out int userChoice) && (userChoice == 0 || userChoice == 1))
                {
                    return userChoice;
                }
                else
                {
                    MessageBox.Show("Invalid choice. Please enter 0 or 1.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return GetUserChoice(); // تکرار ورودی در صورت نادرست بودن
                }
            }
            return 1; // مقدار پیش‌فرض
        }

        private void SetTaskManagerStatus(int status)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKeyName))
                {
                    key.SetValue(RegistryValueName, status, RegistryValueKind.DWord);
                }

                OutputTextBlock.Text = $"Task Manager status set to: {(status == 0 ? "Active" : "Not Active")}";
            }
            catch (Exception ex)
            {
                OutputTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private bool CheckFirstRun()
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

        private void CreateStartupRegistryKey()
        {
            // ایجاد کلید رجیستری برای اجرا خودکار
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                key.SetValue(AppName, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            }
            OutputTextBlock.Text = "Registry key for auto-start created.";
        }

        private bool IsRunAsAdmin()
        {
            using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }

        private void ElevateToAdministrator()
        {
            var exeName = Process.GetCurrentProcess().MainModule.FileName;
            var startInfo = new ProcessStartInfo(exeName)
            {
                UseShellExecute = true,
                Verb = "runas"
            };
            try
            {
                Process.Start(startInfo);
                Application.Current.Shutdown();
            }
            catch
            {
                MessageBox.Show("The application needs to be run as Administrator to perform this task.", "Elevation Required", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
    }


}
