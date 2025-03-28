using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using t_project.DB.Context;
using t_project.Models;
using t_project.Views.Windows;

namespace t_project.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для TypePage.xaml
    /// </summary>
    public partial class TypePage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<EquipmentType> _typeList;
        private ICollectionView _typeView; // Представление коллекции с фильтрацией
        private readonly TypeContext _db;

        public ObservableCollection<EquipmentType> TypesItems
        {
            get => _typeList;
            set
            {
                _typeList = value;
                OnPropertyChanged(nameof(TypesItems));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public TypePage()
        {
            InitializeComponent();
            _db = new TypeContext();
            Loaded += TypePage_Loaded;
            DataContext = this;
        }

        private async void TypePage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTypesAsync();
        }

        private async Task LoadTypesAsync()
        {
            var items = await _db.Type.AsNoTracking().ToListAsync();
            TypesItems = new ObservableCollection<EquipmentType>(items);
            _typeView = CollectionViewSource.GetDefaultView(TypesItems);
            _typeView.Filter = TypeFilter;
            TypesGrid.ItemsSource = _typeView;
        }

        private bool TypeFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
                return true;

            if (item is EquipmentType type)
            {
                string searchQuery = SearchTextBox.Text.ToLower();
                return (type.NameType?.ToLower().Contains(searchQuery) == true ||
                        type.Id.ToString().ToLower().Contains(searchQuery) == true);
            }

            return false;
        }

        private async void AddNewRowButton_Click(object sender, RoutedEventArgs e)
        {
            var newType = new EquipmentType
            {
                NameType = "Персональный компьютер"
            };

            try
            {
                _db.Type.Add(newType);
                await _db.SaveChangesAsync();
                await LoadTypesAsync(); // Перезагружаем данные после добавления
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _typeView?.Refresh();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchPlaceholder.Visibility = Visibility.Hidden;
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
            {
                SearchPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private async void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (TypesGrid.SelectedItem is EquipmentType selectedType)
            {
                // Создаем копию объекта для редактирования в окне
                var typeToEdit = new EquipmentType
                {
                    Id = selectedType.Id,
                    NameType = selectedType.NameType
                };

                EditTypeWindow editWindow = new EditTypeWindow(typeToEdit);
                if (editWindow.ShowDialog() == true)
                {
                    try
                    {
                        // Находим сущность в базе данных
                        var entity = await _db.Type.FindAsync(selectedType.Id);

                        if (entity != null)
                        {
                            // Обновляем только изменяемые поля
                            entity.NameType = typeToEdit.NameType;

                            await _db.SaveChangesAsync();
                            await LoadTypesAsync(); // Обновляем список
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (TypesGrid.SelectedItem is EquipmentType selectedType)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить тип '{selectedType.NameType}'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Вариант 1: Загружаем сущность через контекст
                        var entity = await _db.Type.FindAsync(selectedType.Id);
                        if (entity != null)
                        {
                            _db.Type.Remove(entity);
                            await _db.SaveChangesAsync();
                            await LoadTypesAsync();
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}