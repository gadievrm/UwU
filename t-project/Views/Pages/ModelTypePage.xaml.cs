using Microsoft.EntityFrameworkCore;
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
    public partial class ModelTypePage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<ModelType> _modelTypeList;
        private ICollectionView _modelTypeView;
        private readonly ModelTypeContext _db;

        public ObservableCollection<ModelType> ModelTypesItems
        {
            get => _modelTypeList;
            set { _modelTypeList = value; OnPropertyChanged(nameof(ModelTypesItems)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ModelTypePage()
        {
            InitializeComponent();
            _db = new ModelTypeContext();
            Loaded += ModelTypePage_Loaded;
        }

        private async void ModelTypePage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadModelTypesAsync();
        }

        private async Task LoadModelTypesAsync()
        {
            var items = await _db.ModelTypes.ToListAsync();
            ModelTypesItems = new ObservableCollection<ModelType>(items);
            _modelTypeView = CollectionViewSource.GetDefaultView(ModelTypesItems);
            _modelTypeView.Filter = ModelTypeFilter;
            ModelTypesGrid.ItemsSource = _modelTypeView;
        }

        private bool ModelTypeFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
                return true;

            if (item is ModelType modelType)
            {
                string searchQuery = SearchTextBox.Text.ToLower();
                return (modelType.NameModel?.ToLower().Contains(searchQuery) == true ||
                       modelType.Id.ToString().ToLower().Contains(searchQuery) == true ||
                       modelType.IdType.ToString().ToLower().Contains(searchQuery) == true);
            }

            return false;
        }

        private void AddNewRowButton_Click(object sender, RoutedEventArgs e)
        {
            var newModelType = new ModelType
            {
                NameModel = "Новая модель",
                IdType = 1
            };
            _db.ModelTypes.Add(newModelType);
            _db.SaveChanges();
            ModelTypesItems.Add(newModelType);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {   
            _modelTypeView?.Refresh();
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

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (ModelTypesGrid.SelectedItem is ModelType selectedModelType)
            {
                var editWindow = new EditModelTypeWindow(selectedModelType);
                if (editWindow.ShowDialog() == true)
                {
                    _db.SaveChanges();
                    LoadModelTypesAsync();
                }
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (ModelTypesGrid.SelectedItem is ModelType selectedModelType)
            {
                try
                {
                    _db.ModelTypes.Remove(selectedModelType);
                    await _db.SaveChangesAsync();
                    await LoadModelTypesAsync();
                }
                catch (DbUpdateException)
                {
                    MessageBox.Show("Нельзя удалить вид модели, так как она связана с другим оборудованием",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void ModelTypesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}