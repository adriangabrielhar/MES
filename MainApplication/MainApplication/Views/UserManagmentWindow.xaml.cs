using System;
using System.Windows;
using System.Windows.Controls;

namespace MainApplication.Views
{
    public partial class UserManagementWindow : Window
    {
        public UserManagementWindow()
        {
            InitializeComponent();
        }

        private async void btnSaveUser_Click(object sender, RoutedEventArgs e)
        {
            string username = txtNewUsername.Text;
            string password = txtNewPassword.Password;
            string role = (cmbRole.SelectedItem as ComboBoxItem).Content.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("All fields are required!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ==========================================================
            // [BACK-END INTEGRATION ZONE]
            // Colegii de la Backend trebuie să urmeze acești pași:
            // 1. Instanțiați contextul bazei de date (MESDbContext).
            // 2. Verificați dacă username-ul există deja în tabelul Users.
            // 3. Criptați parola (Hashing).
            // 4. Salvați noul obiect User: context.Users.Add(newUser);
            // 5. await context.SaveChangesAsync();
            // ==========================================================

            // Mesaj de succes temporar (Simulare)
            MessageBox.Show($"Utilizatorul '{username}' a fost înregistrat cu succes ca '{role}'!",
                            "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }
    }
}