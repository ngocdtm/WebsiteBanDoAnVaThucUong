using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("ProductCategory")]
    public class ProductCategory
    {
        public ProductCategory()
        {
            this.Products = new HashSet<Product>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title không được để trống")]
        [StringLength(50, ErrorMessage = "Title Không được vượt quá 50 ký tự")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Alias không được để trống")]
        [StringLength(30, ErrorMessage = "Alias Không được vượt quá 30 ký tự")]
        public string Alias { get; set; }
        [StringLength(200, ErrorMessage = "Description Không được vượt quá 50 ký tự")]
        public string Description { get; set; }
        [StringLength(100, ErrorMessage = "Icon Không được vượt quá 50 ký tự")]
        public string Icon {  get; set; }

        public ICollection<Product> Products { get; set; }
    }
}