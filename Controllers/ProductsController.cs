using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;
using Microsoft.AspNet.Identity;
using WebsiteBanDoAnVaThucUong.Models.EF;
using PagedList;
using System.Data.Entity;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Products
        public ActionResult Index(int? storeId, int? page)
        {
            // Retrieve the selected store from either the parameter or the session
            if (storeId == null)
            {
                storeId = Session["SelectedStoreId"] as int?;
            }

            IQueryable<StoreProduct> storeProducts;

            if (storeId.HasValue)
            {
                // Filter products by storeId
                storeProducts = db.StoreProducts
                    .Where(sp => sp.StoreId == storeId)
                .Include(sp => sp.Product)
                .OrderBy(sp => sp.Product.Title);

                ViewBag.StoreName = db.Stores.Find(storeId)?.Name;
                ViewBag.StoreId = storeId;
                ViewBag.StoreSelected = true;

                // Store the storeId in the session for future use
                Session["SelectedStoreId"] = storeId.Value;
            }
            else
            {
                // If no store is selected, retrieve all products
                storeProducts = db.StoreProducts
                    .Include(sp => sp.Product)
                    .Include(sp => sp.Product.ProductImage)
                    .Include(sp => sp.Product.ProductCategory);

                ViewBag.StoreSelected = false;
            }

            // Apply ordering here before paging
            storeProducts = storeProducts.OrderBy(sp => sp.Product.Title); // Order by any field you prefer

            int pageSize = 6; // Show 6 products per page
            int pageNumber = (page ?? 1);

            return View(storeProducts.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Detail(string alias, int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                // Update the product's view count
                db.Products.Attach(item);
                item.ViewCount += 1;
                db.Entry(item).Property(x => x.ViewCount).IsModified = true;
                db.SaveChanges();
                // Save product to user's view history
                var userId = User.Identity.GetUserId(); // Assuming you're using Identity or any other user identification
                if (userId != null) // If the user is logged in
                {
                    var existingHistory = db.ProductViewHistory
                        .FirstOrDefault(h => h.ProductId == id && h.UserId == userId);
                    if (existingHistory == null) // Avoid duplicate entries in the history
                    {
                        var historyEntry = new ProductViewHistory
                        {
                            ProductId = id,
                            UserId = userId,
                            ViewedAt = DateTime.Now
                        };
                        db.ProductViewHistory.Add(historyEntry);
                        db.SaveChanges();
                    }
                }
                else
                {
                    // For guest users, store viewed product IDs in session
                    List<int> viewedProducts = Session["ViewHistory"] as List<int> ?? new List<int>();
                    if (!viewedProducts.Contains(id))
                    {
                        viewedProducts.Add(id);
                        Session["ViewHistory"] = viewedProducts;
                    }
                }
            }
            // Get the count of reviews
            var countReview = db.Reviews.Where(x => x.ProductId == id).Count();
            ViewBag.CountReview = countReview;
            // Return the product details view
            return View(item);
        }
        public ActionResult ViewHistory()
        {
            var userId = User.Identity.GetUserId();
            var history = db.ProductViewHistory
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.ViewedAt)
                .Take(5) // 5 hui
                .Select(h => h.Product) // Get the products
                .ToList();
            return View(history);
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

        public ActionResult Partial_ItemsByCateId(int? categoryId)
        {
            // Assuming you filter products by category and storeId
            var storeProducts = db.StoreProducts
                .Include(sp => sp.Product)
                .Include(sp => sp.Product.ProductImage)
                .Include(sp => sp.Product.ProductCategory);

            // Filter by categoryId if applicable
            if (categoryId.HasValue)
            {
                storeProducts = storeProducts.Where(sp => sp.Product.ProductCategoryId == categoryId);
            }

            // Group by ProductId to avoid duplicates
            var uniqueProducts = storeProducts
                .GroupBy(sp => sp.Product.Id)
                .Select(g => g.FirstOrDefault()) // Take only the first product per group
                .ToList();

            return PartialView(uniqueProducts);
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
