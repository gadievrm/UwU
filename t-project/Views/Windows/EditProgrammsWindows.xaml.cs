using System.Windows;
using t_project.Models;

namespace t_project.Views.Windows
{
    public partial class EditProgrammsWindow : Window
    {
        public Programms Program { get; private set; }

        public EditProgrammsWindow(Programms program)
        {
            InitializeComponent();
            Program = program;

            IdTextBlock.Text = program.id.ToString();
            NameTextBox.Text = program.ProgrammName;
            DeveloperTextBox.Text = program.OSProgrammer;
            VersionTextBox.Text = program.VersionOS;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Наименование программы не может быть пустым!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Program.ProgrammName = NameTextBox.Text;
            Program.OSProgrammer = DeveloperTextBox.Text;
            Program.VersionOS = VersionTextBox.Text;

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