using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("ProductViewHistory")]
    public class ProductViewHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        public string UserId { get; set; } // Assuming user tracking via UserId
        public DateTime ViewedAt { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}