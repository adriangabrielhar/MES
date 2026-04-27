using System.Security.Cryptography;
using System.Text;
using System.Windows;
using MainApplication.Views;
using MainApplication.Models; // Necesar pentru MESDbContext și clasa User
using MainApplication.BLL.Services; // Necesar pentru AuthService

namespace MainApplication
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Te rog să introduci utilizatorul și parola.", "Avertisment", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string username = txtUsername.Text.Trim().ToLower();
            string password = txtPassword.Password;

            // 1. Transformăm parola introdusă în Hash
            string hashedPassword = ComputeSha256Hash(password);

            // 2. Inițializăm Contextul bazei de date și Serviciul de Autentificare
            // Folosim "using" pentru a închide automat conexiunea la baza de date după verificare
            using (MESDbContext context = new MESDbContext())
            {
                AuthService authService = new AuthService(context);

                // 3. Verificăm credențialele apelând serviciul tău
                // Folosim 'await' și 'AuthenticateAsync'
                User authenticatedUser = await authService.AuthenticateAsync(username, hashedPassword);

                // 4. Dacă funcția a returnat un utilizator, login-ul are succes
                if (authenticatedUser != null)
                {
                    if (authenticatedUser.Role == "Admin")
                    {
                        DashboardWindow adminDash = new DashboardWindow();
                        adminDash.Show();
                        this.Close();
                    }
                    else if (authenticatedUser.Role == "Operator")
                    {
                        DashboardOperator opDash = new DashboardOperator();
                        opDash.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Rol necunoscut configurat pentru acest cont!", "Eroare Rol", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Dacă a returnat null, parola sau userul sunt greșite
                    MessageBox.Show("Date de autentificare incorecte!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Păstrăm funcția de criptare
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

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}