using System.ComponentModel.DataAnnotations;

namespace GoldStore.Models
{
    public class ChargeStore
    {
        [Display(Name = "وضعیت")]
        public short Status { get; set; } = 1;

        [Display(Name = "نوع طلا"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public short GoldType { get; set; }

        [Display(Name = "وزن(گرم)"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Weight { get; set; }

        [Display(Name = "عیار"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public short Carat { get; set; }

        [Display(Name = "شناسه کاربر ثبت کننده"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long RegUserId { get; set; }

        public short EntityType { get; set; } = 0;

        [Display(Name = "اطلاعات عیار")]
        public string? CaratologyInfo { get; set; }

        [Display(Name = "شارژ/دشارژ"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Decharge { get; set; } = 0;
    }
}
