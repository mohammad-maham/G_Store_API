using System;
using System.Collections.Generic;
using NodaTime;

namespace GoldStore.Models;

public partial class GoldRepositoryTransaction
{
    public long Id { get; set; }

    public long GoldRepositoryId { get; set; }

    public short Status { get; set; }

    public short TransactionType { get; set; }

    public decimal LastGoldValue { get; set; }

    public decimal NewGoldValue { get; set; }

    public long RegUserId { get; set; }

    public OffsetTime RegDate { get; set; }

    public decimal Weight { get; set; }

    public short TransactionMode { get; set; }

    public string WalletInfo { get; set; } = null!;
}
