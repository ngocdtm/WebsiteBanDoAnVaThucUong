using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("Order")]
    public class Order : CommonAbstract
    {
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            this.Stores = new HashSet<Store>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        public int TotalQuantity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ProductDiscountTotal { get; set; }
        [ForeignKey("Vouchers")]
        public int? VoucherId { get; set; }
        public decimal VoucherDiscount { get; set; }
        public decimal FinalAmount { get; set; }
        public int TypePayment { get; set; }
        public string CustomerId { get; set; }
        public int StoreId { get; set; }
        public int OrderStatus { get; set; }
        public int ShippingStatus { get; set; }
        public decimal ShippingFee { get; set; }
        public virtual Voucher Vouchers { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
        public ApplicationUser User { get; set; }
    }
}