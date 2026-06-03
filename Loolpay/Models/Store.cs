using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loolpay.Models
{
    public enum StoreGenre
    {
        [Display(Name = "コンビニ")]
        ConvenienceStore,
        [Display(Name = "スーパー")]
        Supermarket,
        [Display(Name = "飲食店")]
        Restaurant,
        [Display(Name = "カフェ")]
        Cafe,
        [Display(Name = "ドラッグストア")]
        DrugStore,
        [Display(Name = "ショッピングモール")]
        ShoppingMall,
        [Display(Name = "ガソリンスタンド")]
        GasStation,
        [Display(Name = "ホテル")]
        Hotel,
        [Display(Name = "その他")]
        Other
    }

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

        [Column("genre")]
        [Display(Name = "Genre")]
        public StoreGenre Genre { get; set; }

        [MaxLength(255)]
        [Column("image_path")]
        public string? ImagePath { get; set; }

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [NotMapped]
        public List<string> SelectedPaymentMethods { get; set; } = new List<string>();
    }
}
