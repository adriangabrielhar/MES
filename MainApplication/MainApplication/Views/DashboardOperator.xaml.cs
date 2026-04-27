using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MainApplication.Models;

namespace MainApplication.Views
{
    public partial class DashboardOperator : Window
    {
        public ObservableCollection<MaterialItem> Materials { get; set; } = new ObservableCollection<MaterialItem>();
        public ObservableCollection<ProductionLine> Lines { get; set; } = new ObservableCollection<ProductionLine>();

        public DashboardOperator()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            // 1. Păstrăm materialele de test pentru inventar (până le vom muta și pe ele în baza de date)
            Materials.Clear();
            Materials.Add(new MaterialItem { MaterialName = "PVC Granules", QuantityPercentage = 45 });
            Materials.Add(new MaterialItem { MaterialName = "Blue Ink", QuantityPercentage = 15 });

            // 2. Citim liniile din baza de date
            try
            {
                using (var context = new MESDbContext())
                {
                    var workstations = context.Workstations.ToList();

                    Lines.Clear();

                    foreach (var station in workstations)
                    {
                        Lines.Add(new ProductionLine
                        {
                            LineName = station.WorkstationName, // Numele liniei din baza de date
                            IsOccupied = station.IsOnline,
                            StatusText = station.IsOnline ? "ONLINE" : "OFFLINE",
                            CurrentProduct = "În așteptare..." // Sau "Fără Produs"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la încărcarea liniilor din baza de date: " + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // 3. Legăm listele de pe interfață la datele noastre
            InventoryList.ItemsSource = Materials;
            LinesControl.ItemsSource = Lines;
        }

        // METODELE CARE LIPSEAU ÎN EROAREA TA:
        private void btnRequestStock_Click(object sender, RoutedEventArgs e)
        {
            var selected = Materials.Where(m => m.IsSelected).Select(m => m.MaterialName).ToList();
            if (selected.Any())
            {
                SessionManager.PendingRequests.Add(new StockRequest
                {
                    Details = $"Order from Operator: {string.Join(", ", selected)}"
                });
                MessageBox.Show("Request sent to Admin.");
            }
            else
            {
                MessageBox.Show("Select at least one material.");
            }
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
            }
        }
    }
}