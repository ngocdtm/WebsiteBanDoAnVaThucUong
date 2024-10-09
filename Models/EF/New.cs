using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("New")]
    public class New : CommonAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tiêu đề tin tức không được để trống")]
        [StringLength(100, ErrorMessage = "Title không được vượt quá 100 ký tự")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Alias không được để trống")]
        [StringLength(100, ErrorMessage = "Alias không được vượt quá 100 ký tự")]
        public string Alias { get; set; }
        [Required(ErrorMessage = "Description không được để trống")]
        [StringLength(200, ErrorMessage = "Description không được vượt quá 100 ký tự")]
        public string Description { get; set; }

        [AllowHtml]
        public string Detail { get; set; }
        [Required(ErrorMessage = "Image không được để trống")]
        [StringLength(100, ErrorMessage = "Image không được vượt quá 100 ký tự")]
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public virtual ApplicationUser User { get; set; }
        //public virtual ApplicationUser UserModified { get; set; }
    }
}