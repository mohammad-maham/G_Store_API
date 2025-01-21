using System;
using System.Collections.Generic;
using NodaTime;

namespace GoldStore.Models;

public partial class Product
{
    public int Id { get; set; }

    public short ProductTypeId { get; set; }

    public string Name { get; set; } = null!;

    public short Status { get; set; }

    public DateTime RegDate { get; set; }

    public decimal DefaultWeight { get; set; }

    public string? ProductInfo { get; set; }

    public short? MaterialId { get; set; }

    public string? Images { get; set; }
}
