using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace t_project.Models
{
    [Table("net_settings")]
    public class NetSettings
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("ip_adress")]
        public string IpAddress { get; set; }

        [Column("mask")]
        public int Mask { get; set; }

        [Column("base_gate")]
        public string BaseGate { get; set; }

        [Column("dns_servers")]
        public string DnsServers { get; set; }

        // Явно указываем имя столбца для внешнего ключа
        [Column("id")] // Должно совпадать с именем в БД
        [ForeignKey("Equipment")]
        public int EquipmentId { get; set; }

        public Equipment Equipment { get; set; }
    }
}