using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace t_project.Models
{
    public class Equipment
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] ImagePath { get; set; }
        public string EquipmentNumber { get; set; }
        public string RoomNumber { get; set; }
        public string ResponsibleUser { get; set; }
        public string TemporaryUser { get; set; }
        public decimal Price { get; set; }
        public string Direction { get; set; }
        public string Status { get; set; }

        [ForeignKey("ModelType")]
        public int? ModelTypeId { get; set; }  // Делаем nullable для обратной совместимости
        public ModelType ModelType { get; set; }

        public string Model { get; set; }  // Оставляем старое поле для совместимости
        public string Comment { get; set; }
    }
}