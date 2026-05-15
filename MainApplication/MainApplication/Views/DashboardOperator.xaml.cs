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
            // Golim listele înainte de a le umple cu date noi
            Materials.Clear();
            Lines.Clear();

            try
            {
                using (var context = new MESDbContext())
                {
                    // ==========================================
                    // 1. ÎNCĂRCAREA STOCULUI DIN BAZA DE DATE
                    // ==========================================
                    var inventoryItems = context.InventoryItems.ToList();

                    foreach (var item in inventoryItems)
                    {
                        Materials.Add(new MaterialItem
                        {
                            MaterialName = item.ItemName,

                            // ProgressBar-ul din interfață așteaptă o valoare între 0 și 100.
                            // Dacă AvailableQuantity este direct procentul, îl lăsăm așa.
                            // Dacă AvailableQuantity reprezintă bucăți/kg (ex: 500), ar trebui să 
                            // calculezi procentul raportat la un maxim (ex: (item.AvailableQuantity * 100) / Max)
                            QuantityPercentage = item.AvailableQuantity
                        });
                    }

                    // ==========================================
                    // 2. ÎNCĂRCAREA LINIILOR DIN BAZA DE DATE
                    // ==========================================
                    var workstations = context.Workstations.ToList();

                    foreach (var station in workstations)
                    {
                        Lines.Add(new ProductionLine
                        {
                            LineName = station.WorkstationName,
                            IsOccupied = station.IsOnline,
                            StatusText = station.IsOnline ? "ONLINE" : "OFFLINE",
                            CurrentProduct = "În așteptare...",
                            LineType = station.LineType
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la conectarea cu Azure: " + ex.Message, "Eroare BD", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // 3. Legăm listele de pe interfață (chiar dacă sunt legate și în XAML, ne asigurăm aici)
            //InventoryList.ItemsSource = Materials;
            LinesControl.ItemsSource = Lines;
        }

        private async void btnRequestStock_Click(object sender, RoutedEventArgs e)
        {
            var selectedMaterials = Materials.Where(m => m.IsSelected).ToList();

            if (!selectedMaterials.Any())
            {
                MessageBox.Show("Vă rugăm să selectați cel puțin un material.", "Avertisment", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new MESDbContext())
                {
                    // 1. Creăm o comandă NOUĂ în baza de date pentru a obține un ID proaspăt
                    var newOrder = new ProductionOrder
                    {
                        ProductName = "Cerere Materiale Operator",
                        RequiredQuantity = 0,
                        LaunchDate = DateTime.Now,
                        StatusName = "New"
                    };

                    context.ProductionOrders.Add(newOrder);
                    context.SaveChanges(); // După această linie, baza de date îi asignează automat următorul ID (ex: 2, 3, 4...)

                    // 2. Extragem ID-ul proaspăt generat
                    int currentOrderId = newOrder.Id;

                    foreach (var material in selectedMaterials)
                    {
                        // Găsim ItemId-ul materialului bifat
                        var dbItem = context.InventoryItems.FirstOrDefault(i => i.ItemName == material.MaterialName);

                        if (dbItem != null)
                        {
                            // 3. Adăugăm materialele la NOUL OrderId
                            var newRequest = new OrderMaterial
                            {
                                OrderId = currentOrderId, // Acum va fi mereu unul nou la fiecare apăsare de buton!
                                ItemId = dbItem.Id,
                                QuantityNeeded = 0
                            };

                            context.OrderMaterials.Add(newRequest);
                        }
                    }

                    // 4. Salvăm detaliile cererii
                    await context.SaveChangesAsync();

                    MessageBox.Show($"Cererea a fost salvată cu succes sub ID-ul: {currentOrderId}!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Debifăm căsuțele
                    foreach (var mat in selectedMaterials)
                    {
                        mat.IsSelected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvarea în baza de date: {ex.InnerException?.Message ?? ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void LineCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Verificăm pe care card de linie a dat click operatorul
            if ((sender as FrameworkElement)?.DataContext is ProductionLine line)
            {
                // Creăm o nouă fereastră Faceplate și îi pasăm linia selectată
                var faceplate = new LineFaceplate(line)
                {
                    Owner = this // Setăm ca fereastra curentă (Operatorul) să fie "proprietarul" ei
                };

                // Deschidem fereastra ca un Dialog (adică operatorul nu poate da click altundeva până nu o închide)
                faceplate.ShowDialog();

                // Opțional: Reîmprospătăm listele după ce se închide fereastra, 
                // în cazul în care s-a schimbat statusul din Faceplate
                LinesControl.Items.Refresh();
            }
        }
    }
}