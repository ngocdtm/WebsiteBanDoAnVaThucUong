using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("Topping")]
    public class Topping
    {
        public Topping()
        {
            this.ProductTopping = new HashSet<ProductTopping>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string NameTopping { get; set; } // Ví dụ: "Trân châu", "Thạch"
        public decimal PriceTopping { get; set; } // Giá tăng thêm cho topping
        public virtual ICollection<ProductTopping> ProductTopping { get; set; }

    }
}