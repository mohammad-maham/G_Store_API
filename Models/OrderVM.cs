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
        public string SourceAddress { get; set; } = null!;
        [Display(Name = "آدرس مقصد"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string DestinationAddress { get; set; } = null!;
        public decimal SourceAmount { get; set; }
        public decimal DestinationAmount { get; set; }
        public string? XchangeData { get; set; } = null!;
    }
}
