using System;
using System.Collections.Generic;
using NodaTime;

namespace GoldStore.Models;

public partial class Product
{
    public int Id { get; set; }

    public int ProductTypeId { get; set; }

    public string Name { get; set; } = null!;

    public short Status { get; set; }

    public decimal DefaultWeight { get; set; }

    public string? ProductInfo { get; set; }

    public int? MaterialId { get; set; }

    public string? Images { get; set; }

    public decimal Taxe { get; set; }

    public decimal MakingFee { get; set; }

    public int Karat { get; set; }

    public DateTime? RegDate { get; set; }
}
