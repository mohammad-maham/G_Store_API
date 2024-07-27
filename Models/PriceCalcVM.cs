using System.ComponentModel.DataAnnotations;

namespace GoldStore.Models
{
    public class PriceCalcVM
    {
        [Display(Name = "نوع خدمت"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public CalcTypes GoldCalcType { get; set; } = CalcTypes.none;

        [Display(Name = "وزن طلا"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public double GoldWeight { get; set; } = 0.0;

        [Display(Name = "نوع محصول"), Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public ProductTypes ProductTypes { get; set; }
    }

    public enum CalcTypes
    {
        none = 0,
        buy = 1,
        sell = 2
    }

    public enum ProductTypes
    {
        global = 0,
        gold750 = 1,
        gold870 = 2,
        gold910 = 3,
        gold100 = 4,
        goldTransferSlip = 5
    }
}
