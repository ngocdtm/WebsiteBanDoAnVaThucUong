using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("OrderDetailPromotion")]
    public class OrderDetailPromotion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("OrderDetail")]
        public int OrderDetailId { get; set; }
        [ForeignKey("Promotion")]
        public int PromotionId { get; set; }
        public decimal DiscountAmount { get; set; }

        public virtual Promotion Promotion { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
        
    }
}