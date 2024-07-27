﻿namespace GoldStore.Models;

public partial class AmountThreshold
{
    public long Id { get; set; }

    public short Status { get; set; }

    public long RegUserId { get; set; }

    public int BuyThreshold { get; set; }

    public int SelThreshold { get; set; }

    public decimal CurrentPrice { get; set; }

    public DateTime RegDate { get; set; }

    public DateTime ExpireEffectDate { get; set; }
}
