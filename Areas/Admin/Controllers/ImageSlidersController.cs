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
using System.Web.UI.WebControls;
using WebsiteBanDoAnVaThucUong.Models;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Areas.Admin.Controllers
{
    public class ImageSlidersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/ImageSliders
        public ActionResult Index(int? page)
        {
            IEnumerable<ImageSlider> items = db.ImageSlider.OrderByDescending(x => x.Id);
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

        // GET: Admin/ImageSliders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImageSlider imageSlider = db.ImageSlider.Find(id);
            if (imageSlider == null)
            {
                return HttpNotFound();
            }
            return View(imageSlider);
        }

        // GET: Admin/ImageSliders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ImageSliders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

            public ActionResult Create([Bind(Include = "Id,Image")] ImageSlider imageSlider, HttpPostedFileBase Image)
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

                        imageSlider.Image = fileName;
                        //Save vào Images Folder
                        Image.SaveAs(path);
                    }

                    db.ImageSlider.Add(imageSlider);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(imageSlider);
            }

        // GET: Admin/ImageSliders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImageSlider imageSlider = db.ImageSlider.Find(id);
            if (imageSlider == null)
            {
                return HttpNotFound();
            }
            return View(imageSlider);
        }

        // POST: Admin/ImageSliders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Image")] ImageSlider imageSlider, HttpPostedFileBase Image)
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

                    imageSlider.Image = fileName;
                    //Save vào Images Folder
                    Image.SaveAs(path);
                }
                db.Entry(imageSlider).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(imageSlider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var slide = db.ImageSlider.Find(id);
            if (slide == null)
            {
                return Json(new { success = false, message = "Slide không tồn tại." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                db.ImageSlider.Remove(slide);
                db.SaveChanges();
                return Json(new { success = true, message = "Slide đã được xóa thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //// POST: Admin/ImageSliders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    ImageSlider imageSlider = db.ImageSlider.Find(id);
        //    db.ImageSlider.Remove(imageSlider);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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

