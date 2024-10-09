using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models.EF;
using WebsiteBanDoAnVaThucUong.Models;
using Microsoft.AspNet.Identity;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Wishlist
        public ActionResult Index(int? page)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<Wishlist> items = db.Wishlists.AsEnumerable().Where(x => x.CustomerId == User.Identity.GetUserId()).OrderByDescending(x => x.CreatedDate);
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }

        [HttpPost]
        public ActionResult AddToWishlist(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm vào danh sách yêu thích." });
            }

            var wishlist = new Wishlist
            {
                ProductId = productId,
                CustomerId = User.Identity.GetUserId(),
                CreatedDate = DateTime.Now
            };

            db.Wishlists.Add(wishlist);
            db.SaveChanges();

            return Json(new { success = true, message = "Đã thêm sản phẩm vào danh sách yêu thích." });
        }

        [HttpPost]
        public ActionResult RemoveFromWishlist(int productId)
        {
            var userId = User.Identity.GetUserId();
            var wishlist = db.Wishlists.FirstOrDefault(x => x.ProductId == productId && x.CustomerId == userId);

            if (wishlist != null)
            {
                db.Wishlists.Remove(wishlist);
                db.SaveChanges();
                return Json(new { success = true, message = "Đã xóa sản phẩm khỏi danh sách yêu thích." });
            }
            return Json(new { success = false, message = "Không tìm thấy sản phẩm trong danh sách yêu thích." });
        }

        [HttpPost]
        public ActionResult RemoveFromWishlist1(int id)
        {
            var userId = User.Identity.GetUserId();
            var wishlist = db.Wishlists.FirstOrDefault(x => x.Id == id && x.CustomerId == userId);
            if (wishlist != null)
            {
                db.Wishlists.Remove(wishlist);
                db.SaveChanges();
                return Json(new { success = true, message = "Đã xóa sản phẩm khỏi danh sách yêu thích." });
            }
            return Json(new { success = false, message = "Không tìm thấy sản phẩm trong danh sách yêu thích." });
        }

    }
}