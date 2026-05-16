using MainApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MainApplication.Views
{
    public partial class AdminApprovalWindow : Window
    {
        public bool StockWasUpdated { get; set; } = false;

        public AdminApprovalWindow()
        {
            InitializeComponent();
            LoadPendingOrdersFromDB();
        }

        private void LoadPendingOrdersFromDB()
        {
            try
            {
                using (var context = new MESDbContext())
                {
                    // Luăm datele din DB
                    var orderMaterials = context.OrderMaterials.ToList();
                    var inventoryItems = context.InventoryItems.ToList();

                    // Grupăm după OrderId (comenzile venite de la linii)
                    var groupedRequests = orderMaterials.GroupBy(om => om.OrderId);
                    var displayList = new List<PendingOrderDisplay>();

                    foreach (var group in groupedRequests)
                    {
                        var detailsList = group.Select(om =>
                        {
                            var itemName = inventoryItems.FirstOrDefault(i => i.Id == om.ItemId)?.ItemName ?? "Unknown";
                            return $"{om.QuantityNeeded}x {itemName}";
                        });

                        // Construim obiectul pe care îl așteaptă XAML-ul tău
                        displayList.Add(new PendingOrderDisplay
                        {
                            Id = group.Key, // Acesta e OrderId (int)
                            Details = $"Comanda #{group.Key}:\n{string.Join(", ", detailsList)}",
                            Time = DateTime.Now.ToString("HH:mm") // Timpul curent
                        });
                    }

                    lbRequests.ItemsSource = displayList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea comenzilor: {ex.Message}", "Eroare DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Metoda pentru butonul de Aprobare (✔)
        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int orderId) // Am schimbat Guid cu int
            {
                try
                {
                    using (var context = new MESDbContext())
                    {
                        // 1. Luăm toate materialele din această comandă
                        var orderItems = context.OrderMaterials.Where(o => o.OrderId == orderId).ToList();

                        // 2. Adăugăm cantitatea în inventar
                        foreach (var item in orderItems)
                        {
                            var dbItem = context.InventoryItems.FirstOrDefault(i => i.Id == item.ItemId);
                            if (dbItem != null)
                            {
                                dbItem.AvailableQuantity += item.QuantityNeeded;
                            }
                        }

                        // 3. Ștergem comanda din coada de așteptare
                        context.OrderMaterials.RemoveRange(orderItems);
                        context.SaveChanges();
                    }

                    StockWasUpdated = true;
                    LoadPendingOrdersFromDB(); // Actualizăm lista pe ecran
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Eroare la aprobare: {ex.Message}");
                }
            }
        }

        // Metoda pentru butonul de Respingere (✖)
        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int orderId) // Am schimbat Guid cu int
            {
                try
                {
                    using (var context = new MESDbContext())
                    {
                        // Doar ștergem comanda fără să modificăm stocul
                        var orderItems = context.OrderMaterials.Where(o => o.OrderId == orderId).ToList();
                        context.OrderMaterials.RemoveRange(orderItems);
                        context.SaveChanges();
                    }
                    LoadPendingOrdersFromDB(); // Actualizăm lista pe ecran
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Eroare la respingere: {ex.Message}");
                }
            }
        }

        // Metoda pentru butonul Close
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    // Clasă creată special pentru a se potrivi cu structura cerută de interfața ta XAML (Id, Details, Time)
    public class PendingOrderDisplay
    {
        public int Id { get; set; }
        public string Details { get; set; }
        public string Time { get; set; }
    }
}