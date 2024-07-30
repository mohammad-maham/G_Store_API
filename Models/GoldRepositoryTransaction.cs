namespace GoldStore.Models;

public partial class GoldRepositoryTransaction
{
    public long Id { get; set; }

    public long GoldRepositoryId { get; set; }

    public short Status { get; set; }

    public short TransactionType { get; set; }

    public double LastGoldValue { get; set; }

    public double NewGoldValue { get; set; }

    public long RegUserId { get; set; }

    public double Weight { get; set; }

    public short TransactionMode { get; set; }

    public string WalletInfo { get; set; } = null!;

    public DateTime RegDate { get; set; }
}
