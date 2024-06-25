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
    }
}
