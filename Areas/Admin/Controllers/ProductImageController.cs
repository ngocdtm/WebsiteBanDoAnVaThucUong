using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models.EF;
using WebsiteBanDoAnVaThucUong.Models;

namespace WebsiteBanDoAnVaThucUong.Areas.Admin.Controllers
{
    public class ProductImageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/ProductImage
        public ActionResult Index(int id)
        {
            ViewBag.ProductId = id;
            var items = db.ProductImages.Where(x => x.ProductId == id).ToList();
            return View(items);
        }

        [HttpPost]
        public ActionResult AddImage(int productId, string url)
        {
            // Kiểm tra xem đã có ảnh nào chưa
            var existingImages = db.ProductImages.Where(x => x.ProductId == productId).ToList();

            db.ProductImages.Add(new ProductImage
            {
                ProductId = productId,
                Image = url,
                IsDefault = !existingImages.Any() // Nếu chưa có ảnh nào thì ảnh mới sẽ là default
           
        });
            db.SaveChanges();
            return Json(new { Success = true });
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var item = db.ProductImages.Find(id);
                    if (item == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy ảnh" });
                    }

                    bool wasDefault = item.IsDefault;
                    var productId = item.ProductId;

                    // Xóa ảnh hiện tại
                    db.ProductImages.Remove(item);
                    db.SaveChanges();

                    // Nếu ảnh vừa xóa là ảnh mặc định, set ảnh tiếp theo làm mặc định
                    if (wasDefault)
                    {
                        var nextImage = db.ProductImages
                            .Where(x => x.ProductId == productId)
                            .FirstOrDefault();

                        if (nextImage != null)
                        {
                            nextImage.IsDefault = true;
                            db.SaveChanges();
                        }
                    }

                    transaction.Commit();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }
        [HttpPost]
        public ActionResult DeleteAll(int productId)
        {
            try
            {
                var items = db.ProductImages.Where(x => x.ProductId == productId);
                db.ProductImages.RemoveRange(items);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult SetDefault(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Lấy ảnh được chọn
                    var image = db.ProductImages.Find(id);
                    if (image == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy ảnh" });
                    }

                    // Lấy tất cả ảnh của sản phẩm
                    var allProductImages = db.ProductImages.Where(x => x.ProductId == image.ProductId);

                    // Set tất cả ảnh về false
                    foreach (var img in allProductImages)
                    {
                        img.IsDefault = false;
                    }

                    // Set ảnh được chọn thành default
                    image.IsDefault = true;

                    db.SaveChanges();
                    transaction.Commit();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }
    }

}