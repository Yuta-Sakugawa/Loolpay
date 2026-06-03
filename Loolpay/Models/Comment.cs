using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Loolpay.Models
{
    public class Comment
    {
        public int Id { get; set; }
        
        public int StoreId { get; set; }
        public Store Store { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;

        [Required]
        [Display(Name = "コメント")]
        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
