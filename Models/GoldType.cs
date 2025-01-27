using System;
using System.Collections.Generic;

namespace GoldStore.Models;

public partial class GoldType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public short Staus { get; set; }
}
