using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Areas.Admin.Controllers
{
    public class StoresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Stores
        public ActionResult Index(int? page)
        {
            IEnumerable<Store> items = db.Stores.OrderByDescending(x => x.Id);
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

        // GET: Admin/Stores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // GET: Admin/Stores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address,Long,Lat,Image,IdManager")] Store store, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    //Lấy tên file của hình được up lên

                    var fileName = Path.GetFileName(Image.FileName);

                    //Tạo đường dẫn tới file

                    var path = Path.Combine(Server.MapPath("~/Image"), fileName);
                    //Lưu tên

                    store.Image = fileName;
                    //Save vào Images Folder
                    Image.SaveAs(path);

                }
                store.IdManager = User.Identity.GetUserId();
                db.Stores.Add(store);
                store.Alias = WebsiteBanDoAnVaThucUong.Models.Common.Filter.FilterChar(store.Name);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(store);
        }

        // GET: Admin/Stores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,Long,Lat,Image,IdManager")] Store store, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {


                store.Alias = WebsiteBanDoAnVaThucUong.Models.Common.Filter.FilterChar(store.Name);
                if (Image != null)
                {
                    //Lấy tên file của hình được up lên

                    var fileName = Path.GetFileName(Image.FileName);

                    //Tạo đường dẫn tới file

                    var path = Path.Combine(Server.MapPath("~/Image"), fileName);
                    //Lưu tên

                    store.Image = fileName;
                    //Save vào Images Folder
                    Image.SaveAs(path);

                }
                db.Stores.Attach(store);
                db.Entry(store).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var store = db.Stores.Find(id);
            if (store == null)
            {
                return Json(new { success = false, message = "Chi nhánh không tồn tại." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                db.Stores.Remove(store);
                db.SaveChanges();
                return Json(new { success = true, message = "Chi nhánh đã được xóa thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
