using System;
using System.Collections.Generic;

namespace GoldStore.Models;

public partial class ProductType
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public short Unit { get; set; }

    public short Status { get; set; }

    public string? ProductTypeDefaultInfo { get; set; }
}
