namespace GoldStore.Models
{
    public class GoldRepositoryReportFilterVM
    {
        public long? Carat { get; set; }
        public int? GoldType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? UserId { get; set; }
        public int? MaintenanceType { get; set; }
    }

    public class GoldRepositoryReportFilterDataVM
    {
        public long TransactionId { get; set; }
        public DateTime RegDate { get; set; }
        public int GoldTypeId { get; set; }
        public string? GoldType { get; set; }
        public int GoldMaintenanceTypeId { get; set; }
        public string? GoldMaintenanceType { get; set; }
        public int Carat { get; set; }
        public long RegUserId { get; set; }
        public double LastGoldValue { get; set; }
        public double NewGoldValue { get; set; }
        public int TransactionTypeId { get; set; }
        public string? TransactionType { get; set; }
        public double Weight { get; set; }
        public string? RegPersianDate { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
    }
}
