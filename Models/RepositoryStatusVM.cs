namespace GoldStore.Models
{
    public class RepositoryStatusVM
    {
        public List<RepositoryVM>? RepositoryVM { get; set; }
        public decimal TotalWeight { get; set; } = 0.0M;
    }

    public class RepositoryVM
    {
        public decimal Weight { get; set; }
        public int EntityTypeId { get; set; }
        public string? LastUpdateUser { get; set; }
        public string? LastUpdatePersianDate { get; set; }
        public long LastUpdateUserId { get; set; }
        public DateTime? LastUpdateGregDate { get; set; }
        public int MaintenanceTypeId { get; set; }
    }
}
