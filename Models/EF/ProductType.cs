using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("ProductType")]
    public class ProductType
    {
        public ProductType()
        {
            ProductCategories = new HashSet<ProductCategory>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên loại sản phẩm không được để trống")]
        [Display(Name = "Tên loại sản phẩm")]
        public string Name { get; set; }

        public string Alias { get; set; }

        public virtual ICollection<ProductCategory> ProductCategories { get; set; }


    }
}