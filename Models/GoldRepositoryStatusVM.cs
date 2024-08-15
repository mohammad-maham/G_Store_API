namespace GoldStore.Models
{
    public class GoldRepositoryStatusVM
    {
        public List<GoldRepositoryVM>? GoldRepositoryVM { get; set; }
        public double TotalWeight { get; set; } = 0.0;
    }

    public class GoldRepositoryVM
    {
        public double Weight { get; set; }
        public int GoldType { get; set; }
        public string? LastUpdateUser { get; set; }
        public string? LastUpdatePersianDate { get; set; }
        public long LastUpdateUserId { get; set; }
        public DateTime? LastUpdateGregDate { get; set; }
        public int Carat { get; set; }
        public string? CaratologyInfo { get; set; }
        public short GoldMaintenanceType { get; set; }
    }
}
