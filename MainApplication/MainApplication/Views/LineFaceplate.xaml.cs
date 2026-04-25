using System;
using System.Windows;
using System.Windows.Media;

namespace MainApplication.Views
{
    public partial class LineFaceplate : Window
    {
        private ProductionLine _line;

        public LineFaceplate(ProductionLine line)
        {
            InitializeComponent();
            _line = line;
            DataContext = _line;
            UpdateUI();
        }

        private void UpdateUI()
        {
            txtLineName.Text = _line.LineName;
            txtProduct.Text = $"Product: {_line.CurrentProduct}";
            txtStatus.Text = _line.StatusText;
            brdStatusIndicator.Background = _line.IsOccupied ? Brushes.Red : Brushes.LimeGreen;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // ==========================================================
            // [PLC / HARDWARE INTEGRATION]
            // SEND CMD: START_MOTOR_FOR_{_line.LineName}
            // ==========================================================
            _line.IsOccupied = true;
            _line.StatusText = "RUNNING";
            UpdateUI();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            // ==========================================================
            // [PLC / HARDWARE INTEGRATION]
            // SEND CMD: STOP_MOTOR_FOR_{_line.LineName}
            // ==========================================================
            _line.IsOccupied = false;
            _line.StatusText = "STOPPED";
            UpdateUI();
        }

        private void btnEStop_Click(object sender, RoutedEventArgs e)
        {
            // ==========================================================
            // [PLC / HARDWARE INTEGRATION]
            // SEND CMD: EMERGENCY_STOP_ALL_DRIVES
            // ==========================================================
            _line.IsOccupied = true;
            _line.StatusText = "EMERGENCY";
            UpdateUI();
            MessageBox.Show("EMERGENCY STOP ACTIVATED!", "PLC ALARM", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnMaintenance_Click(object sender, RoutedEventArgs e)
        {
            // ==========================================================
            // [BACK-END INTEGRATION]
            // INSERT INTO MAINTENANCE_TICKETS (LINE_ID, DATE) VALUES (...)
            // ==========================================================
            MessageBox.Show("Cerere de mentenanta inregistrata.");
        }
    }
}