using System.ComponentModel.DataAnnotations;

namespace GoldStore.Models
{
    public partial class OrderVM
    {
        [Display(Name = "وزن(گرم)"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Weight { get; set; }
        [Display(Name = "شناسه کاربری"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long UserId { get; set; }
        [Display(Name = "شناسه کیف پول"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long WalleId { get; set; }
        [Display(Name = "آدرس مبدأ"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long? SourceAddress { get; set; } = 1; // Currency
        [Display(Name = "آدرس مقصد"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long? DestinationAddress { get; set; } = 2; // Gold
        [Display(Name = "مقدار درخواستی"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public double SourceAmount { get; set; }
        public double DestinationAmount { get; set; }
        [Display(Name = "قیمت لحظه ای"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public double CurrentCalculatedPrice { get; set; }
        [Display(Name = "عیار")]
        public long Carat { get; set; } = 750;
        public int GoldType { get; set; } = 2;
    }
}
