//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using WebsiteBanDoAnVaThucUong.Models.EF;
//using WebsiteBanDoAnVaThucUong.Models;

//namespace WebsiteBanDoAnVaThucUong.Areas.Admin.Controllers
//{
//    public class PromotionTypesController : Controller
//    {
//        private ApplicationDbContext db = new ApplicationDbContext();

//        // GET: Admin/PromotionTypes
//        public ActionResult Index()
//        {
//            return View(db.PromotionType.ToList());
//        }

//        // GET: Admin/PromotionTypes/Details/5
//        public ActionResult Details(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            PromotionType promotionType = db.PromotionType.Find(id);
//            if (promotionType == null)
//            {
//                return HttpNotFound();
//            }
//            return View(promotionType);
//        }

//        // GET: Admin/PromotionTypes/Create
//        public ActionResult Create()
//        {
//            return View();
//        }

//        // POST: Admin/PromotionTypes/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "Id,NameType,Description")] PromotionType promotionType)
//        {
//            if (ModelState.IsValid)
//            {
//                db.PromotionType.Add(promotionType);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            return View(promotionType);
//        }

//        // GET: Admin/PromotionTypes/Edit/5
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            PromotionType promotionType = db.PromotionType.Find(id);
//            if (promotionType == null)
//            {
//                return HttpNotFound();
//            }
//            return View(promotionType);
//        }

//        // POST: Admin/PromotionTypes/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "Id,NameType,Description")] PromotionType promotionType)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(promotionType).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            return View(promotionType);
//        }

//        // POST: Admin/PromotionTypes/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            PromotionType promotionType = db.PromotionType.Find(id);
//            db.PromotionType.Remove(promotionType);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}