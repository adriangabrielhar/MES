using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using MainApplication.Views;
using Xunit;

namespace MainApplication.Tests.ViewsTests
{
    public class RequestStockWindowTests
    {
        [Fact]
        public void RequestStockWindow_Should_Be_Created()
        {
            RunOnStaThread(() =>
            {
                var materials = new List<string> { "PVC Granules", "Blue Ink" };

                var window = new RequestStockWindow(materials);

                Assert.NotNull(window);
                Assert.Equal("Select Material Request", window.Title);
                Assert.Equal(400, window.Width);
                Assert.Equal(500, window.Height);

                window.Close();
            });
        }

        [Fact]
        public void RequestStockWindow_Should_Load_Materials()
        {
            RunOnStaThread(() =>
            {
                var materials = new List<string> { "PVC Granules", "Blue Ink" };

                var window = new RequestStockWindow(materials);

                Assert.NotNull(window.AllMaterials);
                Assert.Equal(2, window.AllMaterials.Count);
                Assert.Equal("PVC Granules", window.AllMaterials[0].MaterialName);
                Assert.Equal("Blue Ink", window.AllMaterials[1].MaterialName);

                window.Close();
            });
        }

        [Fact]
        public void RequestStockWindow_Should_Have_UI_Controls()
        {
            RunOnStaThread(() =>
            {
                var materials = new List<string> { "PVC Granules", "Blue Ink" };

                var window = new RequestStockWindow(materials);

                var searchBox = window.FindName("txtSearch") as TextBox;
                var listBox = window.FindName("lbMaterials") as ListBox;

                Assert.NotNull(searchBox);
                Assert.NotNull(listBox);

                window.Close();
            });
        }

        [Fact]
        public void RequestStockWindow_Should_Bind_Materials_To_ListBox()
        {
            RunOnStaThread(() =>
            {
                var materials = new List<string> { "PVC Granules", "Blue Ink" };

                var window = new RequestStockWindow(materials);

                var listBox = window.FindName("lbMaterials") as ListBox;

                Assert.NotNull(listBox);
                Assert.Same(window.AllMaterials, listBox!.ItemsSource);

                window.Close();
            });
        }

        [Fact]
        public void Search_Should_Filter_Materials()
        {
            RunOnStaThread(() =>
            {
                var materials = new List<string> { "PVC Granules", "Blue Ink", "Cap Red" };

                var window = new RequestStockWindow(materials);

                var searchBox = window.FindName("txtSearch") as TextBox;
                var listBox = window.FindName("lbMaterials") as ListBox;

                searchBox!.Text = "blue";

                var filteredItems = listBox!.ItemsSource.Cast<RequestItem>().ToList();

                Assert.Single(filteredItems);
                Assert.Equal("Blue Ink", filteredItems[0].MaterialName);

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