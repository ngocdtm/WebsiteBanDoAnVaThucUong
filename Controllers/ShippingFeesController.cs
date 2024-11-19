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
    public class ShippingFeesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShippingFees
        public ActionResult Index()
        {
            return View(db.ShippingFee.ToList());
        }

        // GET: ShippingFees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingFee shippingFee = db.ShippingFee.Find(id);
            if (shippingFee == null)
            {
                return HttpNotFound();
            }
            return View(shippingFee);
        }

        // GET: ShippingFees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShippingFees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FeePerKm,MinimumFee")] ShippingFee shippingFee)
        {
            if (ModelState.IsValid)
            {
                db.ShippingFee.Add(shippingFee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shippingFee);
        }

        // GET: ShippingFees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingFee shippingFee = db.ShippingFee.Find(id);
            if (shippingFee == null)
            {
                return HttpNotFound();
            }
            return View(shippingFee);
        }

        // POST: ShippingFees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FeePerKm,MinimumFee")] ShippingFee shippingFee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shippingFee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shippingFee);
        }

        // GET: ShippingFees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingFee shippingFee = db.ShippingFee.Find(id);
            if (shippingFee == null)
            {
                return HttpNotFound();
            }
            return View(shippingFee);
        }

        // POST: ShippingFees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ShippingFee shippingFee = db.ShippingFee.Find(id);
            db.ShippingFee.Remove(shippingFee);
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
