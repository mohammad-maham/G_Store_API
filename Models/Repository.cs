using System;
using System.Collections.Generic;
using NodaTime;

namespace GoldStore.Models;

public partial class Repository
{
    public long Id { get; set; }

    public short Status { get; set; }

    public int Entity { get; set; }

    public decimal Value { get; set; }

    public long RegUserId { get; set; }

    public DateTime RegDate { get; set; }

    public long TransactionId { get; set; }

    public int MaintenanceTypeId { get; set; }

    public int AvailabilityTypeId { get; set; }

    public string AvailabilityInfo { get; set; } = null!;
}
