using System;
using System.Collections.Generic;

namespace GoldStore.Models;

public partial class EntityType
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public string Caption { get; set; } = null!;

    public short Status { get; set; }
}
