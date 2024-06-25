using System;
using System.IO;
using Xunit;
using Microsoft.Win32;
using SystemAccessManager.ConsoleAppPlus;

namespace SystemAccessManager.xUnitTestApp
{
    public class ProgramTests
    {
        /// <summary>
        /// This test method verifies that the GetUserChoice method returns the correct choice (0 or 1) based on user input.
        /// </summary>
        [Fact]
        public void GetUserChoice_ValidInput_ReturnsExpectedChoice()
        {
            // Arrange
            var input = new StringReader("0\n");
            Console.SetIn(input);

            // Act
            var result = Program.GetUserChoice();

            // Assert
            Assert.Equal(0, result);
        }

        /// <summary>
        /// This test method verifies that the CheckFirstRun method returns true when there is no registry value for the application,
        /// indicating that it is the first run of the application.
        /// </summary>
        [Fact]
        public void CheckFirstRun_NoRegistryValue_ReturnsTrue()
        {
            // Arrange
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                key.DeleteValue(Program.AppName, false);
            }

            // Act
            var result = Program.CheckFirstRun();

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// This test method verifies that the CheckFirstRun method returns false when there is a registry value for the application,
        /// indicating that it is not the first run of the application.
        /// </summary>
        [Fact]
        public void CheckFirstRun_RegistryValueExists_ReturnsFalse()
        {
            // Arrange
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                key.SetValue(Program.AppName, "dummyValue");
            }

            // Act
            var result = Program.CheckFirstRun();

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// This test method verifies that the SetTaskManagerStatus method sets the correct registry value.
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void SetTaskManagerStatus_SetsRegistryValue(int status)
        {
            // Act
            Program.SetTaskManagerStatus(status);

            // Assert
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Program.RegistryKeyName))
            {
                var value = key.GetValue(Program.RegistryValueName);
                Assert.Equal(status, (int)value);
            }
        }

        /// <summary>
        /// This test method verifies that the SetStartMenuStatus method sets the correct registry value.
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void SetStartMenuStatus_SetsRegistryValue(int status)
        {
            // Act
            Program.SetStartMenuStatus(status);

            // Assert
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Program.StartMenuRegistryKeyName))
            {
                var value = key.GetValue(Program.StartMenuRegistryValueName);
                Assert.Equal(status, (int)value);
            }
        }

        /// <summary>
        /// This test method verifies that the HideStartMenu method sets the correct registry value.
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HideStartMenu_SetsRegistryValue(bool hide)
        {
            // Act
            Program.HideStartMenu(hide);

            // Assert
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Program.StartMenuRegistryKeyName))
            {
                var value = key.GetValue("NoStartMenu");
                Assert.Equal(hide ? 1 : 0, (int)value);
            }
        }

        /// <summary>
        /// This test method verifies that the SetTaskbarStatus method sets the correct registry value.
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void SetTaskbarStatus_SetsRegistryValue(int status)
        {
            // Act
            Program.SetTaskbarStatus(status);

            // Assert
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Program.StartMenuRegistryKeyName))
            {
                var value = key.GetValue(Program.TaskbarRegistryValueName);
                Assert.Equal(status, (int)value);
            }
        }

        ///// <summary>
        ///// This test method verifies that the HideTaskbar method correctly hides and unhides the Taskbar.
        ///// </summary>
        //[Theory]
        //[InlineData(true)]
        //[InlineData(false)]
        //public void HideTaskbar_HidesAndUnhidesTaskbar(bool hide)
        //{
        //    // Act
        //    Program.HideTaskbar(hide);

        //    // Assert
        //    IntPtr taskbarHandle = Program.FindWindow("Shell_TrayWnd", null);
        //    Assert.NotEqual(IntPtr.Zero, taskbarHandle);

        //    // Assuming we have a way to check visibility status programmatically:
        //    // bool isVisible = IsWindowVisible(taskbarHandle);
        //    // Assert.Equal(!hide, isVisible);
        //}
    }
}
