using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("FeedBackLetter")]
    public class FeedBackLetter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title không được để trống")]
        [StringLength(100)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Content không được để trống")]
        [StringLength(100)]
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}