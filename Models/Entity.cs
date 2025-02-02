using System;
using System.Collections.Generic;

namespace GoldStore.Models;

public partial class Entity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Caption { get; set; }

    public int EntityTypeId { get; set; }

    public short Status { get; set; }

    public string Symbol { get; set; } = null!;

    public int MaterialId { get; set; }

    public int EntityMode { get; set; }

    public double Scale { get; set; }

    public int AmountCode { get; set; }
}
