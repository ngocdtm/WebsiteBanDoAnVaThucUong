using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("Contact")]
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(30, ErrorMessage = "Email Không được vượt quá 30 ký tự")]
        public string Email { get; set; }
        [StringLength(30, ErrorMessage = "Name Không được vượt quá 30 ký tự")]
        public string Name { get; set; }
        [StringLength(15, ErrorMessage = "Hotline Không được vượt quá 15 ký tự")]
        public string Hotline { get; set; }
        [StringLength(50, ErrorMessage = "Webstite Không được vượt quá 50 ký tự")]
        public string Website { get; set; }
        [StringLength(100, ErrorMessage = "Message Không được vượt quá 100 ký tự")]
        public string Message { get; set; }

    }
}