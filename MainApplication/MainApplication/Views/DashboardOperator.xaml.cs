using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
            Materials.Add(new MaterialItem { MaterialName = "PVC Granules", QuantityPercentage = 45 });
            Materials.Add(new MaterialItem { MaterialName = "Blue Ink", QuantityPercentage = 15 });

            Lines.Add(new ProductionLine { LineName = "LINE 01", IsOccupied = true, StatusText = "RUNNING", CurrentProduct = "Bottle 0.5L" });
            Lines.Add(new ProductionLine { LineName = "LINE 02", IsOccupied = false, StatusText = "READY" });

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