using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models.EF;
using WebsiteBanDoAnVaThucUong.Models;
using PagedList.Mvc;
using PagedList;
using Microsoft.AspNet.Identity;

namespace WebsiteBanDoAnVaThucUong.Areas.Admin.Controllers
{
    public class NewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/News
        public ActionResult Index(string Searchtext, int? page)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    var pageSize = 10;
                    if (page == null)
                    {
                        page = 1;
                    }
                    IEnumerable<New> items = db.News.OrderByDescending(x => x.Id);
                    if (!string.IsNullOrEmpty(Searchtext))
                    {
                        items = items.Where(x => x.Alias.Contains(Searchtext) || x.Title.Contains(Searchtext) || x.CreatedDate.ToString().Contains(Searchtext));
                    }
                    var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    items = items.ToPagedList(pageIndex, pageSize);
                    ViewBag.PageSize = pageSize;
                    ViewBag.Page = page;
                    return View(items);
                }
            }
           
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(New model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedBy = User.Identity.GetUserId();  // Lấy Id của user đang đăng nhập
                model.CreatedDate = DateTime.Now;
                //model.CategoryId = 6;
                model.ModifiedBy = User.Identity.GetUserId(); // Lấy Id của user đang đăng nhập
                model.ModifiedDate = DateTime.Now;
                model.Alias = WebsiteBanDoAnVaThucUong.Models.Common.Filter.FilterChar(model.Title);
                db.News.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var item = db.News.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(New model)
        {
            if (ModelState.IsValid)
            {
                var existingNews = db.News.Find(model.Id);
                if (existingNews != null)
                {
                    existingNews.Title = model.Title;
                    existingNews.Alias = WebsiteBanDoAnVaThucUong.Models.Common.Filter.FilterChar(model.Title);
                    existingNews.Description = model.Description;
                    existingNews.Detail = model.Detail;
                    existingNews.Image = model.Image;
                    existingNews.IsActive = model.IsActive;
                    existingNews.ModifiedBy = User.Identity.GetUserId();
                    existingNews.ModifiedDate = DateTime.Now;

                    db.Entry(existingNews).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.News.Find(id);
            if (item != null)
            {
                db.News.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.News.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isAcive = item.IsActive });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.News.Find(Convert.ToInt32(item));
                        db.News.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

    }
}