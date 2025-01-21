using System;
using System.Collections.Generic;

namespace GoldStore.Models;

public partial class GoldType
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public short Staus { get; set; }
}
