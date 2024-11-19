using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models.EF;
using WebsiteBanDoAnVaThucUong.Models;
using System.Web.Management;
using WebsiteBanDoAnVaThucUong.Models.Payments;
using System.Data.Entity;
using System.Diagnostics;
using Newtonsoft.Json;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ShoppingCartController()
        {
        }

        public ShoppingCartController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: ShoppingCart
        [AllowAnonymous]
        public ActionResult Index()
        {

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }

       
        [AllowAnonymous]
        public ActionResult CheckOutSuccess()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Partial_Item_ThanhToan()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                // Lấy ShippingFee từ database
                using (var db = new ApplicationDbContext())
                {
                    var storeId = cart.StoreId;
                    var order = db.Orders
                                 .Where(o => o.StoreId == storeId)
                                 .OrderByDescending(o => o.CreatedDate)
                                 .FirstOrDefault();

                    if (order != null)
                    {
                        cart.ShippingFee = order.ShippingFee;
                        Session["Cart"] = cart; // Cập nhật lại session
                    }

                    ViewBag.ShippingFee = cart.ShippingFee;
                }
                return PartialView(cart.Items);
            }
            return PartialView();
        }


        [AllowAnonymous]
        public ActionResult Partial_Item_Cart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }

        [AllowAnonymous]
        public ActionResult ShowCount()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.Items.Count }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult Partial_CheckOut()
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            if (user != null)
            {
                ViewBag.User = user;
            }
            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult CheckOut()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }

        // Add this method to calculate shipping fee
        private decimal CalculateShippingFee(Address customerAddress, Store store)
        {
            if (customerAddress == null || store == null)
                return 0;

            // Get shipping fee settings
            var shippingFeeSettings = db.ShippingFee.FirstOrDefault();
            if (shippingFeeSettings == null)
                return 0;

            // Calculate distance between store and customer
            double distance = CalculateDistance(
                (double)store.Lat,
                (double)store.Long,
                (double)customerAddress.Latitude,
                (double)customerAddress.Longitude
            );

            // Calculate shipping fee based on distance and settings
            decimal shippingFee = shippingFeeSettings.FeePerKm * (decimal)distance;
            return Math.Max(shippingFee, shippingFeeSettings.MinimumFee);
        }

        // Helper method to calculate distance
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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(OrderViewModel req, string CouponCode)
        {

            var code = new { Success = false, Code = -1, Url = "", msg = "" };
            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null && cart.Items.Any())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var storeId = cart.Items.First().StoreId;
                            var customerAddress = (Address)Session["CustomerAddress"];
                            var store = db.Stores.Find(storeId);

                            // Kiểm tra địa chỉ khách hàng
                            if (customerAddress == null)
                            {
                                throw new Exception("Vui lòng cập nhật địa chỉ giao hàng.");
                            }
                            if (store == null)
                            {
                                throw new Exception("Không tìm thấy thông tin cửa hàng.");
                            }


                            if (cart.Items.All(item => item.StoreId == storeId))
                            {

                                // Tính phí ship
                                decimal shippingFee = CalculateShippingFee(customerAddress, store);

                                // 1. Tạo và lưu Order trước
                                Order order = new Order
                                {
                                    StoreId = storeId,
                                    OrderStatus = 1,
                                    ShippingStatus = 1,
                                    CreatedDate = DateTime.Now,
                                    CreatedBy = req.CustomerName,
                                    ModifiedDate = DateTime.Now,
                                    ModifiedBy = User.Identity.GetUserId(),
                                    TypePayment = req.TypePayment,
                                    ShippingFee = shippingFee  // Add shipping fee to order
                                };

                                if (User.Identity.IsAuthenticated)
                                    order.CustomerId = User.Identity.GetUserId();

                                Random rd = new Random();
                                order.Code = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);

                                db.Orders.Add(order);
                                db.SaveChanges(); // Lưu Order để có OrderId

                                var appliedPromotions = ApplyPromotions(cart);
                                decimal totalAmount = 0;
                                // 2. Process each cart item
                                foreach (var item in cart.Items)
                                {
                                    var product = db.Products.Find(item.ProductId);
                                    var isBeverage = product.ProductTypeId == 2;
                                    var hasExtras = product.ProductTypeId == 1;
                                    if (product == null) continue;

                                    // Xử lý đặc biệt cho sản phẩm tặng
                                    decimal unitPrice = item.IsGift ? 0 : item.BasePrice;
                                    decimal subtotal = item.IsGift ? 0 : (item.Quantity * item.BasePrice);
                                    decimal discountAmount = item.IsGift ? item.BasePrice : item.DiscountAmount;
                                    decimal finalAmount = item.IsGift ? 0 : item.TotalPrice;

                                    // Create OrderDetail
                                    var orderDetail = new OrderDetail
                                    {
                                        OrderId = order.Id,
                                        ProductId = item.ProductId,
                                        Quantity = item.Quantity,
                                        UnitPrice = item.BasePrice, // Use BasePrice instead of Price
                                        Subtotal = item.Quantity * item.BasePrice,
                                        DiscountAmount = item.DiscountAmount,
                                        FinalAmount = item.TotalPrice,
                                        IsGift = item.IsGift,
                                        // Lưu thông tin về size, topping, extra dưới dạng JSON
                                        SelectedSizeIds = JsonConvert.SerializeObject(item.SelectedSizeIds ?? new List<int>()),
                                        SelectedToppingIds = JsonConvert.SerializeObject(item.SelectedToppingIds ?? new List<int>()),
                                        SelectedExtraIds = JsonConvert.SerializeObject(item.SelectedExtraIds ?? new List<int>()),

                                        // Lưu giá của từng option
                                        SizePrice = !item.IsGift ? item.ProductSizes
                                    .Where(ps => item.SelectedSizeIds?.Contains(ps.Id) ?? false)
                                    .Sum(ps => ps.Size?.PriceSize ?? 0) : 0,

                                        ToppingPrice = !item.IsGift ? item.ProductToppings
                                    .Where(pt => item.SelectedToppingIds?.Contains(pt.Id) ?? false)
                                    .Sum(pt => pt.Topping?.PriceTopping ?? 0) : 0,

                                        ExtraPrice = !item.IsGift ? item.ProductExtras
                                    .Where(pe => item.SelectedExtraIds?.Contains(pe.Id) ?? false)
                                    .Sum(pe => pe.Extra?.Price ?? 0) : 0
                                    };

                                    db.OrderDetails.Add(orderDetail);
                                    db.SaveChanges();

                                    // Xử lý Promotions
                                    if (!item.IsGift && appliedPromotions.TryGetValue(item.ProductId, out var promotions))
                                    {
                                        foreach (var promo in promotions)
                                        {
                                            db.OrderDetailPromotion.Add(new OrderDetailPromotion
                                            {
                                                OrderDetailId = orderDetail.Id,
                                                PromotionId = promo.PromotionId,
                                                DiscountAmount = promo.DiscountAmount
                                            });
                                        }
                                    }

                                    // Cập nhật số lượng tồn kho
                                    if (!item.IsGift)
                                    {
                                        var storeProduct = db.StoreProducts
                                            .FirstOrDefault(sp => sp.ProductId == item.ProductId && sp.StoreId == storeId);
                                        if (storeProduct != null)
                                        {
                                            if (storeProduct.StockCount < item.Quantity)
                                            {
                                                throw new Exception($"Sản phẩm {item.ProductName} không đủ số lượng trong kho.");
                                            }
                                            storeProduct.StockCount -= item.Quantity;
                                            storeProduct.SellCount += item.Quantity;
                                            db.Entry(storeProduct).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            throw new Exception($"Không tìm thấy sản phẩm {item.ProductName} trong kho của cửa hàng.");
                                        }
                                    }

                                    totalAmount += orderDetail.FinalAmount;
                                }

                                // 3. Cập nhật thông tin tổng của Order
                                order.TotalQuantity = cart.GetTotalQuantity();
                                //order.SubTotal = order.OrderDetails.Sum(od => od.Subtotal);
                                //order.ProductDiscountTotal = order.OrderDetails.Sum(od => od.DiscountAmount);
                                order.SubTotal = order.OrderDetails.Where(od => !od.IsGift).Sum(od => od.Subtotal);
                                order.ProductDiscountTotal = order.OrderDetails.Where(od => !od.IsGift).Sum(od => od.DiscountAmount);
                                // Áp dụng voucher nếu có
                                var voucher = db.Vouchers.FirstOrDefault(v =>
                                    v.Coupon == CouponCode &&
                                    v.StartDate <= DateTime.Now &&
                                    v.EndDate >= DateTime.Now);
                                if (voucher != null)
                                {
                                    order.VoucherId = voucher.Id;
                                    order.VoucherDiscount = order.SubTotal * voucher.Discount;
                                }

                                order.FinalAmount = totalAmount + shippingFee - order.VoucherDiscount;
                                db.Entry(order).State = EntityState.Modified;
                                db.SaveChanges();

                                //send mail cho khachs hang
                                var strSanPham = "";
                                var thanhtien = decimal.Zero;
                                var TongTien = decimal.Zero;

                                strSanPham += "<table style='width:100%; border-collapse: collapse;'>";
                                strSanPham += "<thead>";
                                strSanPham += "<tr>";
                                strSanPham += "<th style='border: 1px solid #ddd; padding: 12px;'>Sản phẩm</th>";
                                strSanPham += "<th style='border: 1px solid #ddd; padding: 12px; text-align: center;'>Số lượng</th>";
                                strSanPham += "<th style='border: 1px solid #ddd; padding: 12px; text-align: right;'>Giá tiền</th>";
                                strSanPham += "</tr>";
                                strSanPham += "</thead>";
                                strSanPham += "<tbody>";

                                foreach (var sp in cart.Items)
                                {
                                    strSanPham += "<tr>";
                                    strSanPham += "<td style='border: 1px solid #ddd; padding: 12px;'>" + sp.ProductName;

                                    // Thêm thông tin Size, Topping, Extra (nếu có)
                                    if (sp.SelectedSizeIds?.Any() == true)
                                    {
                                        strSanPham += "<br/>Size: " + sp.FormattedSizeNames;
                                    }
                                    if (sp.SelectedToppingIds?.Any() == true)
                                    {
                                        strSanPham += "<br/>Topping: " + sp.FormattedToppingNames;
                                    }
                                    if (sp.SelectedExtraIds?.Any() == true)
                                    {
                                        strSanPham += "<br/>Extra: " + sp.FormattedExtraNames;
                                    }

                                    strSanPham += "</td>";

                                    // Hiển thị số lượng
                                    strSanPham += "<td style='border: 1px solid #ddd; padding: 12px; text-align: center;'>" + sp.Quantity + "</td>";

                                    // Hiển thị giá tiền
                                    strSanPham += "<td style='border: 1px solid #ddd; padding: 12px; text-align: right;'>" + WebsiteBanDoAnVaThucUong.Common.Common.FormatNumber(sp.TotalPrice, 0) + "</td>";
                                    strSanPham += "</tr>";

                                    thanhtien += sp.Price * sp.Quantity;
                                }

                                strSanPham += "</tbody>";
                                strSanPham += "</table>";

                                TongTien = thanhtien;
                                string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                                contentCustomer = contentCustomer.Replace("{{MaDon}}", order.Code);
                                contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                                contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                                contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", req.CustomerName);
                                contentCustomer = contentCustomer.Replace("{{Phone}}", req.Phone);
                                contentCustomer = contentCustomer.Replace("{{Email}}", req.Email);
                                contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", req.Address);
                                contentCustomer = contentCustomer.Replace("{{ThanhTien}}", WebsiteBanDoAnVaThucUong.Common.Common.FormatNumber(thanhtien, 0));
                                contentCustomer = contentCustomer.Replace("{{TongTien}}", WebsiteBanDoAnVaThucUong.Common.Common.FormatNumber(TongTien, 0));
                                WebsiteBanDoAnVaThucUong.Common.Common.SendMail("ShopOnline", "Đơn hàng #" + order.Code, contentCustomer.ToString(), req.Email);

                                string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                                contentAdmin = contentAdmin.Replace("{{MaDon}}", order.Code);
                                contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                                contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                                contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", req.CustomerName);
                                contentAdmin = contentAdmin.Replace("{{Phone}}", req.Phone);
                                contentAdmin = contentAdmin.Replace("{{Email}}", req.Email);
                                contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", req.Address);
                                contentAdmin = contentAdmin.Replace("{{ThanhTien}}", WebsiteBanDoAnVaThucUong.Common.Common.FormatNumber(thanhtien, 0));
                                contentAdmin = contentAdmin.Replace("{{TongTien}}", WebsiteBanDoAnVaThucUong.Common.Common.FormatNumber(TongTien, 0));
                                WebsiteBanDoAnVaThucUong.Common.Common.SendMail("ShopOnline", "Đơn hàng mới #" + order.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);


                                //var url = "";
                                if (req.TypePayment == 2)
                                {
                                    var url = UrlPayment(req.TypePaymentVN, order.Code);
                                    code = new { Success = true, Code = req.TypePayment, Url = url, msg = "" };
                                }
                                else
                                {
                                    code = new { Success = true, Code = req.TypePayment, Url = "", msg = "" };
                                }
                                db.SaveChanges(); // After Order creation
                                // Xóa giỏ hàng
                                cart.ClearCart();
                                Session["Cart"] = null;
                                db.SaveChanges(); // After Order creation
                                transaction.Commit();
                            }
                            else
                            {
                                code = new { Success = false, Code = -1, Url = "", msg = "Giỏ hàng chứa sản phẩm từ nhiều cửa hàng khác nhau. Vui lòng chỉ mua hàng từ một cửa hàng trong mỗi đơn hàng." };
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            code = new { Success = false, Code = -1, Url = "", msg = ex.Message };
                        }
                    }
                }
                else
                {
                    code = new { Success = false, Code = -1, Url = "", msg = "Giỏ hàng trống." };
                }
            }
            else
            {
                code = new { Success = false, Code = -1, Url = "", msg = "Dữ liệu không hợp lệ." };
            }
            return Json(code);
        }

        private Dictionary<int, List<(int PromotionId, decimal DiscountAmount)>> ApplyPromotions(ShoppingCart cart)
        {
            var appliedPromotions = new Dictionary<int, List<(int PromotionId, decimal DiscountAmount)>>();
            var activePromotions = db.Promotions.Where(p => p.IsActive && p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now).ToList();

            foreach (var promotion in activePromotions)
            {
                switch (promotion.DiscountType)
                {
                    case 1: // Percentage discount
                        ApplyPercentageDiscount(cart, promotion, appliedPromotions);
                        break;
                    case 2: // Combo discount
                        ApplyComboDiscount(cart, promotion, appliedPromotions);
                        break;
                    case 3: // Buy X Get Y
                        ApplyBuyXGetY(cart, promotion, appliedPromotions);
                        break;
                }
            }

            return appliedPromotions;
        }

        private void ApplyPercentageDiscount(ShoppingCart cart, Promotion promotion, Dictionary<int, List<(int PromotionId, decimal DiscountAmount)>> appliedPromotions)
        {
            var eligibleProducts = db.PromotionProducts
                .Where(pp => pp.PromotionId == promotion.Id)
                .Select(pp => pp.ProductId)
                .ToList();

            foreach (var item in cart.Items.Where(i => eligibleProducts.Contains(i.ProductId)))
            {
                decimal discountAmount = item.Price * (decimal)promotion.DiscountValue * item.Quantity;
                item.DiscountAmount += discountAmount;
                item.TotalPrice -= discountAmount;

                if (!appliedPromotions.ContainsKey(item.ProductId))
                    appliedPromotions[item.ProductId] = new List<(int, decimal)>();
                appliedPromotions[item.ProductId].Add((promotion.Id, discountAmount));
            }
        }

        private void ApplyComboDiscount(ShoppingCart cart, Promotion promotion, Dictionary<int, List<(int PromotionId, decimal DiscountAmount)>> appliedPromotions)
        {
            var eligibleProducts = db.PromotionProducts
                .Where(pp => pp.PromotionId == promotion.Id)
                .Select(pp => pp.ProductId)
                .ToList();

            var comboItems = cart.Items.Where(i => eligibleProducts.Contains(i.ProductId)).ToList();

            if (comboItems.Count >= 3)
            {
                decimal comboTotal = comboItems.Sum(i => i.Price * i.Quantity);
                decimal discountAmount = comboTotal * (decimal)promotion.DiscountValue;

                // Distribute discount proportionally
                foreach (var item in comboItems)
                {
                    decimal itemDiscount = (item.Price * item.Quantity / comboTotal) * discountAmount;
                    item.DiscountAmount += itemDiscount;
                    item.TotalPrice -= itemDiscount;

                    if (!appliedPromotions.ContainsKey(item.ProductId))
                        appliedPromotions[item.ProductId] = new List<(int, decimal)>();
                    appliedPromotions[item.ProductId].Add((promotion.Id, itemDiscount));
                }
            }
        }

        private void ApplyBuyXGetY(ShoppingCart cart, Promotion promotion, Dictionary<int, List<(int PromotionId, decimal DiscountAmount)>> appliedPromotions)
        {
            var buyProducts = db.PromotionProducts
                .Where(pp => pp.PromotionId == promotion.Id && pp.IsBuyProduct)
                .Select(pp => pp.ProductId)
                .ToList();

            var getFreeProducts = db.PromotionProducts
                .Where(pp => pp.PromotionId == promotion.Id && !pp.IsBuyProduct)
                .Select(pp => pp.ProductId)
                .FirstOrDefault();

            var buyItemsCount = cart.Items.Where(i => buyProducts.Contains(i.ProductId)).Sum(i => i.Quantity);

            if (buyItemsCount >= 2 && getFreeProducts != null)
            {
                var freeProduct = db.Products.Find(getFreeProducts);
                if (freeProduct != null)
                {
                    var freeItem = new ShoppingCartItem
                    {
                        ProductId = freeProduct.Id,
                        ProductName = freeProduct.Title,
                        Quantity = 1,
                        Price = freeProduct.SalePrice,
                        TotalPrice = 0,
                        DiscountAmount = freeProduct.SalePrice,
                        IsGift = true, 
                        StoreId = cart.StoreId 
                    };
                    cart.Items.Add(freeItem);

                    if (!appliedPromotions.ContainsKey(freeProduct.Id))
                    appliedPromotions[freeProduct.Id] = new List<(int, decimal)>();
                    appliedPromotions[freeProduct.Id].Add((promotion.Id, freeProduct.SalePrice));
                }
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddToCart(int id, int quantity, int storeId, int? sizeId, int[] toppingIds, int[] extraIds)
        {

            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            try
            {

                var db = new ApplicationDbContext();
                // Kiểm tra store có tồn tại không
                var store = db.Stores.FirstOrDefault(s => s.Id == storeId);
                if (store == null)
                {
                    return Json(new { Success = false, msg = "Cửa hàng không tồn tại!", code = -1, Count = 0 });
                }
                // Kiểm tra sản phẩm có trong store không
                var storeProduct = db.StoreProducts.FirstOrDefault(sp => sp.ProductId == id && sp.StoreId == storeId);
                if (storeProduct == null)
                {
                    return Json(new { Success = false, msg = "Sản phẩm không có sẵn tại cửa hàng này!", code = -1, Count = 0 });
                }

                //// Kiểm tra số lượng có đủ không
                //if (storeProduct.Quantity < quantity)
                //{
                //    return Json(new { Success = false, msg = $"Chỉ còn {storeProduct.Quantity} sản phẩm trong kho!", code = -1, Count = 0 });
                //}
                var checkProduct = db.Products.FirstOrDefault(x => x.Id == id);
                if (checkProduct != null)
                {
                    ShoppingCart cart = (ShoppingCart)Session["Cart"] ?? new ShoppingCart();
                    if (cart == null)
                    {
                        cart = new ShoppingCart();
                    }
                    // Kiểm tra nếu giỏ hàng đã có sản phẩm từ store khác
                    if (cart.Items.Any() && cart.Items.First().StoreId != storeId)
                    {
                        return Json(new { Success = false, msg = "Bạn chỉ có thể mua hàng từ một cửa hàng trong một đơn hàng!", code = -1, Count = cart.Items.Count });
                    }
                    // Calculate total price including add-ons
                    decimal totalPrice = checkProduct.SalePrice;
                    // Add size price if selected
                    if (sizeId.HasValue)
                    {
                        var size = db.ProductSize.FirstOrDefault(s => s.Id == sizeId.Value);
                        if (size != null)
                        {
                            totalPrice += size.Size.PriceSize;
                            System.Diagnostics.Debug.WriteLine($"Added size price: {size.Size.PriceSize}. New total: {totalPrice}");
                        }
                    }
                    // Add topping prices
                    if (toppingIds != null && toppingIds.Length > 0)
                    {
                        var toppings = db.ProductTopping.Where(t => toppingIds.Contains(t.Id));
                        var toppingPrice = toppings.Sum(t => t.Topping.PriceTopping);
                        totalPrice += toppingPrice;
                        System.Diagnostics.Debug.WriteLine($"Added topping price: {toppingPrice}. New total: {totalPrice}");
                    }
                

                    // Add extra prices
                    if (extraIds != null && extraIds.Length > 0)
                    {

                    var extras = db.ProductExtra.Where(e => extraIds.Contains(e.Id));
                    var extraPrice = extras.Sum(e => e.Extra.Price);
                    totalPrice += extraPrice;
                    System.Diagnostics.Debug.WriteLine($"Added extra price: {extraPrice}. New total: {totalPrice}");
                }
                    ShoppingCartItem item = new ShoppingCartItem
                    {
                        ProductId = checkProduct.Id,
                        ProductName = checkProduct.Title,
                        CategoryName = checkProduct.ProductCategory.Title,
                        Alias = checkProduct.Alias,
                        Quantity = quantity,
                        StoreId = storeId,  // Thêm tên store để hiển thị
                        Price = checkProduct.SalePrice, // Giá cơ bản
                        SelectedToppingIds = toppingIds?.ToList() ?? new List<int>(),
                        SelectedSizeIds = sizeId.HasValue ? new List<int> { sizeId.Value } : new List<int>(),
                        SelectedExtraIds = extraIds?.ToList() ?? new List<int>()
                    };
                    // Load Toppings
                    if (toppingIds != null && toppingIds.Length > 0)
                    {
                        item.ProductToppings = db.ProductTopping
                            .Include(pt => pt.Topping)
                            .Where(pt => toppingIds.Contains(pt.Id))
                            .ToList();
                    }

                    // Load Size
                    if (sizeId.HasValue)
                    {
                        item.ProductSizes = db.ProductSize
                            .Include(ps => ps.Size)
                            .Where(ps => ps.Id == sizeId.Value)
                            .ToList();
                    }

                    // Load Extras
                    if (extraIds != null && extraIds.Length > 0)
                    {
                        item.ProductExtras = db.ProductExtra
                            .Include(pe => pe.Extra)
                            .Where(pe => extraIds.Contains(pe.Id))
                            .ToList();
                    }


                    if (checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault) != null)
                    {
                        item.ProductImg = checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault).Image;
                    }
                    item.Price = checkProduct.SalePrice;
                    item.CalculateTotalPrice();  // Sử dụng phương thức có sẵn để tính toàn bộ giá

                    // Thêm vào giỏ hàng với giá đã bao gồm options
                    cart.AddToCart(item, quantity, storeId);

                    Session["Cart"] = cart;
                    code = new { Success = true, msg = "Thêm sản phẩm vào giỏ hàng thành công!", code = 1, Count = cart.Items.Count };

                    // Kiểm tra điều kiện "Mua 2 tặng 1"
                    var promotion = db.Promotions.FirstOrDefault(p => p.DiscountType == 3 && p.IsActive);
                    if (promotion != null)
                    {
                        var eligibleProducts = db.PromotionProducts
                            .Where(pp => pp.PromotionId == promotion.Id && pp.IsBuyProduct)
                            .Select(pp => pp.ProductId)
                            .ToList();

                        if (eligibleProducts.Contains(id))
                        {
                            var cartItems = cart.Items.Where(i => eligibleProducts.Contains(i.ProductId)).ToList();
                            if (cartItems.Sum(i => i.Quantity) >= 2)
                            {
                                var giftProduct = db.PromotionProducts
                                    .FirstOrDefault(pp => pp.PromotionId == promotion.Id && !pp.IsBuyProduct);
                                if (giftProduct != null)
                                {
                                    var giftItem = new ShoppingCartItem
                                    {
                                        ProductId = giftProduct.ProductId,
                                        ProductName = giftProduct.Product.Title,
                                        Alias = giftProduct.Product.Alias,
                                        CategoryName = giftProduct.Product.ProductCategory.Title,
                                        Quantity = 1,
                                        Price = 0,
                                        OriginalPrice = giftProduct.Product.SalePrice,
                                        TotalPrice = 0,
                                        ProductImg = giftProduct.Product.ProductImage.FirstOrDefault(x => x.IsDefault)?.Image,
                                        IsGift = true
                                    };
                                    cart.AddToCart(giftItem, 1, storeId);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in AddToCart: {ex.Message}");
                return Json(new { Success = false, msg = "Đã xảy ra lỗi khi thêm vào giỏ hàng", code = -1 });
            }
            return Json(code);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult ApplyCoupon(string couponCode)
        {
            var voucher = db.Vouchers.FirstOrDefault(v => v.Coupon == couponCode && v.StartDate <= DateTime.Now && v.EndDate >= DateTime.Now);
            if (voucher != null)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                decimal originalTotal = cart.GetTotalPrice();
                decimal discountAmount = originalTotal * voucher.Discount;
                decimal newTotal = originalTotal - discountAmount;

                return Json(new { success = true, originalTotal = originalTotal, newTotal = newTotal, discountAmount = discountAmount });
            }
            return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn." });
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Update(int id, int quantity, int storeId, int? sizeId = null, List<int> toppingIds = null, List<int> extraIds = null)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var storeProduct = db.StoreProducts.FirstOrDefault(sp => sp.ProductId == id && sp.StoreId == storeId);
                if (storeProduct != null)
                {
                    // Kiểm tra số lượng tồn kho
                    if (storeProduct.StockCount < quantity)
                    {
                        return Json(new { Success = false, msg = "Không đủ số lượng tồn kho.", code = -1, Count = cart.Items.Count });
                    }

                    var cartItem = cart.Items.FirstOrDefault(item =>
                        item.ProductId == id &&
                        item.StoreId == storeId &&
                        // So sánh size
                        ((!sizeId.HasValue && (item.SelectedSizeIds == null || item.SelectedSizeIds.Count == 0)) ||
                         (sizeId.HasValue && item.SelectedSizeIds != null && item.SelectedSizeIds.Contains(sizeId.Value))) &&
                        // So sánh topping
                        (toppingIds == null || toppingIds.Count == 0 && (item.SelectedToppingIds == null || item.SelectedToppingIds.Count == 0) ||
                         (item.SelectedToppingIds != null &&
                          item.SelectedToppingIds.OrderBy(x => x).SequenceEqual(toppingIds.OrderBy(x => x)))) &&
                        // So sánh extra
                        (extraIds == null || extraIds.Count == 0 && (item.SelectedExtraIds == null || item.SelectedExtraIds.Count == 0) ||
                         (item.SelectedExtraIds != null &&
                          item.SelectedExtraIds.OrderBy(x => x).SequenceEqual(extraIds.OrderBy(x => x)))));

                    if (cartItem != null)
                    {
                        cart.UpdateQuantity(id, quantity, storeId, sizeId, toppingIds, extraIds);
                        return Json(new { Success = true, msg = "Giỏ hàng đã được cập nhật.", code = 1, Count = cart.Items.Count });
                    }
                }
                return Json(new { Success = false, msg = "Không tìm thấy sản phẩm trong kho của cửa hàng.", code = -1, Count = cart.Items.Count });
            }
            return Json(new { Success = false, msg = "Không tìm thấy giỏ hàng.", code = -1, Count = 0 });
        }



        [AllowAnonymous]
        [HttpPost]
        public ActionResult Delete(int id, int storeId, int? sizeId = null, List<int> toppingIds = null, List<int> extraIds = null)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.ProductId == id);
                if (checkProduct != null)
                {
                    var cartItem = cart.Items.FirstOrDefault(item =>
                      item.ProductId == id &&
                      item.StoreId == storeId &&
                      // So sánh size
                      ((!sizeId.HasValue && (item.SelectedSizeIds == null || item.SelectedSizeIds.Count == 0)) ||
                       (sizeId.HasValue && item.SelectedSizeIds != null && item.SelectedSizeIds.Contains(sizeId.Value))) &&
                      // So sánh topping
                      (toppingIds == null || toppingIds.Count == 0 && (item.SelectedToppingIds == null || item.SelectedToppingIds.Count == 0) ||
                       (item.SelectedToppingIds != null &&
                        item.SelectedToppingIds.OrderBy(x => x).SequenceEqual(toppingIds.OrderBy(x => x)))) &&
                      // So sánh extra
                      (extraIds == null || extraIds.Count == 0 && (item.SelectedExtraIds == null || item.SelectedExtraIds.Count == 0) ||
                       (item.SelectedExtraIds != null &&
                        item.SelectedExtraIds.OrderBy(x => x).SequenceEqual(extraIds.OrderBy(x => x)))));
                    if (cartItem != null)
                    {
                        cart.Remove(id);
                        code = new { Success = true, msg = "Đã xóa khỏi giỏ hàng", code = 1, Count = cart.Items.Count };
                    }
                    else
                    {
                        code = new { Success = true, msg = "Không tìm thấy sản phẩm trong giỏ hàng", code = 1, Count = cart.Items.Count };
                    }
                }
            }
            return Json(code);
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult DeleteAll()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.ClearCart();
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        [AllowAnonymous]
        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = db.Orders.FirstOrDefault(x => x.Code == orderCode);
                        if (itemOrder != null)
                        {
                            itemOrder.OrderStatus = 2;//đã thanh toán
                            db.Orders.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }
            }
            //var a = UrlPayment(0, "DH3574");
            return View();
        }
        #region Thanh toán vnpay
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = db.Orders.FirstOrDefault(x => x.Code == orderCode);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.FinalAmount * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Code);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
        #endregion
    }
}