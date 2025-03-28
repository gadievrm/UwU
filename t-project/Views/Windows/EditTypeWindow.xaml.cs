using System.Windows;
using t_project.Models;

namespace t_project.Views.Windows
{
    public partial class EditTypeWindow : Window
    {
        public EquipmentType EquipmentType { get; private set; }

        public EditTypeWindow(EquipmentType equipmentType)
        {
            InitializeComponent();
            EquipmentType = equipmentType;

            // Заполняем поля данными
            IdTextBlock.Text = equipmentType.Id.ToString();
            NameTextBox.Text = equipmentType.NameType;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Наименование типа не может быть пустым!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Обновляем данные
            EquipmentType.NameType = NameTextBox.Text;

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