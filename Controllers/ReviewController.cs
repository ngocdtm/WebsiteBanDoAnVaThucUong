using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models.EF;
using WebsiteBanDoAnVaThucUong.Models;
using PagedList;
using Microsoft.Reporting.WebForms;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
    public class ReviewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Review
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult LichSuDonHang(int ? page)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var userManager = new UserManager<ApplicationUser>(userStore);
                var user = userManager.FindByName(User.Identity.Name);
                //IEnumerable<Order> items = db.Orders.OrderByDescending(x => x.Id);
                IEnumerable<Order> items = db.Orders.AsEnumerable().Where(x => x.CustomerId == User.Identity.GetUserId()).OrderByDescending(x => x.CreatedDate);
                var pageSize = 10;
                if (page == null)
                {
                    page = 1;
                }
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                items = items.ToPagedList(pageIndex, pageSize);
                ViewBag.PageSize = pageSize;
                ViewBag.Page = page;

                return View(items);
            }
           
            return View();
        }

        public ActionResult Detail(int id)
        {
            var item = db.Orders.Find(id);
            if (item != null)
            {
                db.Orders.Attach(item);
                return View(item);
            }
            return View(item);
        }

        public ActionResult Partial_CTSanPham(int id)
        {
            var items = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            return PartialView(items);
        }

        [HttpPost]
        public ActionResult CancelOrder(int id)
        {
            var order = db.Orders.Find(id);
            if (order != null)
            {
                order.ShippingStatus = 5; // Update the order status to "Cancelled"
                db.SaveChanges();
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        [AllowAnonymous]
        public ActionResult _Review(int productId)
        {
            ViewBag.ProductId = productId;
            var item = new ReviewProduct();
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                item.User = user;
            }
            return PartialView(item);
        }

        [AllowAnonymous]
        public ActionResult _Load_Review(int productId)
        {
            var item = db.Reviews.Where(x => x.ProductId == productId).OrderByDescending(x => x.Id).ToList();
            ViewBag.Count = item.Count;
            return PartialView(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostReview(ReviewProduct req)
        {
            if (ModelState.IsValid)
            {
                req.CreatedDate = DateTime.Now;
                req.CustomerId = User.Identity.GetUserId();
                db.Reviews.Add(req);
                db.SaveChanges();
                return Json(new { Success = true });
            }
            return PartialView("_Review", req);
        }

        //[AllowAnonymous]
        //[HttpPost]
        //public ActionResult PostReview(ReviewProduct req)
        //{
        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("_Review", "Review") });
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        req.CreatedDate = DateTime.Now;
        //        req.CustomerId = User.Identity.GetUserId();
        //        req.User = db.Users.Find(req.CustomerId);
        //        db.Reviews.Add(req);
        //        db.SaveChanges();
        //        return Json(new { Success = true });
        //    }
        //    // Nếu ModelState không hợp lệ, cần gán lại User cho model
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var userId = User.Identity.GetUserId();
        //        req.User = db.Users.Find(userId);
        //    }
        //    return PartialView(req);
        //}


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public ActionResult Reports(string ReportType)
        {
            LocalReport localreport = new LocalReport();
            //LocalR
            localreport.ReportPath = Server.MapPath("~/Reports/Report1.rdlc");

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1" ;
            reportDataSource.Value = db.Orders;
            localreport.DataSources.Add(reportDataSource);

            string reportType = ReportType;
            string mimeType;
            string encoding;
            string fileNameExtension;

            if (reportType == "Excel")
            {
                fileNameExtension = "xlsx";
            }
            else if (reportType == "Word")
            {
                fileNameExtension = "docx";
            }
            else if (reportType == "PDF")
            {
                fileNameExtension = "pdf";
            }
            else
            {
                fileNameExtension = "jpg";
            }

            string[] streams;
            Warning[] warnings;
            byte[] renderByte;
            renderByte = localreport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            Response.AddHeader("content-disposition", "attachment;filename = order_report." + fileNameExtension);

            return File(renderByte, fileNameExtension);
        }
    }
}