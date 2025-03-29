using System;
using System.Windows;
using t_project.Models;

namespace t_project.Views.Windows
{
    public partial class EditUserWindow : Window
    {
        public User user { get; private set; }

        public EditUserWindow(User user)
        {
            InitializeComponent();
            this.user = user;

            LoginTextBox.Text = user.Login;
            PasswordTextBox.Text = user.Password;
            RoleComboBox.SelectedItem = RoleComboBox.Items.GetItemAt(user.Role);
            EMailTextBox.Text = user.Email;
            SurNameTextBox.Text = user.Surname;
            NameTextBox.Text = user.Name;
            LastNameTextBox.Text = user.LastName;
            PhoneTextBox.Text = user.Phone;
            AddressTextBox.Text = user.Address;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Наименование не может быть пустым!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            user.Login = LoginTextBox.Text;
            user.Password = PasswordTextBox.Text;
            user.Role = RoleComboBox.SelectedIndex;
            user.Email = EMailTextBox.Text;
            user.Surname = SurNameTextBox.Text;
            user.Name = NameTextBox.Text;
            user.LastName = LastNameTextBox.Text;
            user.Phone = PhoneTextBox.Text;
            user.Address = AddressTextBox.Text;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}