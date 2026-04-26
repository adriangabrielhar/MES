using System;
using System.Threading;
using System.Windows.Controls;
using MainApplication.Views;
using Xunit;

namespace MainApplication.Tests.ViewsTests
{
    public class AdminApprovalWindowTests
    {
        [Fact]
        public void Window_Should_Be_Created()
        {
            RunOnStaThread(() =>
            {
                var window = new AdminApprovalWindow();

                Assert.NotNull(window);
                Assert.Equal("Pending Requests Approval", window.Title);
                Assert.Equal(500, window.Width);
                Assert.Equal(400, window.Height);

                window.Close();
            });
        }

        [Fact]
        public void Window_Should_Have_ListBox()
        {
            RunOnStaThread(() =>
            {
                var window = new AdminApprovalWindow();

                var list = window.FindName("lbRequests") as ListBox;

                Assert.NotNull(list);

                window.Close();
            });
        }

        [Fact]
        public void Window_Should_Have_StockFlag_DefaultFalse()
        {
            RunOnStaThread(() =>
            {
                var window = new AdminApprovalWindow();

                Assert.False(window.StockWasUpdated);

                window.Close();
            });
        }

        [Fact]
        public void Window_Should_Load_ItemsSource()
        {
            RunOnStaThread(() =>
            {
                var window = new AdminApprovalWindow();

                var list = window.FindName("lbRequests") as ListBox;

                Assert.NotNull(list);
                Assert.NotNull(list!.ItemsSource);

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