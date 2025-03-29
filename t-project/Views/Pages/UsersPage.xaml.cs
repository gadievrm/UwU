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
    public partial class UsersPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<User> _UsersList;
        private ICollectionView UsersView;
        private readonly UsersContext _db;

        public ObservableCollection<User> UsersItems
        {
            get => _UsersList;
            set
            {
                _UsersList = value;
                OnPropertyChanged(nameof(UsersItems));
            }
        }

        public UsersPage()
        {
            InitializeComponent();
            _db = new UsersContext();
            _ = LoadUsersAsync();
            DataContext = this;
        }

        private async Task LoadUsersAsync()
        {
            var items = await _db.Users.AsNoTracking().ToListAsync();
            UsersItems = new ObservableCollection<User>(items);
            UsersView = CollectionViewSource.GetDefaultView(UsersItems);
            UsersView.Filter = UsersFilter;
            UsersGrid.ItemsSource = UsersView;
        }

        private bool UsersFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
                return true;

            if (item is User user)
            {
                return (user.Name + user.LastName + user.Surname).ToLower().Contains(SearchTextBox.Text.ToLower());
            }
            return false;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UsersView?.Refresh();
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
            var newUser = new User
            {
                Login = "ivanov",
                Password = "",
                Role = 0,
                Email = "ivanov@mail.ru",
                Name = "Иван",
                Surname = "Иванов",
                LastName = "Иванович",
                Phone = "",
                Address = ""
            };

            var editWindow = new EditUserWindow(newUser);
            if (editWindow.ShowDialog() == true)
            {
                UsersItems.Add(editWindow.user);
                using (var db = new UsersContext())
                {
                    db.Users.Add(editWindow.user);
                    db.SaveChanges();
                }
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser)
            {
                var editWindow = new EditUserWindow(selectedUser);
                if (editWindow.ShowDialog() == true)
                {
                    UsersGrid.Items.Refresh();
                    using (var db = new UsersContext())
                    {
                        db.ChangeTracker.Clear();
                        db.Entry(selectedUser).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser)
            {
                var result = MessageBox.Show($"Удалить пользователя {selectedUser.Surname} {selectedUser.Name} {selectedUser.LastName}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new UsersContext())
                    {
                        db.ChangeTracker.Clear();
                        db.Users.Remove(selectedUser);
                        await db.SaveChangesAsync();
                        UsersItems.Remove(selectedUser);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}