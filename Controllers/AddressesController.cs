using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
    public class AddressesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly HttpClient _httpClient;

        public AddressesController()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.BaseAddress = new Uri("https://provinces.open-api.vn/api/?depth=2");
        }

        [HttpGet]
        public async Task<JsonResult> GetProvinces()
        {
            try
            {
                var response = await _httpClient.GetAsync("p/");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Json(content, JsonRequestBehavior.AllowGet);
                }
                return Json(new { error = "Failed to fetch provinces" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetDistricts(string code)
        {
            try
            {
                var response = await _httpClient.GetAsync($"p/{code}?depth=2");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Json(content, JsonRequestBehavior.AllowGet); // Trả về string JSON trực tiếp
                }
                return Json(new { error = "Failed to fetch districts" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetWards(string code) // Thay đổi kiểu tham số từ int sang string
        {
            try
            {
                var response = await _httpClient.GetAsync($"d/{code}?depth=2");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Json(content, JsonRequestBehavior.AllowGet);
                }
                return Json(new { error = "Failed to fetch wards" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult SaveCustomerAddress(string province, string district, string ward, string addressLine)
        {
            // Save the customer address to the session
            Session["CustomerAddress"] = new Address
            {
                ProvinceName = province,
                DistrictName = district,
                WardName = ward,
                StreetAddress = addressLine
            };

            return Json(new { success = true });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}