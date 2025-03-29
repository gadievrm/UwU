using Microsoft.EntityFrameworkCore;
using System;
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
        private ICollectionView _programmersView;
        private readonly ProgrammerContext _db;

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ProgrammersView?.Refresh();
            }
        }

        public ObservableCollection<Programmer> ProgrammersItems
        {
            get => _programmersList;
            set
            {
                _programmersList = value;
                OnPropertyChanged(nameof(ProgrammersItems));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ICollectionView ProgrammersView => _programmersView;

        public ProgrammersPage()
        {
            InitializeComponent();
            _db = new ProgrammerContext();
            _programmersList = new ObservableCollection<Programmer>();
            _programmersView = CollectionViewSource.GetDefaultView(_programmersList);
            _programmersView.Filter = FilterProgrammers;
            DataContext = this;
            _ = LoadProgrammersAsync();
        }

        private bool FilterProgrammers(object item)
        {
            if (item is Programmer programmer)
            {
                return programmer.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private async Task LoadProgrammersAsync()
        {
            var programmers = await _db.Programmers.ToListAsync();
            ProgrammersItems = new ObservableCollection<Programmer>(programmers);
            _programmersView = CollectionViewSource.GetDefaultView(ProgrammersItems);
            _programmersView.Refresh();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _programmersView.Filter = item =>
            {
                if (item is Programmer programmer)
                {
                    return programmer.Name.ToLower().Contains(SearchTextBox.Text.ToLower());
                }
                return false;
            };
            _programmersView.Refresh();
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

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProgrammersGrid.SelectedItem is Programmer selectedProgrammer)
            {
                try
                {
                    bool isProgrammerUsed = await CheckProgrammerUsage(selectedProgrammer.Name);
                    if (isProgrammerUsed)
                    {
                        MessageBox.Show("Этот программист используется в других модулях. Удаление невозможно.",
                                      "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (MessageBox.Show($"Удалить программиста: {selectedProgrammer.Name}?",
                                      "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        _db.Programmers.Remove(selectedProgrammer);
                        await _db.SaveChangesAsync();
                        ProgrammersItems.Remove(selectedProgrammer);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task<bool> CheckProgrammerUsage(string programmerName)
        {
            using (var db = new ProgrammsContext())
            {
                return await db.Programms.AnyAsync(p => p.OSProgrammer == programmerName);
            }
        }

        private async void AddNewRowButton_Click(object sender, RoutedEventArgs e)
        {
            var newProgrammer = new Programmer { Name = "Новый программист" };
            ProgrammersItems.Add(newProgrammer);
            await _db.Programmers.AddAsync(newProgrammer);
            await _db.SaveChangesAsync();
        }

        private async void ProgrammersGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is Programmer editedItem)
            {
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}