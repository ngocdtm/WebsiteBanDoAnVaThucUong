using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
    public class MenuController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuTop()
        {
            //var items = db.Categories.OrderByDescending(x => x.Position).ToList();
            var items = db.Categories.Where(x => x.IsActive).Take(12).OrderBy(x => x.Position).ToList();
            return PartialView("_MenuTop", items);
        }

        public ActionResult MenuProductCategory()
        {
            var items = db.ProductCategories.ToList();
            return PartialView("_MenuProductCategory", items);
        }

        public ActionResult MenuLeft(int? id)
        {
            if (id != null)
            {
                ViewBag.CateId = id;
            }
            var items = db.ProductCategories.ToList();
            return PartialView("_MenuLeft", items);
        }

        public ActionResult MenuArrivals()
        {
            var items = db.ProductCategories.ToList();
            return PartialView("_MenuArrivals", items);
        }

        public ActionResult MenuHot()
        {
            var items = db.ProductCategories.ToList();
            return PartialView("_MenuHot", items);
        }
    }
}