namespace GoldStore.Models
{
    public class RepositoryReportFilterVM
    {
        public long? Carat { get; set; }
        public Enums.EntityTypes? EntityType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? UserId { get; set; }
        public int? MaintenanceType { get; set; }
    }

    public class RepositoryReportFilterDataVM
    {
        public long TransactionId { get; set; }
        public DateTime RegDate { get; set; }
        public int EntityId { get; set; }
        public string? Entity { get; set; }
        public int MaintenanceTypeId { get; set; }
        public string? MaintenanceType { get; set; }
        public long RegUserId { get; set; }
        public decimal LastValue { get; set; }
        public decimal NewValue { get; set; }
        public int TransactionTypeId { get; set; }
        public string? TransactionType { get; set; }
        public decimal Weight { get; set; }
        public string? RegPersianDate { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
        public string? ArchiveOperation { get; set; }
    }
}
