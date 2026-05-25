using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loolpay.Models
{
    [Table("stores")]
    public class Store
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("store_id")]
        public int StoreId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("store_name")]
        [Display(Name = "Store Name")]
        public string StoreName { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("store_address")]
        [Display(Name = "Address")]
        public string? StoreAddress { get; set; }

        [MaxLength(200)]
        [Column("pay")]
        public string? Pay { get; set; }

        [MaxLength(255)]
        [Column("image_path")]
        public string? ImagePath { get; set; }

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [NotMapped]
        public List<string> SelectedPaymentMethods { get; set; } = new List<string>();
    }
}
