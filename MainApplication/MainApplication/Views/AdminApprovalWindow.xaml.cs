using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MainApplication.Views
{
    public partial class AdminApprovalWindow : Window
    {
        // Aceasta este proprietatea care lipsea în eroarea ta (CS1061)
        public bool StockWasUpdated { get; set; } = false;

        public AdminApprovalWindow()
        {
            InitializeComponent();
            // Legăm lista de cereri la ListBox
            lbRequests.ItemsSource = SessionManager.PendingRequests;
        }

        // Metoda pentru butonul de Aprobare (✔)
        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Guid id)
            {
                var request = SessionManager.PendingRequests.FirstOrDefault(r => r.Id == id);
                if (request != null)
                {
                    SessionManager.PendingRequests.Remove(request);
                    StockWasUpdated = true;
                    MessageBox.Show("Cerere aprobată!");
                }
            }
        }

        // Metoda pentru butonul de Respingere (✖)
        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Guid id)
            {
                var request = SessionManager.PendingRequests.FirstOrDefault(r => r.Id == id);
                if (request != null)
                {
                    SessionManager.PendingRequests.Remove(request);
                    MessageBox.Show("Cerere respinsă.");
                }
            }
        }

        // Metoda pentru butonul Close
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}