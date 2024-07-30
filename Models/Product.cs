namespace GoldStore.Models;

public partial class Product
{
    public int Id { get; set; }

    public short ProductTypeId { get; set; }

    public string Name { get; set; } = null!;

    public short Status { get; set; }

    public DateTime RegDate { get; set; }

    public int DefaultWeight { get; set; }

    public string ProductInfo { get; set; } = null!;

    public short? MaterialId { get; set; }
}
