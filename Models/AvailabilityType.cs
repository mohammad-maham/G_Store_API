using System;
using System.Collections.Generic;

namespace GoldStore.Models;

public partial class AvailabilityType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Title { get; set; }
}
