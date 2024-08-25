using System.ComponentModel.DataAnnotations;

namespace GoldStore.Models
{
    public class PriceCalcVM
    {
        [Display(Name = "نوع خدمت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int GoldCalcType { get; set; }

        [Display(Name = "وزن طلا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public double GoldWeight { get; set; }

        [Display(Name = "عیار")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public double GoldCarat { get; set; }
    }

    public enum CalcTypes
    {
        none = 0,
        buy = 1,
        sell = 2,
        threshold = 3,
    }
}
