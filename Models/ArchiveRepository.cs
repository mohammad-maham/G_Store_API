using System;
using System.Collections.Generic;
using NodaTime;

namespace GoldStore.Models;

public partial class ArchiveRepository
{
    public long Id { get; set; }

    public short Status { get; set; }

    public int Type { get; set; }

    public decimal Weight { get; set; }

    public int Carat { get; set; }

    public long RegUserId { get; set; }

    public int EntityType { get; set; }

    public string? CaratologyInfo { get; set; }

    public DateTime RegDate { get; set; }

    public DateTime ArchiveDate { get; set; }

    public string ArchiveOperation { get; set; } = null!;

    public int MaintenanceType { get; set; }

    public long TransactionId { get; set; }

    public long ArchiveId { get; set; }
}
