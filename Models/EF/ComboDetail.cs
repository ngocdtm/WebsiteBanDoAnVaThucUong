using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    // Chi tiết trong combo
    [Table("ComboDetail")]
    public class ComboDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Combo")]
        public int ComboId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public bool IsRequired { get; set; } // Bắt buộc hay không khi thêm món ăn kèm, topping, nước ống,...
        public decimal AdditionalPrice { get; set; } // Giá tăng thêm nếu có

        public virtual Combo Combo { get; set; }
        public virtual Product Product { get; set; }
    }
}