using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using t_project.DB.Context;
using t_project.Models;
using t_project.Views.Windows;

namespace t_project.Views
{
    public partial class MaterialsPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<Equipment> Equipments;
        private ObservableCollection<Material> _materialsList;
        private ICollectionView MaterialsView;
        private readonly MaterialContext _db;
        private readonly EquipmentContext equipmentContext;

        public ObservableCollection<Material> MaterialItems
        {
            get => _materialsList;
            set
            {
                _materialsList = value;
                OnPropertyChanged(nameof(MaterialItems));
            }
        }

        public MaterialsPage()
        {
            InitializeComponent();
            _db = new MaterialContext();
            equipmentContext = new EquipmentContext();
            _ = LoadMaterialsAsync();
            DataContext = this;
        }

        private async Task LoadMaterialsAsync()
        {
            var equipments = await equipmentContext.Equipment.AsNoTracking().ToListAsync();
            Equipments = new ObservableCollection<Equipment>(equipments);

            var items = await _db.Materials
                .Include(m => m.Equipment)
                .AsNoTracking()
                .ToListAsync();

            MaterialItems = new ObservableCollection<Material>(items);
            MaterialsView = CollectionViewSource.GetDefaultView(MaterialItems);
            MaterialsView.Filter = MaterialsFilter;
            MaterialsGrid.ItemsSource = MaterialsView;
        }

        private bool MaterialsFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
                return true;

            var material = item as Material;
            string searchQuery = SearchTextBox.Text.ToLower();

            return material != null &&
                   (material.Name?.ToLower().Contains(searchQuery) == true ||
                    material.Description?.ToLower().Contains(searchQuery) == true ||
                    material.ComeDate.ToString("dd.MM.yyyy").Contains(searchQuery) ||
                    material.ResponsibleUser?.ToLower().Contains(searchQuery) == true ||
                    material.TemporaryUser?.ToLower().Contains(searchQuery) == true ||
                    material.MaterialType?.ToLower().Contains(searchQuery) == true ||
                    material.Quantity.ToString().Contains(searchQuery));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialsGrid.SelectedItem is Material selectedMaterial)
            {
                EditMaterialWindow editWindow = new EditMaterialWindow(selectedMaterial);
                if (editWindow.ShowDialog() == true)
                {
                    MaterialsGrid.Items.Refresh();
                    SaveChangesToDatabase(selectedMaterial);
                }
            }
        }

        private void SaveChangesToDatabase(Material updatedMaterial)
        {
            if (!ValidateMaterial(updatedMaterial))
                return;

            using (var db = new MaterialContext())
            {
                var existingMaterial = db.Materials.Find(updatedMaterial.Id);
                if (existingMaterial != null)
                {
                    existingMaterial.Name = updatedMaterial.Name;
                    existingMaterial.Description = updatedMaterial.Description;
                    existingMaterial.ComeDate = updatedMaterial.ComeDate;
                    existingMaterial.Image = updatedMaterial.Image;
                    existingMaterial.Quantity = updatedMaterial.Quantity;
                    existingMaterial.ResponsibleUser = updatedMaterial.ResponsibleUser;
                    existingMaterial.TemporaryUser = updatedMaterial.TemporaryUser;
                    existingMaterial.MaterialType = updatedMaterial.MaterialType;
                    existingMaterial.EquipmentId = updatedMaterial.EquipmentId;

                    db.SaveChanges();
                }
            }
        }

        private bool ValidateMaterial(Material material)
        {
            if (string.IsNullOrWhiteSpace(material.Name))
            {
                MessageBox.Show("Поле 'Наименование' обязательно для заполнения!",
                                "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (material.Quantity <= 0)
            {
                MessageBox.Show("Количество должно быть больше нуля!",
                                "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialsGrid.SelectedItem is Material selectedMaterial)
            {
                var result = MessageBox.Show($"Удалить материал: {selectedMaterial.Name}?",
                                              "Подтверждение удаления",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    MaterialItems.Remove(selectedMaterial);

                    using (var db = new MaterialContext())
                    {
                        var materialToDelete = await db.Materials.FindAsync(selectedMaterial.Id);
                        if (materialToDelete != null)
                        {
                            db.Materials.Remove(materialToDelete);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        private void AddNewRowButton_Click(object sender, RoutedEventArgs e)
        {
            var newMaterial = new Material
            {
                Name = "Новый материал",
                Description = "Описание",
                ComeDate = DateTime.Now,
                Quantity = 1,
                ResponsibleUser = "Ответственный",
                MaterialType = "Тип"
            };

            if (!ValidateMaterial(newMaterial))
                return;

            MaterialItems.Add(newMaterial);

            using (var db = new MaterialContext())
            {
                db.Materials.Add(newMaterial);
                db.SaveChanges();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MaterialsView.Refresh();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
            {
                SearchPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
            {
                SearchPlaceholder.Visibility = Visibility.Visible;
            }
        }
    }
}