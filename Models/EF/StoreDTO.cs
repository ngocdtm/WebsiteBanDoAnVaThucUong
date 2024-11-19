using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    public class StoreDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }
        public string Image { get; set; }
        public string Alias { get; set; }
        public decimal ShippingFee {  get; set; }
    }

}