using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loolpay.Models
{
    [Table("favorites")]
    public class Favorite
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [Key, Column(Order = 1)]
        public int StoreId { get; set; }

        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; } = null!;
    }
}
