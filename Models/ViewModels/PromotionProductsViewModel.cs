using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Models.ViewModels
{
    public class PromotionProductsViewModel
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public List<Product> AllProducts { get; set; }
        public List<int> SelectedProductIds { get; set; }
    }
}