using Microsoft.AspNet.Identity;
using PagedList;
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
    public class VouchersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Vouchers
        public ActionResult Index(int? page)
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
                    IEnumerable<Voucher> items = db.Vouchers.OrderByDescending(x => x.Id);
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
        //public ActionResult Index()
        //{
        //    var vouchers = db.Vouchers.Include(v => v.User);
        //    return View(vouchers.ToList());
        //}

        // GET: Admin/Vouchers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voucher voucher = db.Vouchers.Find(id);
            if (voucher == null)
            {
                return HttpNotFound();
            }
            return View(voucher);
        }

        // GET: Admin/Vouchers/Create
        public ActionResult Create()
        {
            //ViewBag.CreatedBy = new SelectList(db.ApplicationUsers, "Id", "FullName");
            return View();
        }

        // POST: Admin/Vouchers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,VoucherName,Coupon,VoucherDes,Discount,StartDate,EndDate,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy")] Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                //voucher.CreatedDate = DateTime.Now;
                //voucher.ModifiedDate = DateTime.Now;
                //voucher.CreatedBy = User.Identity.GetUserId();
                //voucher.ModifiedBy = User.Identity.GetUserId();
                db.Vouchers.Add(voucher);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.CreatedBy = new SelectList(db.ApplicationUsers, "Id", "FullName", voucher.CreatedBy);
            return View(voucher);
        }

        // GET: Admin/Vouchers/Edit/5
        public ActionResult Edit(int id)
        {
            var item = db.Vouchers.Find(id);
            return View(item);
        }

        // POST: Admin/Vouchers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Voucher model)
        {
            if (ModelState.IsValid)
            {
                var existingVou = db.Vouchers.Find(model.Id);
                if (existingVou != null)
                {
                    existingVou.VoucherName = model.VoucherName;
                    existingVou.VoucherDes = model.VoucherDes;
                    existingVou.Coupon = model.Coupon;
                    existingVou.Discount = model.Discount;
                    existingVou.StartDate = model.StartDate;
                    existingVou.EndDate = model.EndDate;

                    db.Entry(existingVou).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            //ViewBag.CreatedBy = new SelectList(db.ApplicationUsers, "Id", "FullName", voucher.CreatedBy);
            return View(model);
        }

        // GET: Admin/Vouchers/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Voucher voucher = db.Vouchers.Find(id);
        //    if (voucher == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(voucher);
        //}
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Vouchers.Find(id);
            if (item != null)
            {
                db.Vouchers.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        //// POST: Admin/Vouchers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Voucher voucher = db.Vouchers.Find(id);
        //    db.Vouchers.Remove(voucher);
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
