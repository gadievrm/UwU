using System;
using System.Windows;
using t_project.Models;

namespace t_project.Views.Windows
{
    public partial class EditInventoryWindow : Window
    {
        public Inventory Inventory { get; private set; }

        public EditInventoryWindow(Inventory inventory)
        {
            InitializeComponent();
            Inventory = inventory;
            NameTextBox.Text = inventory.InventName;
            StartDatePicker.SelectedDate = inventory.DateStart;
            EndDatePicker.SelectedDate = inventory.DateEnd;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Наименование не может быть пустым!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Inventory.InventName = NameTextBox.Text;
            Inventory.DateStart = StartDatePicker.SelectedDate ?? DateTime.Now;
            Inventory.DateEnd = EndDatePicker.SelectedDate ?? DateTime.Now;

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