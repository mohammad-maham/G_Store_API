using System.ComponentModel.DataAnnotations;

namespace GoldStore.Models
{
    public class OrderVM
    {
        [Required(ErrorMessage ="لطفا {0} را وارد کنید")]
        public int Weight { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long UserId { get; set; }
    }
}
