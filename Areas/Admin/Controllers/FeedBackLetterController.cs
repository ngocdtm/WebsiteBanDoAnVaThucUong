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
    public class FeedBackLetterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/FeedBackLetter
        public ActionResult Index(int? page)
        {
            var items = db.FeedBackLetters.Include(o => o.User).OrderByDescending(x => x.CreateDate).ToList();

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
    }
}