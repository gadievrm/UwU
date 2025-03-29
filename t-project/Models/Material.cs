using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace t_project.Models
{
    [Table("materials")]
    public class Material
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("material_name")]
        [Required]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("come_date")]
        public DateTime ComeDate { get; set; }

        [Column("picture")]
        public byte[] Image { get; set; }

        [Column("material_number")]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Column("responsible_user")]
        public string ResponsibleUser { get; set; }

        [Column("temporary_user")]
        public string TemporaryUser { get; set; }

        [Column("type")]
        public string MaterialType { get; set; }

        [ForeignKey("Equipment")]
        [Column("equipment_id")]
        public int? EquipmentId { get; set; }

        public Equipment Equipment { get; set; }
    }
}