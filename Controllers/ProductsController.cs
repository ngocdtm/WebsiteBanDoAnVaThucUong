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
using WebsiteBanDoAnVaThucUong.Filters;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Products
        [StoreSelectorFilter]
        public ActionResult Index(int? storeId, int? page)
        {
            // Retrieve the selected store from either the parameter or the session
            if (storeId.HasValue)
            {
                Session["SelectedStoreId"] = storeId.Value;
            }
            else
            {
                storeId = Session["SelectedStoreId"] as int?;
                ViewBag.StoreRequiredMessage = "Vui lòng chọn cửa hàng trước khi xem chi tiết sản phẩm.";
            }

            IQueryable<StoreProduct> storeProducts;

            if (storeId.HasValue)
            {
                // Filter products by storeId
                storeProducts = db.StoreProducts
                    .Where(sp => sp.StoreId == storeId)
                    .Include(sp => sp.Product)
                    .Include(sp => sp.Product.ProductImage)
                    .Include(sp => sp.Product.ProductCategory)
                    .OrderBy(sp => sp.Product.Title);

                var store = db.Stores.Find(storeId);
                if (store != null)
                {
                    ViewBag.StoreName = store.Name;
                    ViewBag.StoreId = storeId;
                    ViewBag.StoreSelected = true;
                }
            }
            else
            {
                // If no store is selected, retrieve all products
                storeProducts = db.StoreProducts
                    .Include(sp => sp.Product)
                    .Include(sp => sp.Product.ProductImage)
                    .Include(sp => sp.Product.ProductCategory)
                    .OrderBy(sp => sp.Product.Title);

                ViewBag.StoreSelected = false;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);

            return View(storeProducts.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Detail(string alias, int id)
        {
            try
            {
                // Check if a store has been selected
                var storeId = Session["SelectedStoreId"] as int?;
                if (!storeId.HasValue)
                {
                    TempData["Message"] = "Vui lòng chọn cửa hàng trước khi xem chi tiết sản phẩm.";
                    return RedirectToAction("Index");
                }

                // Load product details with eager loading
                var item = db.StoreProducts
                    .Include(sp => sp.Product.ProductCategory)
                    .Include(sp => sp.Product.ProductImage)
                    .Include(sp => sp.Product.ProductTopping)
                    .Include(sp => sp.Product.ProductSize)
                    .Include(sp => sp.Product.ProductExtra)
                    .FirstOrDefault(sp => sp.ProductId == id && sp.StoreId == storeId);

                if (item == null)
                {
                    return HttpNotFound("Sản phẩm không có sẵn trong cửa hàng này.");
                }

                // Update view count
                UpdateViewCount(id);

                // Save to view history
                SaveToViewHistory(id);

                // Get review count
                ViewBag.CountReview = db.Reviews.Count(x => x.ProductId == id);

                return View(item);
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error in Detail action: {ex.Message}");
                throw;
            }
        }
        private void UpdateViewCount(int productId)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // Use ExecuteSqlCommand for direct update
                    context.Database.ExecuteSqlCommand(
                        "UPDATE Products SET ViewCount = ViewCount + 1 WHERE Id = @p0",
                        productId
                    );
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't throw it since view count is not critical
                System.Diagnostics.Debug.WriteLine($"Error updating view count: {ex.Message}");
            }
        }
        private void SaveToViewHistory(int productId)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                if (!string.IsNullOrEmpty(userId))
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var existingHistory = context.ProductViewHistory
                            .FirstOrDefault(h => h.ProductId == productId && h.UserId == userId);

                        if (existingHistory == null)
                        {
                            var historyEntry = new ProductViewHistory
                            {
                                ProductId = productId,
                                UserId = userId,
                                ViewedAt = DateTime.Now
                            };

                            context.ProductViewHistory.Add(historyEntry);
                            context.SaveChanges();
                        }
                        else
                        {
                            // Update existing entry's timestamp using SQL
                            context.Database.ExecuteSqlCommand(
                                "UPDATE ProductViewHistory SET ViewedAt = @p0 WHERE ProductId = @p1 AND UserId = @p2",
                                DateTime.Now, productId, userId
                            );
                        }
                    }
                }
                else
                {
                    // For guest users
                    var viewedProducts = Session["ViewHistory"] as List<int> ?? new List<int>();
                    if (!viewedProducts.Contains(productId))
                    {
                        viewedProducts.Add(productId);
                        Session["ViewHistory"] = viewedProducts;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't throw it since view history is not critical
                System.Diagnostics.Debug.WriteLine($"Error saving view history: {ex.Message}");
            }
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
            var storeId = Session["SelectedStoreId"] as int?;
            // Filter by store if selected

            var items = db.StoreProducts.Include(sp => sp.Product)
                .Include(sp => sp.Product.ProductImage)
                .Include(sp => sp.Product.ProductCategory);
            if (id > 0)
            {
                items = items.Where(x => x.Product.ProductCategoryId == id);
            }
            var cate = db.ProductCategories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Title;
            }

            ViewBag.CateId = id;
            // Group by ProductId to avoid duplicates
            var uniqueProducts = items
                .GroupBy(sp => sp.Product.Id)
                .Select(g => g.FirstOrDefault())
                .ToList();
            // Pass stock information to view
            var stockInfo = uniqueProducts.ToDictionary(
            sp => sp.ProductId,
            sp => new StockInfo
            {
                StockCount = sp.StockCount,
                IsSoldOut = sp.StockCount <= 0
            }
            );

            ViewBag.StockInfo = stockInfo;
            return View(uniqueProducts);
        }

        public ActionResult Partial_ItemsByCateId(int? categoryId)
        {
            var storeId = Session["SelectedStoreId"] as int?;
            var storeProducts = db.StoreProducts
                .Include(sp => sp.Product)
                .Include(sp => sp.Product.ProductImage)
                .Include(sp => sp.Product.ProductCategory);

            // Filter by store if selected
            if (storeId.HasValue)
            {
                storeProducts = storeProducts.Where(sp => sp.StoreId == storeId);
            }

            // Filter by categoryId if applicable
            if (categoryId.HasValue)
            {
                storeProducts = storeProducts.Where(sp => sp.Product.ProductCategoryId == categoryId);
            }

            // Group by ProductId to avoid duplicates
            var uniqueProducts = storeProducts
                .GroupBy(sp => sp.Product.Id)
                .Select(g => g.FirstOrDefault())
                .ToList();
            // Pass stock information to view
            var stockInfo = uniqueProducts.ToDictionary(
            sp => sp.ProductId,
            sp => new StockInfo
            {
                StockCount = sp.StockCount,
                IsSoldOut = sp.StockCount <= 0
            }
            );

            ViewBag.StockInfo = stockInfo;

            return PartialView(uniqueProducts);
        }
        // Add method to check stock before adding to cart
        [HttpPost]
        public JsonResult CheckStockAvailability(int productId)
        {
            var storeId = Session["SelectedStoreId"] as int?;
            if (!storeId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng chọn cửa hàng" });
            }

            var storeProduct = db.StoreProducts
                .FirstOrDefault(sp => sp.ProductId == productId && sp.StoreId == storeId);

            if (storeProduct == null)
            {
                return Json(new { success = false, message = "Sản phẩm không có sẵn tại cửa hàng này" });
            }

            if (storeProduct.StockCount <= 0)
            {
                return Json(new { success = false, message = "Sản phẩm đã hết hàng" });
            }

            return Json(new { success = true, stockCount = storeProduct.StockCount });
        }

        [HttpPost]
        public JsonResult SelectStore(int storeId)
        {
            try
            {
                Session["SelectedStoreId"] = storeId;
                var store = db.Stores.Find(storeId);
                return Json(new { success = true, storeName = store.Name });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
