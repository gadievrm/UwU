using System.ComponentModel.DataAnnotations.Schema;

namespace t_project.Models
{
    [Table("ModelType")]
    public class ModelType
    {
        public int Id { get; set; }
        public string NameModel { get; set; }
        public int IdType { get; set; }
    }
}