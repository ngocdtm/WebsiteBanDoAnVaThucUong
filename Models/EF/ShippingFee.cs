using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("ShippingFee")]
    public class ShippingFee
    {
        public int Id { get; set; }
        public decimal FeePerKm { get; set; }
        public decimal MinimumFee { get; set; }
    }
}