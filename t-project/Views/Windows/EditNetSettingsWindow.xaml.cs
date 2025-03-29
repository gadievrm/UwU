using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

using t_project.Models;

namespace t_project.Views.Windows
{
    public partial class EditNetSettingsWindow : Window
    {
        public NetSettings Settings { get; private set; }

        public EditNetSettingsWindow(NetSettings settings)
        {
            InitializeComponent();
            Settings = settings;
            DataContext = Settings;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                DialogResult = true;
                Close();
            }
        }

        private bool ValidateInputs()
        {
            var ipRegex = new Regex(@"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$");

            if (!ipRegex.IsMatch(Settings.IpAddress))
            {
                MessageBox.Show("Некорректный IP-адрес!");
                return false;
            }

            if (Settings.Mask < 0 || Settings.Mask > 32)
            {
                MessageBox.Show("Маска должна быть от 0 до 32!");
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void IpValidation_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9\.]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void NumberValidation_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }
    }
}