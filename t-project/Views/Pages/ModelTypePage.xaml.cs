using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using t_project.DB.Context;
using t_project.Models;
using t_project.Views.Windows;

namespace t_project.Views.Pages
{
    public partial class ModelTypePage : Page, INotifyPropertyChanged
    {
        private readonly ModelTypeContext _context;
        private ObservableCollection<ModelType> _models;
        private ICollectionView _modelsView;

        public ObservableCollection<ModelType> ModelsItems
        {
            get => _models;
            set
            {
                _models = value;
                OnPropertyChanged();
            }
        }

        public ModelTypePage()
        {
            InitializeComponent();
            _context = new ModelTypeContext();
            Loaded += (s, e) => LoadModels();
            DataContext = this;
        }

        private void LoadModels()
        {
            _context.ModelTypes
                .Include(m => m.Type)
                .Load();

            ModelsItems = _context.ModelTypes.Local.ToObservableCollection();
            _modelsView = CollectionViewSource.GetDefaultView(ModelsItems);
            _modelsView.Filter = ModelFilter;
        }

        private bool ModelFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
                return true;

            var model = item as ModelType;
            var searchText = SearchTextBox.Text.ToLower();

            return model.Name.ToLower().Contains(searchText) ||
                   model.Type?.NameType.ToLower().Contains(searchText) == true ||
                   model.Id.ToString().Contains(searchText);
        }

        private void AddNewRowButton_Click(object sender, RoutedEventArgs e)
        {
            var newModel = new ModelType { Name = "Новая модель" };
            var editWindow = new EditModelTypeWindow(
                model: newModel,
                equipmentTypes: _context.EquipmentTypes.Local.ToList()
            );

            if (editWindow.ShowDialog() == true)
            {
                _context.ModelTypes.Add(newModel);
                _context.SaveChanges();
                LoadModels();
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (ModelsGrid.SelectedItem is ModelType selectedModel)
            {
                var editWindow = new EditModelTypeWindow(
                    model: selectedModel,
                    equipmentTypes: _context.EquipmentTypes.Local.ToList()
                );

                if (editWindow.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    LoadModels();
                }
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (ModelsGrid.SelectedItem is ModelType selectedModel)
            {
                if (_context.Equipments.Any(e => e.ModelTypeId == selectedModel.Id))
                {
                    MessageBox.Show(
                        "Невозможно удалить: модель используется в оборудовании",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                _context.ModelTypes.Remove(selectedModel);
                _context.SaveChanges();
                LoadModels();
            }
        }

        // Обработчики поиска (остаются без изменений)
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _modelsView?.Refresh();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchPlaceholder.Visibility = Visibility.Hidden;
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchPlaceholder.Visibility = Visibility.Visible;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}