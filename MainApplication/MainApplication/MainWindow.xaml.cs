/* * Proiect: MES Interface
 * Modul: Gestiune Acces Utilizatori
 * Autor: Ciulavu Paul
 * Data Creării: 2026-04-11
 * Descriere: Gestiune logica intre utilizatori
 */
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainApplication
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUsername.Text;
            string pass = txtPassword.Password;

            //TO DO: Apeleaza utilizatorii dintr-o baza de date

            if (user == "admin" && pass == "1234")
            {
                MessageBox.Show("Acces Grantat: ADMINISTRATOR", "Sistem Industrial", MessageBoxButton.OK, MessageBoxImage.Information);
                txtPassword.Clear();
            }
            else if (user == "operator" && pass == "0000")
            {
                MessageBox.Show("Acces Grantat: OPERATOR", "Sistem Industrial", MessageBoxButton.OK, MessageBoxImage.Information);
                txtPassword.Clear();
            }
            else
            {
                MessageBox.Show("Eroare: Utilizator sau parolă incorecte", "Acces Respins", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}