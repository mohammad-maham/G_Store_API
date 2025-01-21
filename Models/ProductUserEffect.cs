using System;
using System.Collections.Generic;
using NodaTime;

namespace GoldStore.Models;

public partial class ProductUserEffect
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public short LikeStatus { get; set; }

    public long UserId { get; set; }

    public DateTime RegDate { get; set; }

    public long ParentId { get; set; }

    public string? Message { get; set; }

    public short MessageType { get; set; }

    public short Status { get; set; }

    public string? MessageDetail { get; set; }
}
