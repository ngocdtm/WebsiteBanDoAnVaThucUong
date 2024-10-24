using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("Topping")]
    public class Topping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NameTopping { get; set; } // Ví dụ: "Trân châu", "Thạch"

        public decimal PriceTopping { get; set; } // Giá tăng thêm cho topping

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
