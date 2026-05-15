using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MainApplication.Models;
using System.Windows.Threading;

namespace MainApplication.Views
{
    public partial class DashboardWindow : Window
    {
        public ObservableCollection<ProductionLine> Lines { get; set; } = new ObservableCollection<ProductionLine>();

        public DashboardWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            LoadDataFromDatabase();
            LoadRequestsFromDatabase();

            DispatcherTimer autoRefreshTimer = new DispatcherTimer();
            autoRefreshTimer.Interval = TimeSpan.FromSeconds(5);
            autoRefreshTimer.Tick += (sender, args) => LoadRequestsFromDatabase();
            autoRefreshTimer.Start();
        }

        private void UpdateStats()
        {
            if (txtTotalLines == null) return;
            txtTotalLines.Text = Lines.Count.ToString();
            txtRunningLines.Text = Lines.Count(l => l.IsOccupied).ToString();
            txtAvailableLines.Text = (Lines.Count - Lines.Count(l => l.IsOccupied)).ToString();
        }

        // DESCHIDE FEREASTRA DE GESTIONARE USERI
        private void btnManageUsers_Click(object sender, RoutedEventArgs e)
        {
            UserManagementWindow userWin = new UserManagementWindow();
            userWin.Owner = this;
            userWin.ShowDialog();
        }

        // DESCHIDE FEREASTRA DE APROBĂRI CERERI
        private void btnOpenApproval_Click(object sender, RoutedEventArgs e)
        {
            AdminApprovalWindow approvalWin = new AdminApprovalWindow();
            approvalWin.Owner = this;
            approvalWin.Closed += (s, args) => LoadRequestsFromDatabase();
            approvalWin.ShowDialog();
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
                        LineName = station.WorkstationName,
                        IsOccupied = station.IsOnline,
                        StatusText = station.IsOnline ? "ONLINE" : "OFFLINE",
                        CurrentProduct = "None",
                        LineType = station.LineType ?? "Final Product"
                    });
                }
                LinesControl.ItemsSource = Lines;
                UpdateStats();
            }
        }

        private void LoadRequestsFromDatabase()
        {
            try
            {
                using (var context = new MESDbContext())
                {
                    var orderMaterials = context.OrderMaterials.ToList();
                    var inventoryItems = context.InventoryItems.ToList();
                    var groupedRequests = orderMaterials.GroupBy(om => om.OrderId);

                    var displayList = new List<StockRequest>();
                    foreach (var group in groupedRequests)
                    {
                        var itemNames = group.Select(om => inventoryItems.FirstOrDefault(i => i.Id == om.ItemId)?.ItemName ?? "Unknown").ToList();
                        displayList.Add(new StockRequest { Details = $"Order #{group.Key}: {string.Join(", ", itemNames)}" });
                    }
                    lstRequests.ItemsSource = displayList;
                }
            }
            catch { /* Log error */ }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void btnAddLine_Click(object sender, RoutedEventArgs e)
        {
            AddLineWindow addLineWin = new AddLineWindow();
            addLineWin.Owner = this;
            if (addLineWin.ShowDialog() == true)
            {
                LoadDataFromDatabase();
            }
        }

        private void LineCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Verificăm pe care linie a dat click Admin-ul
            if ((sender as FrameworkElement)?.DataContext is ProductionLine line)
            {
                var faceplate = new LineFaceplate(line)
                {
                    Owner = this
                };

                faceplate.ShowDialog();

                // Reîmprospătăm interfața după ce se închide fațada, pentru a vedea noile statusuri
                LoadDataFromDatabase();
            }
        }
    }
}
