using System.ComponentModel.DataAnnotations;

namespace GoldStore.Models
{
    public class ChargeRepository
    {
        [Display(Name = "وضعیت")]
        public short Status { get; set; } = 1;    
        [Display(Name = "وزن(گرم)"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long Weight { get; set; }
        [Display(Name = "شناسه کاربر ثبت کننده"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long RegUserId { get; set; }
        [Display(Name = "نوع موجودیت"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int EntityType { get; set; } = 0;
        [Display(Name = "شارژ/دشارژ"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Decharge { get; set; } = 0;
        [Display(Name = "نوع نگهداری"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int MaintenanceType { get; set; }
    }
}
