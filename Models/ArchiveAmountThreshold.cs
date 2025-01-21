using System;
using System.Collections.Generic;
using NodaTime;

namespace GoldStore.Models;

public partial class ArchiveAmountThreshold
{
    public short Status { get; set; }

    public long RegUserId { get; set; }

    public int BuyThreshold { get; set; }

    public int SelThreshold { get; set; }

    public decimal? CurrentPrice { get; set; }

    public DateTime RegDate { get; set; }

    public DateTime ArchiveDate { get; set; }

    public DateTime ExpireEffectDate { get; set; }

    public long Id { get; set; }

    public int EntityId { get; set; }
}
