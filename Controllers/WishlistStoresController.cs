//using PagedList;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using WebsiteBanDoAnVaThucUong.Models.EF;
//using WebsiteBanDoAnVaThucUong.Models;

//namespace WebsiteBanDoAnVaThucUong.Controllers
//{
//    public class WishlistStoresController : Controller
//    {
//        // GET: Wishlist
//        public ActionResult Index(int? page)
//        {
//            var pageSize = 10;
//            if (page == null)
//            {
//                page = 1;
//            }
//            IEnumerable<WishlistStore> items = db.WishlistStores.Where(x => x.UserName == User.Identity.Name).OrderByDescending(x => x.CreatedDate);
//            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
//            items = items.ToPagedList(pageIndex, pageSize);
//            ViewBag.PageSize = pageSize;
//            ViewBag.Page = page;
//            return View(items);
//        }

//        [HttpPost]
//        [AllowAnonymous]
//        public ActionResult PostWishlist(int StoreId)
//        {
//            if (Request.IsAuthenticated == false)
//            {
//                return Json(new { Success = false, Message = "Bạn chưa đăng nhập. cua hang" });
//            }
//            var checkItem = db.WishlistStores.FirstOrDefault(x => x.StoreId == StoreId && x.UserName == User.Identity.Name);
//            if (checkItem != null)
//            {
//                return Json(new { Success = false, Message = "Cửa hàng đã được yêu thích rồi." });
//            }
//            var item = new WishlistStore();
//            item.StoreId = StoreId;
//            item.UserName = User.Identity.Name;
//            item.CreatedDate = DateTime.Now;
//            db.WishlistStores.Add(item);
//            db.SaveChanges();
//            return Json(new { Success = true });
//        }

//        [HttpPost]
//        [AllowAnonymous]
//        public ActionResult PostDeleteWishlist(int StoreId)
//        {
//            var checkItem = db.WishlistStores.FirstOrDefault(x => x.StoreId == StoreId && x.UserName == User.Identity.Name);
//            if (checkItem != null)
//            {
//                var item = db.WishlistStores.Find(checkItem.Id);
//                db.Set<WishlistStore>().Remove(item);
//                var i = db.SaveChanges();
//                return Json(new { Success = true, Message = "Xóa thành công." });
//            }
//            return Json(new { Success = false, Message = "Xóa thất bại." });
//        }

//        private ApplicationDbContext db = new ApplicationDbContext();
//        protected override void Dispose(bool disposing)
//        {
//            base.Dispose(disposing);
//        }
//    }
//}