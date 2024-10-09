using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("Promotion")]
    public class Promotion
    {
        public Promotion()
        {
            //this.PromotionRule = new HashSet<PromotionRule>();
            this.PromotionProduct = new HashSet<PromotionProduct>();
            this.OrderDetailPromotion = new HashSet<OrderDetailPromotion>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DiscountType { get; set; } // 1: Percentage, 2: Combo, 3: Buy X Get Y
        public decimal DiscountValue { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<PromotionProduct> PromotionProduct { get; set; }
        public virtual ICollection<OrderDetailPromotion> OrderDetailPromotion { get; set; }
    }
}