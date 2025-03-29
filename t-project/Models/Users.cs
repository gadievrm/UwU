using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace t_project.Models
{
    public class User : INotifyPropertyChanged
    {
        private int _role;
        private string _login;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Логин обязателен")]
        [Column("login")]
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged(nameof(Login));
            }
        }

        [Required(ErrorMessage = "Пароль обязателен")]
        [Column("password")]
        public string Password { get; set; }

        [Column("role")]
        public int Role
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged(nameof(Role));
                OnPropertyChanged(nameof(RoleString));
            }
        }

        [NotMapped]
        public string RoleString => Role switch
        {
            0 => "Администратор",
            1 => "Преподаватель",
            2 => "Сотрудник",
            _ => "Неизвестно"
        };

        [Column("e_mail")]
        public string E_mail { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [Column("name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна")]
        [Column("l_name")]
        public string L_name { get; set; }

        [Column("s_name")]
        public string S_name { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("address")]
        public string Address { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}