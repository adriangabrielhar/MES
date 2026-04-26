using System.Threading;
using System.Windows.Controls;
using MainApplication;
using Xunit;

namespace MainApplication.Tests.ViewsTests
{
    public class MainWindowTests
    {
        [Fact]
        public void MainWindow_Should_Be_Created()
        {
            RunOnStaThread(() =>
            {
                var window = new MainWindow();

                Assert.NotNull(window);
                Assert.Equal("XACT-FIL MES - Login", window.Title);

                window.Close();
            });
        }

        [Fact]
        public void MainWindow_Should_Have_Username_TextBox()
        {
            RunOnStaThread(() =>
            {
                var window = new MainWindow();

                var usernameBox = window.FindName("txtUsername") as TextBox;

                Assert.NotNull(usernameBox);

                window.Close();
            });
        }

        [Fact]
        public void MainWindow_Should_Have_PasswordBox()
        {
            RunOnStaThread(() =>
            {
                var window = new MainWindow();

                var passwordBox = window.FindName("txtPassword") as PasswordBox;

                Assert.NotNull(passwordBox);

                window.Close();
            });
        }

        private static void RunOnStaThread(ThreadStart testCode)
        {
            Exception? exception = null;

            var thread = new Thread(() =>
            {
                try
                {
                    testCode();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null)
            {
                throw exception;
            }
        }
    }
}