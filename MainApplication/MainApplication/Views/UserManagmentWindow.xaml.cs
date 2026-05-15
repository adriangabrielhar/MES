using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MainApplication.Models; // Asigură-te că namespace-ul este corect

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
            string username = txtNewUsername.Text.Trim().ToLower();
            string password = txtNewPassword.Password;
            string role = (cmbRole.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Toate câmpurile sunt obligatorii!", "Eroare Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new MESDbContext())
                {
                    // 1. Verificăm dacă utilizatorul există deja
                    if (context.Users.Any(u => u.Username == username))
                    {
                        MessageBox.Show("Acest utilizator există deja în sistem!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // 2. Criptăm parola
                    string hashedPassword = ComputeSha256Hash(password);

                    // 3. Creăm și salvăm noul obiect User
                    var newUser = new User
                    {
                        Username = username,
                        PasswordHash = hashedPassword,
                        Role = role
                    };

                    context.Users.Add(newUser);
                    await context.SaveChangesAsync();

                    MessageBox.Show($"Utilizatorul '{username}' a fost înregistrat cu succes ca '{role}'!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la conectarea cu baza de date: {ex.Message}", "Eroare Server", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Funcția de criptare (identică cu cea de la Login)
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}