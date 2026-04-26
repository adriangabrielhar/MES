using System;
using MainApplication.Views;
using Xunit;

namespace MainApplication.Tests.ModelsTests
{
    public class ModelsTests
    {
        [Fact]
        public void MaterialItem_Should_Detect_Low_Stock()
        {
            var material = new MaterialItem
            {
                MaterialName = "Blue Ink",
                QuantityPercentage = 15
            };

            Assert.True(material.IsLowStock);
        }

        [Fact]
        public void MaterialItem_Should_Not_Be_Low_Stock()
        {
            var material = new MaterialItem
            {
                MaterialName = "PVC Granules",
                QuantityPercentage = 80
            };

            Assert.False(material.IsLowStock);
        }

        [Fact]
        public void MaterialItem_Should_Change_Selected_State()
        {
            var material = new MaterialItem();

            material.IsSelected = true;

            Assert.True(material.IsSelected);
        }

        [Fact]
        public void ProductionLine_Should_Store_Data()
        {
            var line = new ProductionLine
            {
                LineName = "LINE 01",
                IsOccupied = true,
                StatusText = "RUNNING",
                CurrentProduct = "Bottle 0.5L"
            };

            Assert.Equal("LINE 01", line.LineName);
            Assert.True(line.IsOccupied);
            Assert.Equal("RUNNING", line.StatusText);
            Assert.Equal("Bottle 0.5L", line.CurrentProduct);
        }

        [Fact]
        public void StockRequest_Should_Have_Id_And_Time()
        {
            var request = new StockRequest
            {
                Details = "Order from Operator: Blue Ink"
            };

            Assert.NotEqual(Guid.Empty, request.Id);
            Assert.Equal("Order from Operator: Blue Ink", request.Details);
            Assert.True(request.Time <= DateTime.Now);
        }

        [Fact]
        public void SessionManager_Should_Store_Pending_Request()
        {
            SessionManager.PendingRequests.Clear();

            var request = new StockRequest
            {
                Details = "Order from Operator: PVC Granules"
            };

            SessionManager.PendingRequests.Add(request);

            Assert.Single(SessionManager.PendingRequests);
            Assert.Equal("Order from Operator: PVC Granules", SessionManager.PendingRequests[0].Details);
        }
    }
}