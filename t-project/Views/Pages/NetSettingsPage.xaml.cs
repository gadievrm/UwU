using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using t_project.DB.Context;
using t_project.Models;
using t_project.Views.Windows;

namespace t_project.Views
{
    public partial class NetSettingsPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<NetSettings> _netSettingsList;
        private readonly NetSettingsContext _db;

        public ObservableCollection<NetSettings> NetSettingsList
        {
            get => _netSettingsList;
            set
            {
                _netSettingsList = value;
                OnPropertyChanged(nameof(NetSettingsList));
            }
        }

        public NetSettingsPage()
        {
            InitializeComponent();
            _db = new NetSettingsContext();
            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            var settings = _db.NetSettings.ToList();
            NetSettingsList = new ObservableCollection<NetSettings>(settings);
        }

        private void AddNewRowButton_Click(object sender, RoutedEventArgs e)
        {
            var newSettings = new NetSettings
            {
                IpAddress = "192.168.0.1",
                Mask = 24,
                BaseGate = "192.168.0.254",
                DnsServers = "8.8.8.8;8.8.4.4"
            };

            if (ValidateSettings(newSettings))
            {
                NetSettingsList.Add(newSettings);
                _db.NetSettings.Add(newSettings);
                _db.SaveChanges();
            }
        }

        private bool ValidateSettings(NetSettings settings)
        {
            var ipRegex = new Regex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");

            if (!ipRegex.IsMatch(settings.IpAddress))
            {
                MessageBox.Show("Некорректный IP-адрес!");
                return false;
            }

            if (settings.Mask < 0 || settings.Mask > 32)
            {
                MessageBox.Show("Маска должна быть в диапазоне 0-32!");
                return false;
            }

            return true;
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (NetSettingsGrid.SelectedItem is NetSettings selected)
            {
                var editWindow = new EditNetSettingsWindow(selected);
                if (editWindow.ShowDialog() == true)
                {
                    _db.SaveChanges();
                    LoadData();
                }
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (NetSettingsGrid.SelectedItem is NetSettings selected)
            {
                _db.NetSettings.Remove(selected);
                _db.SaveChanges();
                LoadData();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}