using System.ComponentModel.DataAnnotations.Schema;

namespace t_project.Models
{
    [Table("Programms")]
    public class Programms
    {
        public int id { get; set; }

        [Column("program_name")]
        public string ProgrammName { get; set; }

        [Column("os_programmer")]
        public string OSProgrammer { get; set; }

        [Column("version_os")]
        public string VersionOS { get; set; }
    }
}   