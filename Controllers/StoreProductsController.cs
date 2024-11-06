using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Filters;
using WebsiteBanDoAnVaThucUong.Models;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
    public class StoreProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [StoreSelectorFilter]
        public ActionResult Index(int? storeId, int? page)
        {
            if (!storeId.HasValue)
            {
                return RedirectToAction("Index", "Stores");
            }

            var storeProducts = db.StoreProducts
                .Where(sp => sp.StoreId == storeId)
                .Include(sp => sp.Product)
                .Include(sp => sp.Product.PromotionProduct.Select(pp => pp.Promotion))
                .OrderBy(sp => sp.Product.Title);

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.StoreName = db.Stores.Find(storeId)?.Name;
            ViewBag.StoreId = storeId;

            return View(storeProducts.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Edit(int storeId, int productId)
        {
            var storeProduct = db.StoreProducts
                .FirstOrDefault(sp => sp.StoreId == storeId && sp.ProductId == productId);

            if (storeProduct == null)
            {
                return HttpNotFound();
            }

            return View(storeProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StoreProduct storeProduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(storeProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { storeId = storeProduct.StoreId });
            }
            return View(storeProduct);
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