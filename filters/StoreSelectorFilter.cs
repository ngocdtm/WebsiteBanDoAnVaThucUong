// File: Filters/StoreSelectorFilter.cs
using System;
using System.Linq;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;
using WebsiteBanDoAnVaThucUong.Models.EF; // Your namespace for ApplicationDbContext

namespace WebsiteBanDoAnVaThucUong.Filters
{
    public class StoreSelectorFilter : ActionFilterAttribute
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var stores = db.Stores
                .OrderBy(x => x.Name)
                .Select(s => new StoreDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Address = s.Address.StreetAddress 
                }).ToList();

            // Check if stores are populated correctly
            if (stores == null || !stores.Any())
            {
                // Log hoặc xử lý khi danh sách cửa hàng trống
                filterContext.Controller.ViewBag.ErrorMessage = "Không có cửa hàng nào!";
            }

            filterContext.Controller.ViewBag.Stores = stores;

            base.OnActionExecuting(filterContext);
        }
    }

}
