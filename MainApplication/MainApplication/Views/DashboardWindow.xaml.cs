using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MainApplication.Models;

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
            LoadDataFromDatabase();
            // 2. IMPORTANT: Spunem interfeței să se uite la colecția "Lines"
            LinesControl.ItemsSource = Lines;
            InventoryList.ItemsSource = Materials; // Nu uita de inventar
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

        private async void btnAddLine_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new MESDbContext())
            {
                int count = context.Workstations.Count();
                if (count >= 10)
                {
                    MessageBox.Show("Limita maximă de 10 stații de lucru a fost atinsă!", "Limită", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // AM MODIFICAT AICI: Folosim WorkstationName
                var newStation = new Workstation
                {
                    WorkstationName = $"LINE {(count + 1):D2}",
                    IsOnline = true,
                    CurrentOrderId = null
                };

                try
                {
                    context.Workstations.Add(newStation);
                    await context.SaveChangesAsync();

                    LoadDataFromDatabase();

                    // AM MODIFICAT AICI
                    MessageBox.Show($"Stația {newStation.WorkstationName} a fost adăugată cu succes!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Eroare la adăugarea liniei: {ex.Message}");
                }
            }
        }

        private void LoadDataFromDatabase()
        {
            using (var context = new MESDbContext())
            {
                var workstations = context.Workstations.ToList();

                Lines.Clear();

                foreach (var station in workstations)
                {
                    Lines.Add(new ProductionLine
                    {
                        // AM MODIFICAT AICI: Folosim WorkstationName.
                        LineName = station.WorkstationName,
                        IsOccupied = station.IsOnline,
                        StatusText = station.IsOnline ? "ONLINE" : "OFFLINE",
                        CurrentProduct = "Fără Produs"
                    });
                }

                UpdateStats();
            }
        }

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