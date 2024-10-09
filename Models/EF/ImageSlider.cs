using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("Banner")]
    public class ImageSlider
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(250)]
        public string Image { get; set; }
    }
}