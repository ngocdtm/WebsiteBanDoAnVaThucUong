using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Category
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {

                    var items = db.Categories;
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
            if(User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Category model)
        {
            if (User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    model.Alias = WebsiteBanDoAnVaThucUong.Models.Common.Filter.FilterChar(model.Title);
                    db.Categories.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    var item = db.Categories.Find(id);
                    return View(item);
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View() ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Attach(model);
                model.ModifiedDate = DateTime.Now;
                model.Alias = WebsiteBanDoAnVaThucUong.Models.Common.Filter.FilterChar(model.Title);
                db.Entry(model).Property(x => x.Title).IsModified = true;
                db.Entry(model).Property(x => x.Description).IsModified = true;
                db.Entry(model).Property(x => x.Alias).IsModified = true;
                db.Entry(model).Property(x => x.Position).IsModified = true;
                db.Entry(model).Property(x => x.IsActive).IsModified = true;
                db.Entry(model).Property(x => x.ModifiedDate).IsModified = true;
                db.Entry(model).Property(x => x.ModifiedBy).IsModified = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Categories.Find(id);
            if (item != null)
            {
                //var DeleteItem = db.Categories.Attach(item);
                db.Categories.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }


        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.Categories.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isAcive = item.IsActive });
            }

            return Json(new { success = false });
        }
    }
}