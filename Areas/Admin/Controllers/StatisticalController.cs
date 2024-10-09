using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;

namespace WebsiteBanDoAnVaThucUong.Areas.Admin.Controllers
{
    public class StatisticalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Statistical
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetStatistical(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails on o.Id equals od.OrderId
                        join p in db.Products on od.ProductId equals p.Id
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.UnitPrice,
                            OriginalPrice = p.OriginalPrice,
                            ProductId = p.Id,
                            ProductName = p.Title
                        };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate);
            }

            var revenueResult = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price),
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy
            }).ToList();

            var productOrderResult = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalQuantity = x.Sum(y => y.Quantity),
                OrderCount = x.Select(y => y.Quantity).Count()
            }).ToList();

            var topProducts = query.GroupBy(x => new { x.ProductId, x.ProductName }).Select(x => new
            {
                ProductName = x.Key.ProductName,
                TotalQuantity = x.Sum(y => y.Quantity)
            }).OrderByDescending(x => x.TotalQuantity).Take(5).ToList();

            return Json(new { RevenueData = revenueResult, ProductOrderData = productOrderResult, TopProducts = topProducts }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFilteredOrders(decimal minTotal, decimal maxTotal)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails on o.Id equals od.OrderId
                        group od by new { o.Id, o.CreatedDate } into g
                        let total = g.Sum(x => x.Quantity * x.UnitPrice)
                        where total >= minTotal && total <= maxTotal
                        select new
                        {
                            OrderId = g.Key.Id,
                            CreatedDate = g.Key.CreatedDate,
                            Total = total
                        };

            var result = query.ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetProductStatistics()
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails on o.Id equals od.OrderId
                        join p in db.Products on od.ProductId equals p.Id
                        select new
                        {
                            ProductName = p.Title,
                            Quantity = od.Quantity
                        };

            var result = query.GroupBy(x => x.ProductName).Select(x => new
            {
                ProductName = x.Key,
                TotalQuantity = x.Sum(y => y.Quantity)
            }).OrderByDescending(x => x.TotalQuantity).Take(5).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

    }
}