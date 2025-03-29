using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace t_project.Models
{
    [Table("inventory")]
    public class Inventory
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("date_start")]
        public DateTime DateStart { get; set; }
        [Column("date_end")]
        public DateTime DateEnd { get; set; }
        [Column("invent_name")]
        public string InventName { get; set; }
    }
}