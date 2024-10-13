using PagedList;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Areas.Admin.Controllers
{
    public class StoreProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? storeId, int? page)
        {
            if (!storeId.HasValue)
            {
                return RedirectToAction("Index", "Stores");
            }

            var store = db.Stores.Find(storeId);
            if (store == null)
            {
                return HttpNotFound();
            }

            var storeProducts = db.StoreProducts
                .Where(sp => sp.StoreId == storeId)
                .Include(sp => sp.Product)
                .OrderBy(sp => sp.Product.Title);

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.StoreName = store.Name;
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
