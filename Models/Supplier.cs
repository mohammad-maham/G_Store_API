namespace GoldStore.Models;

public partial class Supplier
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public short Status { get; set; }

    public long RegUserId { get; set; }

    public DateTime RegDate { get; set; }

    public string SupplierInfo { get; set; } = null!;

    public string? Description { get; set; }
}
