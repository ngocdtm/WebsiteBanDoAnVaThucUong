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

namespace WebsiteBanDoAnVaThucUong.Areas.Admin.Controllers
{
    public class ToppingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Toppings
        public ActionResult Index()
        {
            return View(db.Topping.ToList());
        }

        // GET: Admin/Toppings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topping topping = db.Topping.Find(id);
            if (topping == null)
            {
                return HttpNotFound();
            }
            return View(topping);
        }

        // GET: Admin/Toppings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Toppings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NameTopping,PriceTopping")] Topping topping)
        {
            if (ModelState.IsValid)
            {
                db.Topping.Add(topping);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(topping);
        }

        // GET: Admin/Toppings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topping topping = db.Topping.Find(id);
            if (topping == null)
            {
                return HttpNotFound();
            }
            return View(topping);
        }

        // POST: Admin/Toppings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NameTopping,PriceTopping")] Topping topping)
        {
            if (ModelState.IsValid)
            {
                db.Entry(topping).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(topping);
        }

        // GET: Admin/Toppings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topping topping = db.Topping.Find(id);
            if (topping == null)
            {
                return HttpNotFound();
            }
            return View(topping);
        }

        // POST: Admin/Toppings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Topping topping = db.Topping.Find(id);
            db.Topping.Remove(topping);
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
