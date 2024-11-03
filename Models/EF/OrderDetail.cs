using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        public OrderDetail()
        {
            this.OrderDetailPromotion = new HashSet<OrderDetailPromotion>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Orders")]
        public int OrderId { get; set; }
        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public virtual Order Orders { get; set; }
        public virtual Product Products { get; set; }
        // Thông tin về size, topping, extra
        public string SelectedSizeIds { get; set; }  // Lưu dưới dạng chuỗi JSON
        public string SelectedToppingIds { get; set; }  // Lưu dưới dạng chuỗi JSON
        public string SelectedExtraIds { get; set; }  // Lưu dưới dạng chuỗi JSON

        // Giá của từng option
        public decimal SizePrice { get; set; }
        public decimal ToppingPrice { get; set; }
        public decimal ExtraPrice { get; set; }

        public virtual ICollection<OrderDetailPromotion> OrderDetailPromotion { get; set; }
    }
}