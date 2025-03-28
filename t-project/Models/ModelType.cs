namespace t_project.Models
{
    public class ModelType
    {
        public int Id { get; set; }
        public string Name { get; set; }  // Наименование модели (обязательное)
        public int TypeId { get; set; }   // Код типа оборудования
        public EquipmentType Type { get; set; } // Навигационное свойство
    }
}