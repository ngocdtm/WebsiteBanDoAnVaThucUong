using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    // Mapping giữa Product và Extra
    [Table("ProductExtra")]
    public class ProductExtra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [ForeignKey("Extra")]
        public int ExtraId { get; set; }

        public bool IsRecommended { get; set; } // Đánh dấu món này được khuyến nghị kèm
        public decimal SpecialPrice { get; set; } // Giá đặc biệt khi dùng kèm với món này

        public virtual Product Product { get; set; }
        public virtual Extra Extra { get; set; }
    }
}