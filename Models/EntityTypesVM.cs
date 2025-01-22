namespace GoldStore.Models
{
    public class EntityTypesVM
    {
        public List<Entity>? EntityTypes { get; set; }
    }

    public class GoldCarat
    {
        public int Id { get; set; } = 1;
        public string? Name { get; set; } = "750";
        public int Value { get; set; } = 750;
    }
}
