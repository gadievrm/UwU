using System.ComponentModel.DataAnnotations.Schema;

namespace t_project.Models
{
    [Table("programms")]
    public class Programmer
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("os_programmer")]
        public string Name { get; set; }
    }
}