namespace GoldStore.Models;

public partial class AmountThreshold
{
    public long Id { get; set; }

    public short Status { get; set; }

    public long RegUserId { get; set; }

    public double BuyThreshold { get; set; }

    public double SelThreshold { get; set; }

    public double CurrentPrice { get; set; }

    public DateTime RegDate { get; set; }

    public DateTime ExpireEffectDate { get; set; }

    public int IsOnlinePrice { get; set; }
}
