using System;
using System.Linq;
using System.Web.Mvc;
using System.Diagnostics;
using WebsiteBanDoAnVaThucUong.Models;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
    public class VouchersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult GetActiveVoucher()
        {
            Debug.WriteLine("GetActiveVoucher action called");
            try
            {
                var currentDate = DateTime.Now;
                var activeVoucher = db.Vouchers
                    .Where(v => v.IsActive && v.StartDate <= currentDate && v.EndDate >= currentDate)
                    .OrderByDescending(v => v.Discount)
                    .Select(v => new
                    {
                        voucherName = v.VoucherName,
                        coupon = v.Coupon,
                        description = v.VoucherDes,
                        discount = v.Discount
                    }).ToList();
                return Json(activeVoucher, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetActiveVoucher: {ex.Message}");
                return Json(new { error = "An error occurred while fetching the voucher" }, JsonRequestBehavior.AllowGet);
            }
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


