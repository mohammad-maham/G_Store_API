using System.ComponentModel.DataAnnotations;
using static GoldStore.Models.Enums;

namespace GoldStore.Models
{
    public class PriceCalcVM
    {
        [Display(Name = "نوع خدمت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int CalcType { get; set; }

        [Display(Name = "وزن طلا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public double Weight { get; set; }

        [Display(Name = "عیار")]
        public double? Carat { get; set; }

        [Display(Name = "نوع محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public EntityTypes EntityId { get; set; }

        [Display(Name = "شناسه متریال")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long? MaterialId { get; set; }
    }

    public enum CalcTypes
    {
        none = 0,
        buy = 1,
        sell = 2,
        threshold = 3,
    }
}
