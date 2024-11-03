using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("ProductSize")]
    public class ProductSize
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [ForeignKey("Size")]
        public int SizeId { get; set; }

        public bool IsRecommended { get; set; } // Đánh dấu món này được khuyến nghị kèm
        public decimal SpecialPrice { get; set; } // Giá đặc biệt khi dùng kèm với món này

        public virtual Product Product { get; set; }
        public virtual Size Size { get; set; }
    }
}
