using System;
using System.Collections.Generic;

namespace GoldStore.Models;

public partial class Entity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Caption { get; set; }

    public short EntityTypeId { get; set; }

    public short Status { get; set; }

    public string Symbol { get; set; } = null!;

    public short MaterialId { get; set; }

    public short EntityMode { get; set; }
}
