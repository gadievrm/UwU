using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using t_project.DB.Context;
using t_project.Models;
using t_project.Views.Windows;

namespace t_project.Views.Pages
{
    public partial class UsersPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<User> UsersItems { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<RoleItem> Roles { get; } = new ObservableCollection<RoleItem>
        {
            new RoleItem { Id = 0, Name = "Администратор" },
            new RoleItem { Id = 1, Name = "Преподаватель" },
            new RoleItem { Id = 2, Name = "Сотрудник" }
        };

        private ICollectionView _usersView;
        private readonly UserContext _db = new UserContext();

        public UsersPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += async (s, e) => await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            UsersItems.Clear();
            var users = await _db.Users.AsNoTracking().ToListAsync();
            users.ForEach(UsersItems.Add);
            _usersView = CollectionViewSource.GetDefaultView(UsersItems);
            _usersView.Filter = UserFilter;
        }

        private bool UserFilter(object obj)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text)) return true;
            return obj is User user &&
                (user.Login.Contains(SearchTextBox.Text) ||
                 user.L_name.Contains(SearchTextBox.Text));
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
            _usersView.Refresh();

        private void AddNewRowButton_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new User
            {
                Login = "Новый пользователь",
                Password = "Пароль123",
                L_name = "Фамилия",
                Name = "Имя",
                Role = 2
            };

            var editWindow = new EditUserWindow(newUser);
            if (editWindow.ShowDialog() == true)
            {
                UsersItems.Add(editWindow.User);
                using var db = new UserContext();
                db.Users.Add(editWindow.User);
                db.SaveChanges();
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser)
            {
                var editWindow = new EditUserWindow(selectedUser);
                if (editWindow.ShowDialog() == true)
                {
                    using var db = new UserContext();
                    db.Entry(selectedUser).State = EntityState.Modified;
                    db.SaveChanges();
                    UsersGrid.Items.Refresh();
                }
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User user)
            {
                using (var db = new InventoryContext())
                {
                    if (await db.Inventory.AnyAsync(i => i.UserId == user.Id))
                    {
                        MessageBox.Show("Нельзя удалить: пользователь связан с инвентаризациями");
                        return;
                    }
                }

                if (MessageBox.Show("Удалить пользователя?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var db = new UserContext();
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    UsersItems.Remove(user);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class RoleItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}