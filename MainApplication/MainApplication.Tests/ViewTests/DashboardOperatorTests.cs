using System;
using System.Threading;
using System.Windows.Controls;
using MainApplication.Views;
using Xunit;

namespace MainApplication.Tests.ViewsTests
{
    public class DashboardOperatorTests
    {
        [Fact]
        public void DashboardOperator_Should_Be_Created()
        {
            RunOnStaThread(() =>
            {
                var window = new DashboardOperator();

                Assert.NotNull(window);
                Assert.Equal("Operator Terminal", window.Title);
                Assert.Equal(1250, window.Width);
                Assert.Equal(800, window.Height);

                window.Close();
            });
        }

        [Fact]
        public void DashboardOperator_Should_Load_Initial_Materials()
        {
            RunOnStaThread(() =>
            {
                var window = new DashboardOperator();

                Assert.NotNull(window.Materials);
                Assert.Equal(2, window.Materials.Count);

                Assert.Equal("PVC Granules", window.Materials[0].MaterialName);
                Assert.Equal(45, window.Materials[0].QuantityPercentage);

                Assert.Equal("Blue Ink", window.Materials[1].MaterialName);
                Assert.Equal(15, window.Materials[1].QuantityPercentage);

                window.Close();
            });
        }

        [Fact]
        public void DashboardOperator_Should_Load_Initial_Lines()
        {
            RunOnStaThread(() =>
            {
                var window = new DashboardOperator();

                Assert.NotNull(window.Lines);
                Assert.Equal(2, window.Lines.Count);

                Assert.Equal("LINE 01", window.Lines[0].LineName);
                Assert.True(window.Lines[0].IsOccupied);
                Assert.Equal("RUNNING", window.Lines[0].StatusText);

                Assert.Equal("LINE 02", window.Lines[1].LineName);
                Assert.False(window.Lines[1].IsOccupied);
                Assert.Equal("READY", window.Lines[1].StatusText);

                window.Close();
            });
        }

        [Fact]
        public void DashboardOperator_Should_Have_UI_Controls()
        {
            RunOnStaThread(() =>
            {
                var window = new DashboardOperator();

                var inventoryList = window.FindName("InventoryList") as ListBox;
                var linesControl = window.FindName("LinesControl") as ItemsControl;

                Assert.NotNull(inventoryList);
                Assert.NotNull(linesControl);

                window.Close();
            });
        }

        [Fact]
        public void DashboardOperator_Should_Bind_ItemsSources()
        {
            RunOnStaThread(() =>
            {
                var window = new DashboardOperator();

                var inventoryList = window.FindName("InventoryList") as ListBox;
                var linesControl = window.FindName("LinesControl") as ItemsControl;

                Assert.NotNull(inventoryList);
                Assert.NotNull(linesControl);

                Assert.Same(window.Materials, inventoryList!.ItemsSource);
                Assert.Same(window.Lines, linesControl!.ItemsSource);

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