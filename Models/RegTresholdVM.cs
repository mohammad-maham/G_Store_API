using System.ComponentModel.DataAnnotations;

namespace GoldStore.Models
{
    public class RegTresholdVM
    {
        public short Status { get; set; }
        public long RegUserId { get; set; }

        [Display(Name = "کف قیمت خرید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int BuyThreshold { get; set; }

        [Display(Name = "کف قیمت فروش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int SelThreshold { get; set; }

        [Display(Name = "مبلغ پایه/جاری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public decimal CurrentPrice { get; set; }
        public DateTime RegDate { get; set; }

        [Display(Name = "تاریخ موثر انقضاء")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public DateTime ExpireEffectDate { get; set; }
    }
}
