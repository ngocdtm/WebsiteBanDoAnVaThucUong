using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Models
{
    public class StoreProductViewModel
    {
        public IPagedList<StoreProduct> StoreProducts { get; set; }
        public Address CustomerAddress { get; set; }
        public decimal ShippingFee { get; set; }
        public List<StoreDTO> Stores { get; set; }
    }
}