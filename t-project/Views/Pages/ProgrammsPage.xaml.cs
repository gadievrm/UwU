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
    public partial class ProgrammsPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<Programms> _programmsList;
        private ICollectionView _programmsView;
        private readonly ProgrammsContext _db;

        public ObservableCollection<Programms> ProgrammsItems
        {
            get => _programmsList;
            set { _programmsList = value; OnPropertyChanged(nameof(ProgrammsItems)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ProgrammsPage()
        {
            InitializeComponent();
            _db = new ProgrammsContext();
            Loaded += ProgrammsPage_Loaded;
        }

        private async void ProgrammsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadProgrammsAsync();
        }

        private async Task LoadProgrammsAsync()
        {
            var items = await _db.Programms.ToListAsync();
            ProgrammsItems = new ObservableCollection<Programms>(items);
            _programmsView = CollectionViewSource.GetDefaultView(ProgrammsItems);
            _programmsView.Filter = ProgrammsFilter;
            ProgrammsGrid.ItemsSource = _programmsView;
        }

        private bool ProgrammsFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
                return true;

            if (item is Programms program)
            {
                string searchQuery = SearchTextBox.Text.ToLower();
                return (program.ProgrammName?.ToLower().Contains(searchQuery) == true ||
                       program.OSProgrammer?.ToLower().Contains(searchQuery) == true ||
                       program.VersionOS?.ToLower().Contains(searchQuery) == true ||
                       program.id.ToString().Contains(searchQuery));
            }
            return false;
        }

        private void AddNewRowButton_Click(object sender, RoutedEventArgs e)
        {
            var newProgram = new Programms
            {
                ProgrammName = "Новая программа",
                OSProgrammer = "Разработчик",
                VersionOS = "1.0"
            };
            _db.Programms.Add(newProgram);
            _db.SaveChanges();
            ProgrammsItems.Add(newProgram);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _programmsView?.Refresh();
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

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProgrammsGrid.SelectedItem is Programms selectedProgram)
            {
                var editWindow = new EditProgrammsWindow(selectedProgram);
                if (editWindow.ShowDialog() == true)
                {
                    _db.SaveChanges();
                    LoadProgrammsAsync();
                }
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProgrammsGrid.SelectedItem is Programms selectedProgram)
            {
                try
                {
                    _db.Programms.Remove(selectedProgram);
                    await _db.SaveChangesAsync();
                    await LoadProgrammsAsync();
                }
                catch (DbUpdateException)
                {
                    MessageBox.Show("Нельзя удалить программу, так как она связана с другим оборудованием",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}