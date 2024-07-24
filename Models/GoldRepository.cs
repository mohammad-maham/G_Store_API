namespace GoldStore.Models;

public partial class GoldRepository
{
    public long Id { get; set; }

    public short Status { get; set; }

    public short GoldType { get; set; }

    public int Weight { get; set; }

    public short Carat { get; set; }

    public long RegUserId { get; set; }

    public DateTime RegDate { get; set; }

    public short EntityType { get; set; }

    public string? CaratologyInfo { get; set; }
}
