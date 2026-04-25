using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MainApplication.Views
{
    public partial class DashboardWindow : Window
    {
        public ObservableCollection<MaterialItem> Materials { get; set; } = new ObservableCollection<MaterialItem>();
        public ObservableCollection<ProductionLine> Lines { get; set; } = new ObservableCollection<ProductionLine>();
        public List<string> AvailableProducts { get; set; } = new List<string> { "Bottle 0.5L", "Cap Red", "None" };

        public DashboardWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadData();
            Lines.CollectionChanged += (s, e) => UpdateStats();
            lstRequests.ItemsSource = SessionManager.PendingRequests;
        }

        private void LoadData()
        {
            Materials.Add(new MaterialItem { MaterialName = "PVC Granules", QuantityPercentage = 80 });
            Lines.Add(new ProductionLine { LineName = "LINE 01", IsOccupied = true, StatusText = "RUNNING", CurrentProduct = "Bottle 0.5L" });
            InventoryList.ItemsSource = Materials;
            LinesControl.ItemsSource = Lines;
            UpdateStats();
        }

        private void UpdateStats()
        {
            if (txtTotalLines == null) return;
            txtTotalLines.Text = Lines.Count.ToString();
            txtRunningLines.Text = Lines.Count(l => l.IsOccupied).ToString();
            txtAvailableLines.Text = (Lines.Count - Lines.Count(l => l.IsOccupied)).ToString();
        }

        private void btnAddLine_Click(object sender, RoutedEventArgs e) => Lines.Add(new ProductionLine { LineName = "NEW LINE", StatusText = "READY" });

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            foreach (var m in Materials) m.QuantityPercentage = 100;
            SessionManager.PendingRequests.Clear();
            InventoryList.Items.Refresh();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void LineCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is ProductionLine line)
            {
                // new LineFaceplate(line) { Owner = this }.ShowDialog();
                UpdateStats();
            }
        }
    }
}