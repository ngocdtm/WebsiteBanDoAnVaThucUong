using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;
using WebsiteBanDoAnVaThucUong.Models.ViewModels;

namespace WebsiteBanDoAnVaThucUong.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Order
        public ActionResult Index(int? page)
        {
            var items = db.Orders.Include(o => o.User).OrderByDescending(x => x.CreatedDate).ToList();

            if (page == null)
            {
                page = 1;
            }
            var pageNumber = page ?? 1;
            var pageSize = 10;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;
            return View(items.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult View(int id)
        {
            var item = db.Orders.Include(o => o.User).FirstOrDefault(o => o.Id == id);
            return View(item);
        }

        public ActionResult Partial_SanPham(int id)
        {
            var items = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            return PartialView(items);
        }

        [HttpPost]
        public ActionResult UpdateShippingStatus(int id, int status)
        {
            var order = db.Orders.Find(id);
            if (order != null)
            {
                order.ShippingStatus = status;
                order.ModifiedBy = User.Identity.GetUserId();
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public void ThongKe(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails on o.Id equals od.OrderId
                        join p in db.Products
            on od.ProductId equals p.Id
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.UnitPrice,
                            OriginalPrice = p.OriginalPrice
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime start = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate >= start);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate < endDate);
            }
            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(r => new
            {
                Date = r.Key.Value,
                TotalBuy = r.Sum(x => x.OriginalPrice * x.Quantity), // tổng giá bán
                TotalSell = r.Sum(x => x.Price * x.Quantity) // tổng giá mua
            }).Select(x => new RevenueStatisticViewModel
            {
                Date = x.Date,
                Benefit = x.TotalSell - x.TotalBuy,
                Revenues = x.TotalSell
            });
        }
    }
}