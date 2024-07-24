namespace GoldStore.Models;

public partial class GoldRepositoryTransaction
{
    public long Id { get; set; }

    public long GoldRepositoryId { get; set; }

    public short Status { get; set; }

    public short TransactionType { get; set; }

    public decimal LastGoldValue { get; set; }

    public decimal NowGoldValue { get; set; }

    public long UserId { get; set; }

    public DateTime RegDate { get; set; }

    public decimal Wight { get; set; }

    public short TransactionMode { get; set; }

    public string WalletInfo { get; set; } = null!;
}
