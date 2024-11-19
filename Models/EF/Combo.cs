using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
  
    // Combo (Set đồ ăn kèm nước)
    [Table("Combo")]
    public class Combo
    {
        public Combo()
        {
            this.ComboDetail = new HashSet<ComboDetail>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<ComboDetail> ComboDetail { get; set; }
        [ForeignKey("ProductCategory")]
        public int? ProductCategoryId { get; set; } // Category để phân loại combo
        public virtual ProductCategory ProductCategory { get; set; }
    }
}