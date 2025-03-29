using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using t_project.DB.Context;
using t_project.Models;

namespace t_project.Views.Pages
{
    public partial class ProgrammersPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<Programmer> _programmersList;
        private ICollectionView ProgrammersView;
        private readonly ProgrammerContext _db;

        public ObservableCollection<Programmer> ProgrammersItems
        {
            get => _programmersList;
            set
            {
                _programmersList = value;
                OnPropertyChanged(nameof(ProgrammersItems));
            }
        }

        public ProgrammersPage()
        {
            InitializeComponent();
            _db = new ProgrammerContext();
            _ = LoadProgrammersAsync();
            DataContext = this;
        }

        private async Task LoadProgrammersAsync()
        {
            var items = await _db.Programmers.AsNoTracking().ToListAsync();
            ProgrammersItems = new ObservableCollection<Programmer>(items);
            ProgrammersView = CollectionViewSource.GetDefaultView(ProgrammersItems);
            ProgrammersView.Filter = ProgrammersFilter;
            ProgrammersGrid.ItemsSource = ProgrammersView;
        }

        private bool ProgrammersFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
                return true;

            if (item is Programmer programmer)
            {
                string searchQuery = SearchTextBox.Text.ToLower();
                return programmer.Name?.ToLower().Contains(searchQuery) == true;
            }
            return false;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProgrammersView?.Refresh();
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
            var newProgrammer = new Programmer
            {
                Name = "Новый программист"
            };
            ProgrammersItems.Add(newProgrammer);

            using (var db = new ProgrammerContext())
            {
                db.Programmers.Add(newProgrammer);
                db.SaveChanges();
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProgrammersGrid.SelectedItem != null)
            {
                ProgrammersGrid.CurrentCell = new DataGridCellInfo(
                    ProgrammersGrid.SelectedItem,
                    ProgrammersGrid.Columns[1]);
                ProgrammersGrid.BeginEdit();
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProgrammersGrid.SelectedItem is Programmer selectedProgrammer)
            {
                var result = MessageBox.Show(
                    $"Удалить программиста: {selectedProgrammer.Name}?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new ProgrammerContext())
                    {
                        db.Programmers.Remove(selectedProgrammer);
                        await db.SaveChangesAsync();
                        ProgrammersItems.Remove(selectedProgrammer);
                    }
                }
            }
        }

        private void ProgrammersGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is Programmer updatedProgrammer)
            {
                using (var db = new ProgrammerContext())
                {
                    db.Entry(updatedProgrammer).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}