using System;
using System.Collections.Generic;
using NodaTime;

namespace GoldStore.Models;

public partial class RepositoryTransaction
{
    public long Id { get; set; }

    public long RepositoryId { get; set; }

    public short Status { get; set; }

    public int TransactionType { get; set; }

    public decimal LastValue { get; set; }

    public decimal NewValue { get; set; }

    public long RegUserId { get; set; }

    public decimal Value { get; set; }

    public int TransactionMode { get; set; }

    public string? WalletInfo { get; set; }

    public DateTime RegDate { get; set; }

    public string? UserAdditionalData { get; set; }

    public string Description { get; set; } = null!;
}
