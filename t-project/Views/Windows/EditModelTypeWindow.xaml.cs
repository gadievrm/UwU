using System.Windows;
using t_project.Models;

namespace t_project.Views.Windows
{
    public partial class EditModelTypeWindow : Window
    {
        public ModelType ModelType { get; private set; }

        public EditModelTypeWindow(ModelType modelType)
        {
            InitializeComponent();
            ModelType = modelType;

            IdTextBlock.Text = modelType.Id.ToString();
            NameTextBox.Text = modelType.NameModel;
            TypeIdTextBox.Text = modelType.IdType.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Наименование модели не может быть пустым!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ModelType.NameModel = NameTextBox.Text;
            ModelType.IdType = int.Parse(TypeIdTextBox.Text);

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