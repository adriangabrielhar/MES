using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MainApplication.Views
{
    public partial class LineFaceplate : Window
    {
        private ProductionLine _line;

        public LineFaceplate(ProductionLine line)
        {
            InitializeComponent();
            _line = line;

            // BACKEND_TODO: Sincronizați cu baza de date (Inventory) pentru lista materialelor specifice acestei linii
            lstRestockItems.ItemsSource = new List<MaterialStock> {
                new MaterialStock { Name = "Preforme PET 1.5L" },
                new MaterialStock { Name = "Capace Plastic" },
                new MaterialStock { Name = "Etichete Aqua" },
                new MaterialStock { Name = "Sticlă 0.33L" },
                new MaterialStock { Name = "Capace Metal" }
            };

            // BACKEND_TODO: Query pentru produsele ce pot fi rulate pe această linie
            cmbProductSelection.ItemsSource = new List<string> { "Bottle 1.5L PET", "Bottle 0.5L PET", "Glass 0.33L" };

            UpdateUI();
        }

        private void UpdateUI()
        {
            txtLineName.Text = _line.LineName;
            txtProduct.Text = string.IsNullOrEmpty(_line.CurrentProduct) ? "Product: În așteptare..." : $"Product: {_line.CurrentProduct}";
            lblStatus.Text = _line.StatusText;

            // Actualizarea culorii header-ului în funcție de stare
            if (_line.StatusText == "RUNNING") brdStatusBadge.Background = Brushes.Green;
            else if (_line.StatusText == "STOPPED") brdStatusBadge.Background = Brushes.Orange;
            else if (_line.StatusText == "EMERGENCY") brdStatusBadge.Background = Brushes.Red;
            else brdStatusBadge.Background = Brushes.Gray;
        }

        private void cmbProductSelection_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string selected = cmbProductSelection.SelectedItem as string;

            // Simulare logică Semafor BOM
            // BACKEND_TODO: Verificați stocul real (Local Stock vs Required) pentru culorile Verde/Galben/Roșu
            if (selected == "Bottle 1.5L PET")
            {
                icBOMStatus.ItemsSource = new List<BOMStatusItem> {
                    new BOMStatusItem { MaterialName = "Preforme PET 1.5L", StatusColor = Brushes.Green },
                    new BOMStatusItem { MaterialName = "Capace Plastic", StatusColor = Brushes.Yellow },
                    new BOMStatusItem { MaterialName = "Etichete Aqua", StatusColor = Brushes.Green }
                };
                txtTimeRemaining.Text = "00:25:00";
            }
            else if (selected == "Glass 0.33L")
            {
                icBOMStatus.ItemsSource = new List<BOMStatusItem> {
                    new BOMStatusItem { MaterialName = "Sticlă Golașă 0.33L", StatusColor = Brushes.Red },
                    new BOMStatusItem { MaterialName = "Capace Metalice", StatusColor = Brushes.Green },
                    new BOMStatusItem { MaterialName = "Etichete Premium", StatusColor = Brushes.Green }
                };
                txtTimeRemaining.Text = "00:45:00";
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProductSelection.SelectedItem == null)
            {
                MessageBox.Show("Vă rugăm selectați un produs înainte de a porni linia.");
                return;
            }

            _line.IsOccupied = true;
            _line.StatusText = "RUNNING";
            _line.CurrentProduct = cmbProductSelection.SelectedItem.ToString();
            UpdateUI();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _line.IsOccupied = false;
            _line.StatusText = "STOPPED";
            UpdateUI();
        }

        private void btnEStop_Click(object sender, RoutedEventArgs e)
        {
            _line.IsOccupied = true;
            _line.StatusText = "EMERGENCY";
            UpdateUI();
            MessageBox.Show("OPRIRE DE URGENȚĂ ACTIVATĂ!", "PLC ALERT", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnRequestRestock_Click(object sender, RoutedEventArgs e)
        {
            var mat = (sender as FrameworkElement).Tag.ToString();
            MessageBox.Show($"Cerere de reaprovizionare înregistrată pentru: {mat}");
        }

        private void btnMaintenance_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Departamentul de mentenanță a fost notificat.");
        }
    }

    // CLASE DE SUPORT PENTRU BINDING (Asigură-te că sunt incluse în namespace)
    public class MaterialStock { public string Name { get; set; } }
    public class BOMStatusItem { public string MaterialName { get; set; } public Brush StatusColor { get; set; } }
}