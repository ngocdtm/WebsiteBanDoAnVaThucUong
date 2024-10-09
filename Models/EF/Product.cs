using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("Product")]
    public class Product
    {
        public Product()
        {
            this.ProductImage = new HashSet<ProductImage>();
            this.OrderDetail = new HashSet<OrderDetail>();
            this.Review = new HashSet<ReviewProduct>();
            this.Wishlist = new HashSet<Wishlist>();
            this.PromotionProduct = new HashSet<PromotionProduct>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Title { get; set; }

        [Required]
        [StringLength(250)]
        public string Alias { get; set; }

        public string Description { get; set; }
    
        [AllowHtml]
        public string Detail { get; set; }

        [StringLength(250)]
        public string Image { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public int ViewCount { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("ProductCategory")]
        public int ProductCategoryId { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<ProductImage> ProductImage { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        public virtual ICollection<ReviewProduct> Review { get; set; }
        public virtual ICollection<Wishlist> Wishlist { get; set; }
        public virtual ICollection<PromotionProduct> PromotionProduct { get; set; }
    
    }
}