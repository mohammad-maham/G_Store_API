using System;
using System.Collections.Generic;

namespace GoldStore.Models;

public partial class Amount
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Status { get; set; }

    public string? Title { get; set; }
}
