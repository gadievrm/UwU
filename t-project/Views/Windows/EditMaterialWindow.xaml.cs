using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using t_project.DB.Context;
using t_project.Models;
using t_project.Views.Pages;

namespace t_project.Views.Windows
{
    public partial class EditMaterialWindow : Window
    {
        public Material Material { get; private set; }
        public ObservableCollection<Equipment> Equipments { get; set; }

        public EditMaterialWindow(Material material)
        {
            InitializeComponent();
            Material = material;
            DataContext = this;

            LoadEquipment();
            InitializeFields();
        }

        private void LoadEquipment()
        {
            using (var context = new EquipmentContext())
            {
                Equipments = new ObservableCollection<Equipment>(context.Equipment.ToList());
                EquipmentComboBox.ItemsSource = Equipments;

                if (Material.EquipmentId.HasValue)
                {
                    EquipmentComboBox.SelectedItem = Equipments.FirstOrDefault(e => e.Id == Material.EquipmentId);
                }
            }
        }

        private void InitializeFields()
        {
            NameTextBox.Text = Material.Name;
            DescriptionTextBox.Text = Material.Description;
            ComeDatePicker.SelectedDate = Material.ComeDate;
            QuantityTextBox.Text = Material.Quantity.ToString();
            ResponsibleTextBox.Text = Material.ResponsibleUser;
            TemporaryUserTextBox.Text = Material.TemporaryUser;
            TypeComboBox.Text = Material.MaterialType;

            if (Material.Image != null && Material.Image.Length > 0)
            {
                LoadMaterialImage();
            }
        }

        private void LoadMaterialImage()
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(Material.Image))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    MaterialImage.Source = bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки изображения: " + ex.Message);
            }
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (openFileDialog.ShowDialog() == true)
            {
                Material.Image = File.ReadAllBytes(openFileDialog.FileName);
                LoadMaterialImage();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            Material.Name = NameTextBox.Text;
            Material.Description = DescriptionTextBox.Text;
            Material.ComeDate = ComeDatePicker.SelectedDate ?? DateTime.Now;
            Material.Quantity = int.Parse(QuantityTextBox.Text);
            Material.ResponsibleUser = ResponsibleTextBox.Text;
            Material.TemporaryUser = TemporaryUserTextBox.Text;
            Material.MaterialType = TypeComboBox.Text;
            Material.EquipmentId = (EquipmentComboBox.SelectedItem as Equipment)?.Id;

            DialogResult = true;
            Close();
        }

        private bool ValidateInputs()
        {
            // Валидация даты
            if (!ComeDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Дата поступления обязательна для заполнения!");
                return false;
            }

            // Валидация количества
            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Некорректное значение количества!");
                return false;
            }

            // Валидация обязательных полей
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Наименование обязательно для заполнения!");
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void NumberValidation_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
    }
}