using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("Category")]
    public class Category : CommonAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên danh mục không để trống")]
        [StringLength(150)]
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }
        public int Position { get; set; }
        public bool IsActive { get; set; }

        public virtual ApplicationUser UserCreate { get; set; }
        public virtual ApplicationUser UserModified { get; set; }
    }
}