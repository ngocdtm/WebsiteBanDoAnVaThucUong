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
            // Initialize collections in constructor
            ProductImage = new HashSet<ProductImage>();
            OrderDetail = new HashSet<OrderDetail>();
            Review = new HashSet<ReviewProduct>();
            Wishlist = new HashSet<Wishlist>();
            PromotionProduct = new HashSet<PromotionProduct>();
            StoreProducts = new HashSet<StoreProduct>();
            ProductSize = new HashSet<ProductSize>();
            ProductTopping = new HashSet<ProductTopping>();
            ProductExtra = new HashSet<ProductExtra>();
            ComboDetail = new HashSet<ComboDetail>();

            // Initialize selection lists
            SelectedToppingIds = new List<int>();
            SelectedSizeIds = new List<int>();
            SelectedExtraIds = new List<int>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(250, ErrorMessage = "Tên sản phẩm không được vượt quá 250 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Alias không được để trống")]
        [StringLength(250, ErrorMessage = "Alias không được vượt quá 250 ký tự")]
        public string Alias { get; set; }

        [Display(Name = "Mô tả ngắn")]
        public string Description { get; set; }

        [AllowHtml]
        [Display(Name = "Chi tiết")]
        public string Detail { get; set; }

        [StringLength(250)]
        [Display(Name = "Ảnh đại diện")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá gốc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá gốc phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giá gốc")]
        public decimal OriginalPrice { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giá bán")]
        public decimal SalePrice { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Lượt xem")]
        public int ViewCount { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục sản phẩm")]
        [Display(Name = "Danh mục")]
        public int ProductCategoryId { get; set; }

        // Not mapped properties for form binding
        [NotMapped]
        [Display(Name = "Extra")]
        public List<int> SelectedExtraIds { get; set; }

        [NotMapped]
        [Display(Name = "Topping")]
        public List<int> SelectedToppingIds { get; set; }

        [NotMapped]
        [Display(Name = "Size")]
        public List<int> SelectedSizeIds { get; set; }

        [NotMapped]
        [Display(Name = "Loại sản phẩm")]
        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
        public int? ProductTypeId { get; set; }

        // Navigation properties
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<ProductImage> ProductImage { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        public virtual ICollection<ReviewProduct> Review { get; set; }
        public virtual ICollection<Wishlist> Wishlist { get; set; }
        public virtual ICollection<PromotionProduct> PromotionProduct { get; set; }
        public virtual ICollection<StoreProduct> StoreProducts { get; set; }
        public virtual ICollection<ProductSize> ProductSize { get; set; }
        public virtual ICollection<ProductTopping> ProductTopping { get; set; }
        public virtual ICollection<ProductExtra> ProductExtra { get; set; }
        public virtual ICollection<ComboDetail> ComboDetail { get; set; }
    }
}