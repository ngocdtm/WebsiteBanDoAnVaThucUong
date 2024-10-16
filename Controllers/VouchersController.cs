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
                    .FirstOrDefault();

                if (activeVoucher != null)
                {
                    Debug.WriteLine("GetActiveVoucher action called");
                    Debug.WriteLine($"Active voucher found: {activeVoucher.VoucherName}");
                    return Json(new
                    {
                        voucherName = activeVoucher.VoucherName,
                        coupon = activeVoucher.Coupon,
                        description = activeVoucher.VoucherDes,
                        discount = activeVoucher.Discount
                    }, JsonRequestBehavior.AllowGet);
                }

                Debug.WriteLine("No active voucher found");
                return Json(new { message = "No active voucher found" }, JsonRequestBehavior.AllowGet);
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

