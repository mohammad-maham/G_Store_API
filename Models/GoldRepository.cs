﻿namespace GoldStore.Models;

public partial class ArchiveGoldRepository
{
    public long Id { get; set; }

    public long ArchiveId { get; set; }

    public short Status { get; set; }

    public short GoldType { get; set; }

    public double Weight { get; set; }

    public short Carat { get; set; }

    public long RegUserId { get; set; }

    public short EntityType { get; set; }

    public string? CaratologyInfo { get; set; }

    public DateTime RegDate { get; set; }

    public short GoldMaintenanceType { get; set; }

    public long TransactionId { get; set; }

    public DateTime ArchiveDate { get; set; }

    public string? ArchiveOperation { get; set; }
}
