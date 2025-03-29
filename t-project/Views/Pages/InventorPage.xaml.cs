using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using t_project.DB.Context;
using t_project.Models;
using t_project.Views.Windows;
using System.Threading.Tasks;

namespace t_project.Views.Pages
{
    public partial class InventoryPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<Inventory> _inventoryList;
        private ICollectionView InventoryView;
        private readonly InventoryContext _db;

        public ObservableCollection<Inventory> InventoryItems
        {
            get => _inventoryList;
            set
            {
                _inventoryList = value;
                OnPropertyChanged(nameof(InventoryItems));
            }
        }

        public InventoryPage()
        {
            InitializeComponent();
            _db = new InventoryContext();
            _ = LoadInventoryAsync();
            DataContext = this;
        }

        private async Task LoadInventoryAsync()
        {
            var items = await _db.Inventory.AsNoTracking().ToListAsync();
            InventoryItems = new ObservableCollection<Inventory>(items);
            InventoryView = CollectionViewSource.GetDefaultView(InventoryItems);
            InventoryView.Filter = InventoryFilter;
            InventoryGrid.ItemsSource = InventoryView;
        }

        private bool InventoryFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
                return true;

            if (item is Inventory inventory)
            {
                string searchQuery = SearchTextBox.Text.ToLower();
                return inventory.InventName?.ToLower().Contains(searchQuery) == true;
            }
            return false;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InventoryView?.Refresh();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
                SearchPlaceholder.Visibility = Visibility.Hidden;
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
                SearchPlaceholder.Visibility = Visibility.Visible;
        }

        private void AddNewRowButton_Click(object sender, RoutedEventArgs e)
        {
            var newInventory = new Inventory
            {
                InventName = "Новая инвентаризация",
                DateStart = DateTime.Now,
                DateEnd = DateTime.Now.AddDays(7)
            };

            var editWindow = new EditInventoryWindow(newInventory);
            if (editWindow.ShowDialog() == true)
            {
                InventoryItems.Add(editWindow.Inventory);
                using (var db = new InventoryContext())
                {
                    db.Inventory.Add(editWindow.Inventory);
                    db.SaveChanges();
                }
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryGrid.SelectedItem is Inventory selectedInventory)
            {
                var editWindow = new EditInventoryWindow(selectedInventory);
                if (editWindow.ShowDialog() == true)
                {
                    InventoryGrid.Items.Refresh();
                    using (var db = new InventoryContext())
                    {
                        db.Entry(selectedInventory).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryGrid.SelectedItem is Inventory selectedInventory)
            {
                // Проверка связанных записей
                using (var db = new EquipmentContext())
                {
                    if (await db.Equipment.AnyAsync(equip => equip.InventoryId == selectedInventory.Id))
                    {
                        MessageBox.Show("Невозможно удалить инвентаризацию, так как она связана с оборудованием!",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var result = MessageBox.Show($"Удалить инвентаризацию: {selectedInventory.InventName}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new InventoryContext())
                    {
                        db.Inventory.Remove(selectedInventory);
                        await db.SaveChangesAsync();
                        InventoryItems.Remove(selectedInventory);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}