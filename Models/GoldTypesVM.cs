namespace GoldStore.Models
{
    public class GoldTypesVM
    {
        public List<GoldType>? GoldTypes { get; set; }
        public List<GoldCarat>? GoldCarats { get; set; }
    }

    public class GoldCarat
    {
        public int Id { get; set; } = 1;
        public string? Name { get; set; } = "750";
        public int Value { get; set; } = 750;
    }
}
