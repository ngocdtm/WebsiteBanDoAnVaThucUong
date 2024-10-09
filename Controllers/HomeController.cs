using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models.EF;
using WebsiteBanDoAnVaThucUong.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {

            //WebBanHangOnline.Common.Common.SendMail("ABC", "AAAA", "AAAA", "ngohoang29@gmail.com");
            return View();
        }

        public ActionResult Partial_Subcrice()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult Subscribe(Subscribe req)
        {
            if (ModelState.IsValid)
            {
                db.Subscribe.Add(new Subscribe { Email = req.Email, CreatedDate = DateTime.Now });
                db.SaveChanges();
                return Json(new { Success = true });
            }
            return View("Partial_Subcrice", req);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Refresh()
        {
            var item = new ThongKeModel();

            ViewBag.Visitors_online = HttpContext.Application["visitors_online"];
            var hn = HttpContext.Application["HomNay"];
            item.HomNay = HttpContext.Application["HomNay"].ToString();
            item.HomQua = HttpContext.Application["HomQua"].ToString();
            item.TuanNay = HttpContext.Application["TuanNay"].ToString();
            item.TuanTruoc = HttpContext.Application["TuanTruoc"].ToString();
            item.ThangNay = HttpContext.Application["ThangNay"].ToString();
            item.ThangTruoc = HttpContext.Application["ThangTruoc"].ToString();
            item.TatCa = HttpContext.Application["TatCa"].ToString();
            return PartialView(item);
        }
        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult FeedBackLetter()
        {
            var model = new FeedBackLetter();
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                model.User = user;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FeedBackLetter(FeedBackLetter model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("FeedBackLetter", "Home") });
            }
            if (ModelState.IsValid)
            {
                model.CreateDate = DateTime.Now;
                model.CreateBy = User.Identity.GetUserId();
                model.User = db.Users.Find(model.CreateBy); // Gán User trước khi thêm vào database
                db.FeedBackLetters.Add(model);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Gửi đơn thành công!";
                return RedirectToAction("FeedBackLetter");
            }
            // Nếu ModelState không hợp lệ, cần gán lại User cho model
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                model.User = db.Users.Find(userId);
            }
            return View(model);
        }

    }
}