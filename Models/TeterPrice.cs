using System;
using System.Collections.Generic;
using NodaTime;

namespace GoldStore.Models;

public partial class TeterPrice
{
    public long Id { get; set; }

    public decimal Value { get; set; }

    public decimal OrginalValue { get; set; }

    public long Timestamp { get; set; }

    public DateTime RegDate { get; set; }

    public string? Other { get; set; }
}
