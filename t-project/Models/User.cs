using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace t_project.Models
{
    public enum UserRole
    {
        Employee,
        Teacher,
        Admin
    };

    [Table("users")]
    public class User
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("login")]
        public string Login { get; set; }
        [Column("password")]
        public string Password { get; set; }

        [Column("role")]
        public int Role;

        [Column("e_mail")]
        public string Email { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("l_name")]
        public string LastName { get; set; }

        [Column("s_name")]
        public string Surname { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("address")]
        public string Address { get; set; }
    }
}