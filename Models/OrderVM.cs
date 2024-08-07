using System.ComponentModel.DataAnnotations;

namespace GoldStore.Models
{
    public class OrderVM
    {
        [Display(Name = "وزن(گرم)"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Weight { get; set; }
        [Display(Name = "شناسه کاربری"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long UserId { get; set; }
        [Display(Name = "شناسه کیف پول"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long WalleId { get; set; }
        [Display(Name = "آدرس مبدأ")]
        public string? SourceAddress { get; set; }
        [Display(Name = "آدرس مقصد")]
        public int? SourceWalletCurrency { get; set; }
        public int? DestinationWalletCurrency { get; set; }
        public string? DestinationAddress { get; set; }
        [Display(Name = "مقدار درخواستی"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public double SourceAmount { get; set; }
        public double DestinationAmount { get; set; }
        [Display(Name = "قیمت لحظه ای"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public double CurrentCalculatedPrice { get; set; }
        [Display(Name = "عیار")]
        public long Carat { get; set; }
        public int GoldType { get; set; }
    }

    public class WalletTransactionVM
    {
        public string? SourceAddress { get; set; } = "1";
        public string? DestinationAddress { get; set; } = "2";
        public int? SourceWalletCurrency { get; set; } = 1; // Currency
        public int? DestinationWalletCurrency { get; set; } = 2; // Gold
        public double SourceAmount { get; set; }
        public DateTime? ExChangeData { get; set; } = DateTime.Now;
        public long RegUserId { get; set; }
        public long? Status { get; set; } = 0;
        public double DestinationAmout { get; set; }
        public long WalletId { get; set; }
    }

}
