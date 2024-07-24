namespace GoldStore.Models;

public partial class ProductRepository
{
    public long Id { get; set; }

    public int ProductId { get; set; }

    public string ProductCode { get; set; } = null!;

    public short Status { get; set; }

    public int Weight { get; set; }

    public string ProductCustomInfo { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime RegDate { get; set; }
}
