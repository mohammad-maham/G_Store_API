using System.ComponentModel.DataAnnotations;

namespace GoldStore.Models
{
    public partial class PriceCalcVM
    {
        [Display(Name = "نوع خدمت"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public CalcTypes GoldCalcType { get; set; } = CalcTypes.none;

        [Display(Name = "وزن طلا"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public double GoldWeight { get; set; } = 0.0;

        [Display(Name = "عیار"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public double GoldCarat { get; set; } = 750.0;
    }

    public enum CalcTypes
    {
        none = 0,
        buy = 1,
        sell = 2
    }
}
