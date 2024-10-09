using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("ProductImage")]
    public class ProductImage
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public string Image { get; set; }
        public bool IsDefault { get; set; }

        public virtual Product Products { get; set; }
    }
}