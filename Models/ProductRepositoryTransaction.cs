using System;
using System.Collections.Generic;
using NodaTime;

namespace GoldStore.Models;

public partial class ProductRepositoryTransaction
{
    public long Id { get; set; }

    public short Status { get; set; }

    public DateTime RegDate { get; set; }

    public long UserId { get; set; }

    public long ProductRepositoryId { get; set; }

    public long BasketId { get; set; }

    public short TransactionType { get; set; }

    public decimal TransactionCurrentPrice { get; set; }

    public string? DeliveryInfo { get; set; }
}
