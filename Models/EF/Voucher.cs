using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("Voucher")]

    public class Voucher
    {
        public Voucher()
        {
            this.Orders = new HashSet<Order>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "VoucherName không được để trống")]
        [StringLength(50)]
        public string VoucherName { get; set; }
        [Required(ErrorMessage = "Mã Coupon không được để trống")]
        [StringLength(30)]
        public string Coupon { get; set; }
        [Required(ErrorMessage = "Voucher Description không được để trống")]
        [StringLength(100)]
        public string VoucherDes { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}