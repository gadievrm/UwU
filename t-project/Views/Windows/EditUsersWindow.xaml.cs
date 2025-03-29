using System;
using System.Collections.Generic;
using System.Windows;
using t_project.Models;

namespace t_project.Views.Windows
{
    public partial class EditUserWindow : Window
    {
        public User User { get; private set; }
        public string WindowTitle => User.Id == 0 ? "Добавление пользователя" : "Редактирование пользователя";

        private Dictionary<int, string> _roles = new Dictionary<int, string>
        {
            {0, "Администратор"},
            {1, "Преподаватель"},
            {2, "Сотрудник"}
        };

        public EditUserWindow(User user = null)
        {
            InitializeComponent();
            User = user ?? new User();
            DataContext = this;
            InitializeFields();
        }

        private void InitializeFields()
        {
            // Заполнение полей
            LoginTextBox.Text = User.Login;
            PasswordBox.Password = User.Password;
            RoleComboBox.ItemsSource = _roles;
            RoleComboBox.SelectedValue = User.Role;
            LastNameTextBox.Text = User.L_name;
            FirstNameTextBox.Text = User.Name;
            MiddleNameTextBox.Text = User.S_name;
            EmailTextBox.Text = User.E_mail;
            PhoneTextBox.Text = User.Phone;
            AddressTextBox.Text = User.Address;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация обязательных полей
                if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
                    throw new Exception("Логин обязателен для заполнения!");

                if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                    throw new Exception("Пароль обязателен для заполнения!");

                if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
                    throw new Exception("Фамилия обязательна для заполнения!");

                // Обновление модели
                User.Login = LoginTextBox.Text.Trim();
                User.Password = PasswordBox.Password;
                User.Role = (int)(RoleComboBox.SelectedValue ?? 2);
                User.L_name = LastNameTextBox.Text.Trim();
                User.Name = FirstNameTextBox.Text.Trim();
                User.S_name = MiddleNameTextBox.Text.Trim();
                User.E_mail = EmailTextBox.Text.Trim();
                User.Phone = PhoneTextBox.Text.Trim();
                User.Address = AddressTextBox.Text.Trim();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}