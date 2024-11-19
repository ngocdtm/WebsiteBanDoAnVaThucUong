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

            public ActionResult Index(int? storeId, int? page)
            {
                // Retrieve the customer address from the session
                var customerAddress = (Address)Session["CustomerAddress"];
                if (customerAddress == null)
                {
                    // Handle the case when the customer address is not available
                    ViewBag.StoreRequiredMessage = "Vui lòng cập nhật địa chỉ của bạn trước khi xem sản phẩm.";
                    return View(new PagedList<StoreProduct>(new List<StoreProduct>(), 1, 10));
                }

                if (!storeId.HasValue)
                {
                    return RedirectToAction("Index", "Stores");
                }

                var store = db.Stores.Find(storeId);
                if (store == null)
                {
                    // Handle the case when the store is not found
                    ViewBag.StoreRequiredMessage = "Vui lòng chọn cửa hàng trước khi xem sản phẩm.";
                    return View(new PagedList<StoreProduct>(new List<StoreProduct>(), 1, 10));
                }
                var storeProducts = db.StoreProducts
                     .Where(sp => sp.StoreId == storeId)
                     .Include(sp => sp.Product)
                     .Include(sp => sp.Product.PromotionProduct.Select(pp => pp.Promotion))
                     .OrderBy(sp => sp.Product.Title);

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                ViewBag.StoreName = store.Name;
                ViewBag.StoreId = storeId;

                // Calculate the shipping fee based on the customer's and store's locations
                decimal shippingFee = CalculateShippingFee(customerAddress, store);

                // Pass the store products and the shipping fee to the view
                var viewModel = new StoreProductViewModel
                {
                    StoreProducts = storeProducts.ToPagedList(pageNumber, pageSize),
                    CustomerAddress = customerAddress,
                    ShippingFee = shippingFee
                };
                return View(viewModel);
            }

        private decimal CalculateShippingFee(Address customerAddress, Store store)
        {
            // Retrieve the shipping fee settings from the database
            var shippingFeeSettings = db.ShippingFee.FirstOrDefault();

            // Calculate the distance between the customer and the store
            double distance = CalculateDistance((double)store.Lat, (double)store.Long, (double)customerAddress.Latitude, (double)customerAddress.Longitude);

            // Calculate the shipping fee based on the distance and the shipping fee settings
            decimal shippingFee = shippingFeeSettings.FeePerKm * (decimal)distance;
            shippingFee = Math.Max(shippingFee, shippingFeeSettings.MinimumFee);

            return shippingFee;
        }


        private static readonly double EarthRadiusKm = 6371;
        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
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