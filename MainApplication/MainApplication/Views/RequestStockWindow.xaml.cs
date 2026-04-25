using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MainApplication.Views
{
    // Model special pentru fereastra de selecție
    public class RequestItem
    {
        public string MaterialName { get; set; }
        public bool IsSelected { get; set; }
        public string RequestedQuantity { get; set; } = "100";
    }

    public partial class RequestStockWindow : Window
    {
        public ObservableCollection<RequestItem> AllMaterials { get; set; }
        public List<string> FinalRequestResult { get; private set; }

        public RequestStockWindow(List<string> existingMaterials)
        {
            InitializeComponent();

            // Transformăm lista de nume în obiecte selectabile
            AllMaterials = new ObservableCollection<RequestItem>(
                existingMaterials.Select(m => new RequestItem { MaterialName = m })
            );
            lbMaterials.ItemsSource = AllMaterials;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = txtSearch.Text.ToLower();
            if (string.IsNullOrEmpty(filter))
                lbMaterials.ItemsSource = AllMaterials;
            else
                lbMaterials.ItemsSource = AllMaterials.Where(m => m.MaterialName.ToLower().Contains(filter)).ToList();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            // Colectăm doar ce a fost bifat
            var selected = AllMaterials.Where(m => m.IsSelected).ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Vă rugăm selectați cel puțin un material.");
                return;
            }

            FinalRequestResult = selected.Select(s => $"{s.MaterialName} (Cantitate: {s.RequestedQuantity})").ToList();
            this.DialogResult = true; // Închide fereastra și confirmă succesul
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => this.DialogResult = false;
    }
}