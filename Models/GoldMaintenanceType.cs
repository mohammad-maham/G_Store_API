using System;
using System.Collections.Generic;

namespace GoldStore.Models;

public partial class GoldMaintenanceType
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public short Status { get; set; }
}
