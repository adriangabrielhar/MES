using System;
using System.Windows;
using MainApplication.Views;

namespace MainApplication
{
    /// <summary>
    /// Logica pentru MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // InitializeComponent este metoda generata automat care leaga XAML de C#
            // Daca apare cu rosu, asigura-te ca Namespace-ul (MainApplication) e identic in ambele fisiere
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Validare de siguranta pentru obiecte
            if (txtUsername == null || txtPassword == null) return;

            string username = txtUsername.Text.Trim().ToLower();
            string password = txtPassword.Password;

            if (username == "admin" && password == "1234")
            {
                DashboardWindow adminDash = new DashboardWindow();
                adminDash.Show();
                this.Close();
            }
            else if (username == "operator" && password == "0000")
            {
                DashboardOperator opDash = new DashboardOperator();
                opDash.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Date de autentificare incorecte!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}