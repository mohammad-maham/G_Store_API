using System.ComponentModel.DataAnnotations;

namespace GoldStore.Models
{
    public partial class AmountThresholdVM
    {
        [Display(Name = "شناسه"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long Id { get; set; }

        [Display(Name = "وضعیت"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public short Status { get; set; }

        [Display(Name = "شناسه کاربری ثبت کننده"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long RegUserId { get; set; }

        [Display(Name = "قیمت خرید"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public double BuyThreshold { get; set; }

        [Display(Name = "قیمت فروش"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public double SelThreshold { get; set; }

        [Display(Name = "قیمت جاری")]
        public double CurrentPrice { get; set; }

        [Display(Name = "تاریخ انقضاء"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public DateTime ExpireEffectDate { get; set; }

        [Display(Name = "قیمت آنلاین"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public short IsOnlinePrice { get; set; }
    }
}
