using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;
using Microsoft.AspNet.Identity;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Products
        public ActionResult Index()
        {
            var items = db.Products.ToList();

            return View(items);
        }

        public ActionResult Detail(string alias, int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                db.Products.Attach(item);
                item.ViewCount = item.ViewCount + 1;
                db.Entry(item).Property(x => x.ViewCount).IsModified = true;
                db.SaveChanges();
            }
            var countReview = db.Reviews.Where(x => x.ProductId == id).Count();
            ViewBag.CountReview = countReview;
            return View(item);
        }
        public ActionResult ProductCategory(string alias, int id)
        {
            var items = db.Products.ToList();
            if (id > 0)
            {
                items = items.Where(x => x.ProductCategoryId == id).ToList();
            }
            var cate = db.ProductCategories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Title;
            }

            ViewBag.CateId = id;
            return View(items);
        }

        public ActionResult Partial_ItemsByCateId()
        {
            var items = db.Products.Where(x => x.IsActive).Take(12).ToList();
            return PartialView(items);
        }

        public ActionResult Partial_ProductSales()
        {
            var products = db.Products.Where(x => x.IsActive).ToList();
            var filteredProducts = products.Where(product =>
               db.OrderDetails.Where(od => od.ProductId == product.Id).Sum(od => (int?)od.Quantity) > 10).ToList();
            return PartialView(filteredProducts);
        }
        // tìm kiếm dựa theo từ khóa 
        [HttpGet]
        public ActionResult Search(string searchString, int page = 1, int pageSize = 10)
        {
            var items = db.Products
                .Where(x => x.Alias.Contains(searchString) || x.Title.Contains(searchString))
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling((double)db.Products.Count(x => x.Alias.Contains(searchString) || x.Title.Contains(searchString)) / pageSize);

            return View(items);
        }

        //Tìm kiếm dựa vào từ khóa nhưng sẽ gợi ý thêm các sản phẩm liên quan sẽ mở ra view 
        [HttpGet]
        public JsonResult SearchSuggestions(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            // tối đa 5 sp getdata sau đó dropdown 
            var suggestions = db.Products
                .Where(x => x.Alias.Contains(searchString) || x.Title.Contains(searchString))
                .Select(x => new { x.Id, x.Title, x.Alias, x.SalePrice, x.ProductImage })
                .Take(5)
                .ToList();

            return Json(new { success = true, data = suggestions }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductSuggestions(string query)
        {
            var products = db.Products
                             .Where(p => p.Title.Contains(query) || p.Alias.Contains(query))
                             .Select(p => new
                             {
                                 p.Title,
                                 p.Id,
                                 p.Alias,
                                 ImageUrl = p.ProductImage.FirstOrDefault(x => x.IsDefault).Image,
                                 p.SalePrice
                             })
                             .Take(5) // giới hạn 5 sp 
                             .ToList();

            return Json(products, JsonRequestBehavior.AllowGet);
        }

        // Sản phẩm liên quan
        public ActionResult RelatedProducts(int productId)
        {
            var currentProduct = db.Products.Find(productId);
            if (currentProduct == null)
            {
                return HttpNotFound();
            }

            // Chia tiêu đề sản phẩm hiện tại thành các từ khóa
            var keywords = currentProduct.Title.Split(' ');

            // Tìm các sản phẩm có tiêu đề chứa các từ khóa này (loại bỏ sản phẩm hiện tại)
            var relatedProducts = db.Products
                                    .Where(p => p.Id != productId && keywords.Any(kw => p.Title.Contains(kw)))
                                    .Take(5)  // Giới hạn số lượng sản phẩm liên quan
                                    .ToList();

            return PartialView("_RelatedProducts", relatedProducts);
        }

    }
}


