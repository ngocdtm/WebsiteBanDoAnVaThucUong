using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteBanDoAnVaThucUong.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    if (User.IsInRole("Admin"))
            //    {
            //        return View();
            //    }
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            return View();
        }
    }
}