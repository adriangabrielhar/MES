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
        private async void btnRequestStock_Click(object sender, RoutedEventArgs e)
        {
            // Căutăm materialele pe care le-a bifat operatorul
            var selectedMaterials = Materials.Where(m => m.IsSelected).ToList();

            if (!selectedMaterials.Any())
            {
                MessageBox.Show("Te rog să selectezi cel puțin un material!", "Avertisment", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new MESDbContext())
                {
                    foreach (var material in selectedMaterials)
                    {
                        // Găsim ID-ul materialului din baza de date pe baza numelui
                        // (Presupunând că ai deja materialele "PVC Granules" etc. în tabela InventoryItems)
                        var dbItem = context.InventoryItems.FirstOrDefault(i => i.ItemName == material.MaterialName);

                        if (dbItem != null)
                        {
                            // Creăm cererea de materiale în tabelul de legătură
                            var newRequest = new OrderMaterial
                            {
                                OrderId = 1, // ID-ul comenzii la care lucrează operatorul acum
                                ItemId = dbItem.Id,
                                QuantityNeeded = 50 // O cantitate standard sau preluată din interfață
                            };

                            context.OrderMaterials.Add(newRequest);
                        }
                    }

                    // Salvăm toate cererile în baza de date
                    await context.SaveChangesAsync();
                    MessageBox.Show("Cererea de materiale a fost trimisă cu succes către Admin!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Debifăm căsuțele după trimitere
                    foreach (var mat in selectedMaterials) mat.IsSelected = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la trimiterea cererii: {ex.Message}", "Eroare Bază de Date", MessageBoxButton.OK, MessageBoxImage.Error);
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