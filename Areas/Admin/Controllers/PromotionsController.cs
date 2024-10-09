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
using WebsiteBanDoAnVaThucUong.Models.ViewModels;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace WebsiteBanDoAnVaThucUong.Areas.Admin.Controllers
{
    public class PromotionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Promotions
        public ActionResult Index()
        {
            return View(db.Promotions.ToList());
        }

        // GET: Admin/Promotions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promotion promotion = db.Promotions.Find(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            return View(promotion);
        }

        // GET: Admin/Promotions/Create
        public ActionResult Create()
        {
            ViewBag.Products = new MultiSelectList(db.Products, "Id", "Title");
            return View();
        }

        // POST: Admin/Promotions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,DiscountType,DiscountValue,ComboQuantity,BuyQuantity,GetQuantity,IsActive")] Promotion promotion, int[] selectedProducts, int[] selectedBuyProducts, int[] selectedGetProducts)
        {
            if (ModelState.IsValid)
            {
                db.Promotions.Add(promotion);
                db.SaveChanges();

                if (promotion.DiscountType == 1 || promotion.DiscountType == 2) // Percentage or Combo
                {
                    if (selectedProducts != null)
                    {
                        foreach (var productId in selectedProducts)
                        {
                            db.PromotionProducts.Add(new PromotionProduct { PromotionId = promotion.Id, ProductId = productId });
                        }
                    }
                }
                else if (promotion.DiscountType == 3) // Buy X Get Y
                {
                    if (selectedBuyProducts != null && selectedGetProducts != null)
                    {
                        for (int i = 0; i < selectedBuyProducts.Length; i++)
                        {
                            db.PromotionProducts.Add(new PromotionProduct { PromotionId = promotion.Id, ProductId = selectedBuyProducts[i], IsBuyProduct = true });
                        }
                        for (int i = 0; i < selectedGetProducts.Length; i++)
                        {
                            db.PromotionProducts.Add(new PromotionProduct { PromotionId = promotion.Id, ProductId = selectedGetProducts[i], IsBuyProduct = false });
                        }
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Products = new MultiSelectList(db.Products, "Id", "Title", selectedProducts);
            return View(promotion);
        }

        // GET: Admin/Promotions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promotion promotion = db.Promotions.Find(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            var allProducts = db.Products.ToList();
            ViewBag.Products = new MultiSelectList(allProducts, "Id", "Title", promotion.PromotionProduct.Where(pp => promotion.DiscountType != 3 || pp.IsBuyProduct).Select(pp => pp.ProductId));
            ViewBag.BuyProducts = new MultiSelectList(allProducts, "Id", "Title", promotion.PromotionProduct.Where(pp => promotion.DiscountType == 3 && pp.IsBuyProduct).Select(pp => pp.ProductId));
            ViewBag.GetProducts = new MultiSelectList(allProducts, "Id", "Title", promotion.PromotionProduct.Where(pp => promotion.DiscountType == 3 && !pp.IsBuyProduct).Select(pp => pp.ProductId));
            return View(promotion);
        }

        // POST: Admin/Promotions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,DiscountType,DiscountValue,ComboQuantity,BuyQuantity,GetQuantity,IsActive")] Promotion promotion, int[] selectedProducts, int[] selectedBuyProducts, int[] selectedGetProducts)
        {
            if (ModelState.IsValid)
            {
                db.Entry(promotion).State = EntityState.Modified;

                var existingPromotionProducts = db.PromotionProducts.Where(pp => pp.PromotionId == promotion.Id).ToList();
                db.PromotionProducts.RemoveRange(existingPromotionProducts);

                if (promotion.DiscountType == 1 || promotion.DiscountType == 2) // Percentage or Combo
                {
                    if (selectedProducts != null)
                    {
                        foreach (var productId in selectedProducts)
                        {
                            db.PromotionProducts.Add(new PromotionProduct { PromotionId = promotion.Id, ProductId = productId });
                        }
                    }
                }
                else if (promotion.DiscountType == 3) // Buy X Get Y
                {
                    if (selectedBuyProducts != null && selectedGetProducts != null)
                    {
                        for (int i = 0; i < selectedBuyProducts.Length; i++)
                        {
                            db.PromotionProducts.Add(new PromotionProduct { PromotionId = promotion.Id, ProductId = selectedBuyProducts[i], IsBuyProduct = true });
                        }
                        for (int i = 0; i < selectedGetProducts.Length; i++)
                        {
                            db.PromotionProducts.Add(new PromotionProduct { PromotionId = promotion.Id, ProductId = selectedGetProducts[i], IsBuyProduct = false });
                        }
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Products = new MultiSelectList(db.Products, "Id", "Title", selectedProducts);
            ViewBag.BuyProducts = new MultiSelectList(db.Products, "Id", "Title", selectedBuyProducts);
            ViewBag.GetProducts = new MultiSelectList(db.Products, "Id", "Title", selectedGetProducts);
            return View(promotion);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Promotions.Find(id);
            if (item != null)
            {
                db.Promotions.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.Promotions.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isAcive = item.IsActive });
            }

            return Json(new { success = false });
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
