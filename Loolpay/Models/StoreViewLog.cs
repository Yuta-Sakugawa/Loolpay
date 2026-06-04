using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loolpay.Models
{
    [Table("store_view_logs")]
    public class StoreViewLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        [Column("store_id")]
        public int StoreId { get; set; }

        [ForeignKey("StoreId")]
        public Store? Store { get; set; }

        [Required]
        [Column("viewed_at")]
        public DateTime ViewedAt { get; set; } = DateTime.Now;
    }
}
