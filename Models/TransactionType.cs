using System;
using System.Collections.Generic;

namespace GoldStore.Models;

public partial class TransactionType
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
