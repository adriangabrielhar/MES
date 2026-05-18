using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MainApplication.Models;
using System.Windows.Threading;
using System.Windows.Controls;

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

            // 1. Numărul total de linii
            txtTotalLines.Text = Lines.Count.ToString();

            // 2. Liniile care sunt cu ADEVĂRAT în producție (au statusul RUNNING)
            int runningCount = Lines.Count(l => l.StatusText == "RUNNING" && l.IsOccupied);
            txtRunningLines.Text = runningCount.ToString();

            // 3. Liniile disponibile (Total minus cele care rulează)
            int availableCount = Lines.Count - runningCount;
            txtAvailableLines.Text = availableCount.ToString();
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
                        IsOccupied = station.CurrentStatus == "RUNNING",
                        StatusText = station.CurrentStatus ?? "ONLINE",
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

        private void btnDeleteLine_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new MESDbContext())
                {
                    // Preluăm numele tuturor liniilor existente în baza de date
                    var lineNames = context.Workstations.Select(w => w.WorkstationName).ToList();

                    if (!lineNames.Any())
                    {
                        MessageBox.Show("Nu există linii de producție disponibile pentru ștergere.", "Informație", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    // Generăm o mini-fereastră de dialog direct din cod pentru selecție rapidă
                    Window dialog = new Window
                    {
                        Title = "Șterge Linie de Producție",
                        Width = 350,
                        Height = 180,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        Owner = this,
                        ResizeMode = ResizeMode.NoResize,
                        Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F0F2F5"))
                    };

                    StackPanel panel = new StackPanel { Margin = new Thickness(20) };

                    panel.Children.Add(new TextBlock { Text = "Selectați linia pe care doriți să o ștergeți:", Margin = new Thickness(0, 0, 0, 10), FontWeight = FontWeights.Medium });

                    ComboBox cmbLines = new ComboBox { ItemsSource = lineNames, Height = 30, VerticalContentAlignment = VerticalAlignment.Center };
                    cmbLines.SelectedIndex = 0;
                    panel.Children.Add(cmbLines);

                    Button btnDelete = new Button
                    {
                        Content = "ȘTERGE LINIA",
                        Height = 32,
                        Margin = new Thickness(0, 15, 0, 0),
                        Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#D9534F")),
                        Foreground = System.Windows.Media.Brushes.White,
                        FontWeight = FontWeights.Bold
                    };

                    // Logica la apasarea butonului din mini-dialog
                    btnDelete.Click += (s, args) =>
                    {
                        string selectedLine = cmbLines.SelectedItem as string;
                        if (string.IsNullOrEmpty(selectedLine)) return;

                        var result = MessageBox.Show($"Sunteți sigur că doriți să ștergeți definitiv linia '{selectedLine}'?\nAceastă acțiune o va elimina și din baza de date.", "Confirmare Ștergere", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                using (var deleteContext = new MESDbContext())
                                {
                                    var station = deleteContext.Workstations.FirstOrDefault(w => w.WorkstationName == selectedLine);
                                    if (station != null)
                                    {
                                        deleteContext.Workstations.Remove(station);
                                        deleteContext.SaveChanges();
                                    }
                                }
                                dialog.DialogResult = true;
                                dialog.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Eroare la ștergerea din baza de date: {ex.Message}", "Eroare DB", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    };

                    panel.Children.Add(btnDelete);
                    dialog.Content = panel;

                    // Dacă ștergerea s-a finalizat cu succes, reîmprospătăm interfața principală
                    if (dialog.ShowDialog() == true)
                    {
                        MessageBox.Show("Linia de producție a fost eliminată cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDataFromDatabase(); // Actualizează dashboard-ul și cardurile instant
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea datelor: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
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
