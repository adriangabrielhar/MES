using System;
using System.Windows;
using MainApplication.Models;

namespace MainApplication.Views
{
    public partial class AddLineWindow : Window
    {
        public AddLineWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string lineName = txtLineName.Text.Trim();
            string lineType = (cmbLineType.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(lineName))
            {
                MessageBox.Show("Vă rugăm să introduceți un nume pentru linie.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new MESDbContext())
                {
                    // Verifică dacă linia cu acest nume există deja
                    var existingStation = context.Workstations.FirstOrDefault(w => w.WorkstationName == lineName);
                    if (existingStation != null)
                    {
                        MessageBox.Show($"O linie cu numele '{lineName}' există deja!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Creează o nouă stație
                    var newStation = new Workstation
                    {
                        WorkstationName = lineName,
                        IsOnline = true,
                        LineType = lineType,
                        CurrentOrderId = null
                    };

                    context.Workstations.Add(newStation);
                    context.SaveChanges();

                    MessageBox.Show($"Linia '{lineName}' ({lineType}) a fost creată cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la crearea liniei: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
