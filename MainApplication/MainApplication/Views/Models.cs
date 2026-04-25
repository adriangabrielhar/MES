using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MainApplication.Views
{
    public class MaterialItem : INotifyPropertyChanged
    {
        private int _quantity;
        private bool _isSelected;

        public string MaterialName { get; set; } = string.Empty;
        public int QuantityPercentage
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsLowStock)); }
        }
        public bool IsLowStock => QuantityPercentage < 20;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null!) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class ProductionLine : INotifyPropertyChanged
    {
        private bool _isOccupied;
        private string _statusText = "AVAILABLE";
        private string _currentProduct = "None";

        public string LineName { get; set; } = string.Empty;
        public bool IsOccupied
        {
            get => _isOccupied;
            set { _isOccupied = value; OnPropertyChanged(); }
        }
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }
        public string CurrentProduct
        {
            get => _currentProduct;
            set { _currentProduct = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null!) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class StockRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Details { get; set; } = string.Empty;
        public DateTime Time { get; set; } = DateTime.Now;
    }

    public static class SessionManager
    {
        public static ObservableCollection<StockRequest> PendingRequests { get; set; } = new ObservableCollection<StockRequest>();
    }
}