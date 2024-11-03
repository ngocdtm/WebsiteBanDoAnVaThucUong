using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    // Extra cho món ăn (phô mai, sốt,...)
    [Table("Extra")]
    public class Extra
    {
        public Extra()
        {
            this.ProductExtra = new HashSet<ProductExtra>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<ProductExtra> ProductExtra { get; set; }
    }
}