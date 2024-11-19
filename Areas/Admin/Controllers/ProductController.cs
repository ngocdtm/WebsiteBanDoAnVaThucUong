using CKFinder.Settings;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Products
        public ActionResult Index(int? page)
        {
            IEnumerable<Product> items = db.Products.OrderByDescending(x => x.Id);
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }
       
        public ActionResult Add()
        {
            // Load dữ liệu cho các dropdown ngay từ đầu
            var model = new Product();
            LoadDropDownLists(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product model, List<string> Images, List<int> rDefault)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(model.Alias))
                        model.Alias = WebsiteBanDoAnVaThucUong.Models.Common.Filter.FilterChar(model.Title);

                    var category = db.ProductCategories.Find(model.ProductCategoryId);
                    if (category == null)
                    {
                        ModelState.AddModelError("", "Danh mục sản phẩm không tồn tại");
                        LoadDropDownLists(model);
                        return View(model);
                    }

                    // Set product type from category
                    model.ProductTypeId = category.ProductTypeId;

                    // Basic product setup
                    model.ViewCount = 0;
                    model.ProductCategory = category;

                    // Add product to database
                    db.Products.Add(model);
                    db.SaveChanges(); // Save to get the product ID

                    // Process images if provided
                    if (Images?.Any() == true && rDefault?.Any() == true)
                    {
                        ProcessProductImages(model, Images, rDefault[0]);
                    }

                    // Process relationships based on product type
                    if (category.ProductTypeId == 1) // Food
                    {
                        ProcessFoodExtras(model.Id, model.SelectedExtraIds);
                    }
                    else if (category.ProductTypeId == 2) // Beverage
                    {
                        ProcessBeverageToppingsAndSizes(model.Id, model.SelectedToppingIds, model.SelectedSizeIds);
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu sản phẩm: " + ex.Message);
                }
            }

            LoadDropDownLists(model);
            return View(model);
        }



        public ActionResult GetCategoriesByProductType(int productTypeId)
        {
            var categories = db.ProductCategories
                               .Where(c => c.ProductTypeId == productTypeId)
                               .Select(c => new { c.Id, c.Title })
                               .ToList();
            return Json(categories, JsonRequestBehavior.AllowGet);
 
        }

        private void LoadDropDownLists(Product model)
        {
            ViewBag.ProductTopping = new MultiSelectList(db.Topping.ToList(), "Id", "NameTopping", model.SelectedToppingIds);
            ViewBag.ProductSize = new MultiSelectList(db.Size.ToList(), "Id", "NameSize", model.SelectedSizeIds);
            ViewBag.ProductExtra = new MultiSelectList(db.Extra.ToList(), "Id", "Name", model.SelectedExtraIds);
            // Load ProductType
            ViewBag.ProductType = new SelectList(
                db.ProductType.Select(x => new { Id = x.Id, Name = x.Name }),
                "Id",
                "Name",
                model.ProductTypeId);

            // Load Categories based on ProductType if selected
            var categories = model.ProductTypeId.HasValue
                ? db.ProductCategories.Where(c => c.ProductTypeId == model.ProductTypeId)
                : db.ProductCategories;

            ViewBag.ProductCategory = new SelectList(
                categories.Select(x => new { Id = x.Id, Title = x.Title }),
                "Id",
                "Title",
                model.ProductCategoryId);

            // Initialize the selected lists if they're null
            model.SelectedToppingIds = model.SelectedToppingIds ?? new List<int>();
            model.SelectedSizeIds = model.SelectedSizeIds ?? new List<int>();
            model.SelectedExtraIds = model.SelectedExtraIds ?? new List<int>();

            if (model.ProductTypeId == 2) // Beverages
            {
                // Create MultiSelectList for toppings
                ViewBag.ProductTopping = new MultiSelectList(
                    db.Topping.Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.NameTopping
                    }),
                    "Value",
                    "Text",
                    model.SelectedToppingIds);

                // Create MultiSelectList for sizes
                ViewBag.ProductSize = new MultiSelectList(
                    db.Size.Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.NameSize
                    }),
                    "Value",
                    "Text",
                    model.SelectedSizeIds);
            }
            else if (model.ProductTypeId == 1) // Food
            {
                // Create MultiSelectList for extras
                ViewBag.ProductExtra = new MultiSelectList(
                    db.Extra.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Name
                    }),
                    "Value",
                    "Text",
                    model.SelectedExtraIds);
            }
        }
       
        public JsonResult GetToppings()
        {
            var toppings = db.Topping.Select(t => new
            {
                value = t.Id,
                text = t.NameTopping
            }).ToList();

            return Json(toppings, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSizes()
        {
            var sizes = db.Size.Select(s => new
            {
                value = s.Id,
                text = s.NameSize
            }).ToList();

            return Json(sizes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExtras()
        {
            var extras = db.Extra.Select(e => new
            {
                value = e.Id,
                text = e.Name
            }).ToList();

            return Json(extras, JsonRequestBehavior.AllowGet);
        }
        private void ProcessFoodExtras(int productId, List<int> selectedExtras)
        {
            if (selectedExtras?.Any() == true)
            {
                // First remove existing relationships
                var existingExtras = db.ProductExtra.Where(pe => pe.ProductId == productId);
                db.ProductExtra.RemoveRange(existingExtras);

                // Add new relationships
                foreach (var extraId in selectedExtras)
                {
                    var productExtra = new ProductExtra
                    {
                        ProductId = productId,
                        ExtraId = extraId
                    };
                    db.ProductExtra.Add(productExtra);
                }
                db.SaveChanges();
            }
        }
        private void ProcessBeverageToppingsAndSizes(int productId, List<int> selectedToppings, List<int> selectedSizes)
        {
            // Process Toppings
            if (selectedToppings?.Any() == true)
            {
                // Remove existing topping relationships
                var existingToppings = db.ProductTopping.Where(pt => pt.ProductId == productId);
                db.ProductTopping.RemoveRange(existingToppings);

                // Add new topping relationships
                foreach (var toppingId in selectedToppings)
                {
                    var productTopping = new ProductTopping
                    {
                        ProductId = productId,
                        ToppingId = toppingId,
                        IsRecommended = false, // Set default value
                        SpecialPrice = 0 // Set default value
                    };
                    db.ProductTopping.Add(productTopping);
                }
            }

            // Process Sizes
            if (selectedSizes?.Any() == true)
            {
                // Remove existing size relationships
                var existingSizes = db.ProductSize.Where(ps => ps.ProductId == productId);
                db.ProductSize.RemoveRange(existingSizes);

                // Add new size relationships
                foreach (var sizeId in selectedSizes)
                {
                    var productSize = new ProductSize
                    {
                        ProductId = productId,
                        SizeId = sizeId
                    };
                    db.ProductSize.Add(productSize);
                }
            }

            if (selectedToppings?.Any() == true || selectedSizes?.Any() == true)
            {
                db.SaveChanges();
            }
        }

       
        private void ProcessProductImages(Product model, List<string> images, int defaultImageIndex)
        {
            for (int i = 0; i < images.Count; i++)
            {
                bool isDefault = (i + 1 == defaultImageIndex);
                if (isDefault)
                {
                    model.Image = images[i];
                    db.Entry(model).State = EntityState.Modified;
                }

                db.ProductImages.Add(new ProductImage
                {
                    ProductId = model.Id,
                    Image = images[i],
                    IsDefault = isDefault
                });
            }
            db.SaveChanges();
        }
        public ActionResult Edit(int id)
        {
            var product = db.Products
                .Include(x => x.ProductCategory)
                .Include(x => x.ProductTopping.Select(ps => ps.Topping))
                .Include(p => p.ProductSize.Select(ps => ps.Size))
                .Include(p => p.ProductExtra.Select(pe => pe.Extra))
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            // Đảm bảo set ProductTypeId
            product.ProductTypeId = product.ProductCategory.ProductTypeId;

            // Load các selected items
            product.SelectedToppingIds = product.ProductTopping?.Select(pt => pt.ToppingId).ToList() ?? new List<int>();
            product.SelectedSizeIds = product.ProductSize?.Select(ps => ps.SizeId).ToList() ?? new List<int>();
            product.SelectedExtraIds = product.ProductExtra?.Select(pe => pe.ExtraId).ToList() ?? new List<int>();

            LoadDropDownLists(product);
            return View(product);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model, List<string> Images, List<int> rDefault)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var existingProduct = db.Products
                            .Include(p => p.ProductCategory)
                            .Include(p => p.ProductTopping)
                            .Include(p => p.ProductSize)
                            .Include(p => p.ProductExtra)
                            .Include(p => p.ProductImage)
                            .FirstOrDefault(p => p.Id == model.Id);

                        if (existingProduct == null)
                        {
                            return HttpNotFound();
                        }

                        // Update basic properties
                        existingProduct.Title = model.Title;
                        existingProduct.Alias = WebsiteBanDoAnVaThucUong.Models.Common.Filter.FilterChar(model.Title);
                        existingProduct.ProductCategoryId = model.ProductCategoryId;
                        existingProduct.Description = model.Description;
                        existingProduct.Detail = model.Detail;
                        existingProduct.SalePrice = model.SalePrice;
                        existingProduct.OriginalPrice = model.OriginalPrice;
                        existingProduct.Quantity = model.Quantity;
                        existingProduct.IsActive = model.IsActive;

                        // Get product type from category
                        var category = db.ProductCategories.Find(model.ProductCategoryId);
                        if (category == null)
                        {
                            ModelState.AddModelError("", "Danh mục sản phẩm không tồn tại");
                            LoadDropDownLists(model);
                            return View(model);
                        }
                        existingProduct.ProductTypeId = category.ProductTypeId;
                        // Process images if provided
                        if (Images?.Any() == true && rDefault?.Any() == true)
                        {
                            // Remove existing images
                            var existingImages = db.ProductImages.Where(pi => pi.ProductId == existingProduct.Id);
                            db.ProductImages.RemoveRange(existingImages);

                            // Add new images
                            ProcessProductImages(existingProduct, Images, rDefault[0]);
                        }
                        // Clear existing relationships
                        ClearExistingRelationships(existingProduct);

                        // Save changes after removing relationships
                        db.SaveChanges();

                        // Update based on product type
                        if (category.ProductTypeId == 1) // Food
                        {
                            ProcessFoodExtras(existingProduct.Id, model.SelectedExtraIds);
                        }
                        else if (category.ProductTypeId == 2) // Beverage
                        {
                            ProcessBeverageToppingsAndSizes(existingProduct.Id, model.SelectedToppingIds, model.SelectedSizeIds);
                        }
                    
                        db.SaveChanges();
                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                    }

                }
            }

            LoadDropDownLists(model);
            return View(model);
        }

        private void ClearExistingRelationships(Product existingProduct)
        {
            // Clear Toppings
            if (existingProduct.ProductTopping != null)
            {
                var existingToppings = existingProduct.ProductTopping.ToList();
                db.ProductTopping.RemoveRange(existingToppings);
            }

            // Clear Sizes
            if (existingProduct.ProductSize != null)
            {
                var existingSizes = existingProduct.ProductSize.ToList();
                db.ProductSize.RemoveRange(existingSizes);
            }

            // Clear Extras
            if (existingProduct.ProductExtra != null)
            {
                var existingExtras = existingProduct.ProductExtra.ToList();
                db.ProductExtra.RemoveRange(existingExtras);
            }

            db.SaveChanges();
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                var checkImg = item.ProductImage.Where(x => x.ProductId == item.Id);
                if (checkImg != null)
                {
                    foreach (var img in checkImg)
                    {
                        db.ProductImages.Remove(img);
                        db.SaveChanges();
                    }
                }
                db.Products.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.Products.Find(id);
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