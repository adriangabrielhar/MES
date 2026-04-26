using System;
using System.Threading;
using System.Windows.Controls;
using MainApplication.Views;
using Xunit;

namespace MainApplication.Tests.ViewsTests
{
    public class DashboardWindowTests
    {
        [Fact]
        public void DashboardWindow_Should_Be_Created()
        {
            RunOnStaThread(() =>
            {
                var window = new DashboardWindow();

                Assert.NotNull(window);
                Assert.Equal("Admin Console", window.Title);
                Assert.Equal(1200, window.Width);
                Assert.Equal(800, window.Height);

                window.Close();
            });
        }

        [Fact]
        public void DashboardWindow_Should_Have_DataContext()
        {
            RunOnStaThread(() =>
            {
                var window = new DashboardWindow();

                Assert.NotNull(window.DataContext);
                Assert.Same(window, window.DataContext);

                window.Close();
            });
        }

        [Fact]
        public void DashboardWindow_Should_Load_Initial_Data()
        {
            RunOnStaThread(() =>
            {
                var window = new DashboardWindow();

                Assert.NotNull(window.Materials);
                Assert.NotNull(window.Lines);

                Assert.Single(window.Materials);
                Assert.Single(window.Lines);

                Assert.Equal("PVC Granules", window.Materials[0].MaterialName);
                Assert.Equal("LINE 01", window.Lines[0].LineName);

                window.Close();
            });
        }

        [Fact]
        public void DashboardWindow_Should_Have_UI_Controls()
        {
            RunOnStaThread(() =>
            {
                var window = new DashboardWindow();

                Assert.NotNull(window.FindName("txtTotalLines") as TextBlock);
                Assert.NotNull(window.FindName("txtRunningLines") as TextBlock);
                Assert.NotNull(window.FindName("txtAvailableLines") as TextBlock);
                Assert.NotNull(window.FindName("LinesControl") as ItemsControl);
                Assert.NotNull(window.FindName("InventoryList") as ItemsControl);
                Assert.NotNull(window.FindName("lstRequests") as ListBox);

                window.Close();
            });
        }

        [Fact]
        public void DashboardWindow_Should_Update_Stats_On_Load()
        {
            RunOnStaThread(() =>
            {
                var window = new DashboardWindow();

                var totalLines = window.FindName("txtTotalLines") as TextBlock;
                var runningLines = window.FindName("txtRunningLines") as TextBlock;
                var availableLines = window.FindName("txtAvailableLines") as TextBlock;

                Assert.Equal("1", totalLines!.Text);
                Assert.Equal("1", runningLines!.Text);
                Assert.Equal("0", availableLines!.Text);

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
                throw exception;
        }
    }
}