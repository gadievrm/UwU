using System.Windows;
using System.Collections.Generic;
using t_project.Models;

namespace t_project.Views.Windows
{
    public partial class EditModelTypeWindow : Window
    {
        public ModelType Model { get; private set; }
        private readonly List<EquipmentType> _equipmentTypes;

        public EditModelTypeWindow(ModelType model, List<EquipmentType> equipmentTypes)
        {
            InitializeComponent();
            Model = model;
            _equipmentTypes = equipmentTypes;
            DataContext = this;

            // Инициализация ComboBox
            TypeComboBox.ItemsSource = _equipmentTypes;
            TypeComboBox.DisplayMemberPath = "NameType";
            TypeComboBox.SelectedValuePath = "Id";
            TypeComboBox.SelectedValue = model.TypeId;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Model.Name))
            {
                MessageBox.Show("Наименование модели обязательно!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (TypeComboBox.SelectedValue is int typeId)
            {
                Model.TypeId = typeId;
            }

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}   