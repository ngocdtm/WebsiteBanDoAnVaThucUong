using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;
using WebsiteBanDoAnVaThucUong.Models.EF;
using WebsiteBanDoAnVaThucUong.Filters;
using System.Globalization;
using System.Text;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
   
    public class StoresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Stores
        public ActionResult Index(int? page)
        {
            var pageSize = 10;
            var pageIndex = page ?? 1;


            var storeDTOs = db.Stores
                .OrderByDescending(x => x.Id)
                .Select(s => new StoreDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Address = s.Address.StreetAddress,
                    Long = s.Long,
                    Lat = s.Lat,
                    Image = s.Image,
                    Alias = s.Alias
                })
                .ToList();
            // Set ViewBag.Stores with storeDTOs for the store selector
            ViewBag.Stores = storeDTOs;
            var pagedStores = new PagedList<StoreDTO>(storeDTOs, pageIndex, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageIndex;

            return View(pagedStores);

        }
       [HttpPost]
    public JsonResult GetNearbyStores(string province, string district, string ward, string addressLine)
        {
            try
            {
                // Log giá trị đầu vào
                LogDebug("Input Values", new { province, district, ward, addressLine});

                // Lấy tất cả store để debug
                var allStores = db.Stores
                    .Include(s => s.Address)
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.Address.StreetAddress,
                        s.Address.WardName,
                        s.Address.DistrictName,
                        s.Address.ProvinceName,
                        s.Lat,
                        s.Long
                    })
                    .ToList();

                // Log tất cả stores trong database
                LogDebug("All Stores in Database", allStores);

                // Tìm kiếm chính xác
                var exactMatches = allStores
                    .Where(s =>
                        RemoveVietnameseDiacritics(s.ProvinceName?.ToLower() ?? "").Contains(RemoveVietnameseDiacritics(province.ToLower())) &&
                        RemoveVietnameseDiacritics(s.DistrictName?.ToLower() ?? "").Contains(RemoveVietnameseDiacritics(district.ToLower())) &&
                        RemoveVietnameseDiacritics(s.WardName?.ToLower() ?? "").Contains(RemoveVietnameseDiacritics(ward.ToLower())))
                    .ToList();

                // Log kết quả tìm kiếm chính xác
                LogDebug("Exact Matches", exactMatches);

                // Nếu không có kết quả chính xác, tìm theo quận/huyện
                if (!exactMatches.Any())
                {
                    var districtMatches = allStores
                        .Where(s =>
                            RemoveVietnameseDiacritics(s.ProvinceName?.ToLower() ?? "").Contains(RemoveVietnameseDiacritics(province.ToLower())) &&
                            RemoveVietnameseDiacritics(s.DistrictName?.ToLower() ?? "").Contains(RemoveVietnameseDiacritics(district.ToLower())))
                        .ToList();

                    // Log kết quả tìm kiếm theo quận/huyện
                    LogDebug("District Matches", districtMatches);

                    if (districtMatches.Any())
                    {
                        exactMatches = districtMatches;
                    }
                }

                if (!exactMatches.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Không tìm thấy cửa hàng nào trong khu vực này",
                        debug = new
                        {
                            searchCriteria = new { province, district, ward },
                            totalStores = allStores.Count,
                            searchValues = new
                            {
                                normalizedProvince = RemoveVietnameseDiacritics(province.ToLower()),
                                normalizedDistrict = RemoveVietnameseDiacritics(district.ToLower()),
                                normalizedWard = RemoveVietnameseDiacritics(ward.ToLower())
                            },
                            sampleStores = allStores.Take(3).Select(s => new
                            {
                                s.ProvinceName,
                                s.DistrictName,
                                s.WardName,
                                normalizedProvince = RemoveVietnameseDiacritics(s.ProvinceName?.ToLower() ?? ""),
                                normalizedDistrict = RemoveVietnameseDiacritics(s.DistrictName?.ToLower() ?? ""),
                                normalizedWard = RemoveVietnameseDiacritics(s.WardName?.ToLower() ?? "")
                            })
                        }
                    });
                }

                var centerStore = exactMatches.FirstOrDefault();
                var centerLat = centerStore?.Lat ?? 10.776330878;
                var centerLong = centerStore?.Long ?? 106.668019254;

                var nearbyStores = exactMatches
                    .Select(s => new
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Address = FormatAddressFromComponents(
                            s.StreetAddress,
                            s.WardName,
                            s.DistrictName,
                            s.ProvinceName
                        ),
                        Latitude = s.Lat,
                        Longitude = s.Long,
                        Distance = CalculateDistance(centerLat, centerLong, s.Lat, s.Long),
                        ShippingFee = CalculateShippingFee(s.Lat, s.Long, centerLat, centerLong) // Thêm phí ship
                    })
                    .Where(s => s.Distance <= 100)
                    .OrderBy(s => s.Distance)
                    .Take(100)
                    .ToList();

                return Json(new
                {
                    success = true,
                    stores = nearbyStores,
                    debug = new
                    {
                        searchCriteria = new { province, district, ward },
                        totalStores = allStores.Count,
                        matchedStores = exactMatches.Count,
                        nearbyStores = nearbyStores.Count
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Có lỗi xảy ra: " + ex.Message,
                    debug = new { error = ex.ToString() }
                });
            }
        }
        public decimal CalculateShippingFee(double storeLat, double storeLong, double customerLat, double customerLong)
        {
            var distance = CalculateDistance(storeLat, storeLong, customerLat, customerLong);
            var shippingFeeSettings = db.ShippingFee.FirstOrDefault();

            if (shippingFeeSettings == null)
                return 0;

            decimal shippingFee = shippingFeeSettings.FeePerKm * (decimal)distance;
            return Math.Max(shippingFee, shippingFeeSettings.MinimumFee);
        }
        private string RemoveVietnameseDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
        "đ",
        "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
        "í","ì","ỉ","ĩ","ị",
        "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
        "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
        "ý","ỳ","ỷ","ỹ","ỵ"};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
        "d",
        "e","e","e","e","e","e","e","e","e","e","e",
        "i","i","i","i","i",
        "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
        "u","u","u","u","u","u","u","u","u","u","u",
        "y","y","y","y","y"};

            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }

            // Xóa các tiền tố phổ biến
            string[] prefixesToRemove = new[] {
        "thanh pho ", "tp ", "tp. ",
        "quan ", "q. ", "q ",
        "phuong ", "p. ", "p ",
        "xa ", "x. ", "x "
    };

            foreach (var prefix in prefixesToRemove)
            {
                if (text.StartsWith(prefix))
                {
                    text = text.Substring(prefix.Length);
                }
            }

            return text.Trim();
        }

        private void LogDebug(string title, object data)
        {
            System.Diagnostics.Debug.WriteLine($"=== {title} ===");
            System.Diagnostics.Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented));
            System.Diagnostics.Debug.WriteLine("==================");
        }


        private string NormalizeLocationName(string location)
        {
            if (string.IsNullOrEmpty(location))
                return string.Empty;

            // Loại bỏ dấu cách thừa và chuyển về chữ thường
            location = location.Trim().ToLower();

            // Xóa các tiền tố phổ biến
            string[] prefixesToRemove = new[] {
        "thành phố ", "tp ", "tp. ",
        "quận ", "q. ", "q ",
        "phường ", "p. ", "p ",
        "xã ", "x. ", "x "
    };

            foreach (var prefix in prefixesToRemove)
            {
                if (location.StartsWith(prefix))
                {
                    location = location.Substring(prefix.Length);
                }
            }

            // Chuẩn hóa số
            location = location.Replace("02", "2");

            return location.Trim();
        }

        // Hàm mới để format địa chỉ từ các thành phần riêng lẻ
        private string FormatAddressFromComponents(string streetAddress, string wardName, string districtName, string provinceName)
        {
            var components = new[]
            {
        streetAddress,
        wardName,
        districtName,
        provinceName
    };

            return string.Join(", ", components.Where(c => !string.IsNullOrWhiteSpace(c)));
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Bán kính Trái Đất (km)
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Khoảng cách theo km
        }

        private double ToRad(double value)
        {
            return value * Math.PI / 180;
        }
       
        // GET: Stores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
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
