using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
    public class ImageSlidersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ImageSliders
        public ActionResult Index()
        {
            var data = (from d in db.ImageSlider select d).ToList();
            return View(data);
        }

        // GET: ImageSliders/Details/5
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

        // GET: ImageSliders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ImageSliders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Image")] ImageSlider imageSlider)
        {
            if (ModelState.IsValid)
            {
                db.ImageSlider.Add(imageSlider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(imageSlider);
        }

        // GET: ImageSliders/Edit/5
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

        // POST: ImageSliders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Image")] ImageSlider imageSlider)
        {
            if (ModelState.IsValid)
            {
                db.Entry(imageSlider).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(imageSlider);
        }

        // GET: ImageSliders/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: ImageSliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ImageSlider imageSlider = db.ImageSlider.Find(id);
            db.ImageSlider.Remove(imageSlider);
            db.SaveChanges();
            return RedirectToAction("Index");
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
