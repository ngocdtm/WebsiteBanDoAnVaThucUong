//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace WebsiteBanDoAnVaThucUong.Models.EF
//{
//    public class PromotionHelper
//    {
//        public static void ApplyPromotions(ShoppingCart cart, ApplicationDbContext db)
//        {
//            var activePromotions = db.Promotions
//                .Where(p => p.IsActive && p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now)
//                .ToList();

//            foreach (var promotion in activePromotions)
//            {
//                switch (promotion.DiscountType)
//                {
//                    case 1: // Percentage discount
//                        ApplyPercentageDiscount(cart, promotion, db);
//                        break;
//                    case 2: // Combo discount
//                        ApplyComboDiscount(cart, promotion, db);
//                        break;
//                    case 3: // Buy X Get Y
//                        ApplyBuyXGetY(cart, promotion, db);
//                        break;
//                }
//            }
//        }

//        private static void ApplyPercentageDiscount(ShoppingCart cart, Promotion promotion, ApplicationDbContext db)
//        {
//            var eligibleProductIds = db.PromotionProducts
//                .Where(pp => pp.PromotionId == promotion.Id)
//                .Select(pp => pp.ProductId)
//                .ToList();

//            foreach (var item in cart.Items.Where(i => eligibleProductIds.Contains(i.ProductId) && !i.IsGift))
//            {
//                item.OriginalPrice = item.Price;
//                item.DiscountAmount = item.Price * (decimal)promotion.DiscountValue;
//                item.Price -= item.DiscountAmount;
//                item.TotalPrice = item.Price * item.Quantity;
//            }
//        }

//        private static void ApplyComboDiscount(ShoppingCart cart, Promotion promotion, ApplicationDbContext db)
//        {
//            var eligibleProductIds = db.PromotionProducts
//                .Where(pp => pp.PromotionId == promotion.Id)
//                .Select(pp => pp.ProductId)
//                .ToList();

//            var eligibleItems = cart.Items.Where(i => eligibleProductIds.Contains(i.ProductId) && !i.IsGift).ToList();

//            if (eligibleItems.Count >= promotion.ComboQuantity)
//            {
//                foreach (var item in eligibleItems)
//                {
//                    if (item.OriginalPrice == 0)
//                    {
//                        item.OriginalPrice = item.Price;
//                    }
//                    var additionalDiscount = item.OriginalPrice * (decimal)promotion.DiscountValue;
//                    item.DiscountAmount += additionalDiscount;
//                    item.Price = item.OriginalPrice - item.DiscountAmount;
//                    item.TotalPrice = item.Price * item.Quantity;
//                }
//            }
//        }

//        private static void ApplyBuyXGetY(ShoppingCart cart, Promotion promotion, ApplicationDbContext db)
//        {
//            var buyProductIds = db.PromotionProducts
//                .Where(pp => pp.PromotionId == promotion.Id && pp.IsBuyProduct)
//                .Select(pp => pp.ProductId)
//                .ToList();

//            var getFreeProductId = db.PromotionProducts
//                .FirstOrDefault(pp => pp.PromotionId == promotion.Id && !pp.IsBuyProduct)?.ProductId;

//            var buyItemsCount = cart.Items.Where(i => buyProductIds.Contains(i.ProductId)).Sum(i => i.Quantity);

//            if (buyItemsCount >= promotion.BuyQuantity && getFreeProductId.HasValue)
//            {
//                var freeProduct = db.Products.Find(getFreeProductId.Value);
//                if (freeProduct != null)
//                {
//                    var existingGiftItem = cart.Items.FirstOrDefault(i => i.ProductId == getFreeProductId && i.IsGift);
//                    if (existingGiftItem == null)
//                    {
//                        var giftItem = new ShoppingCartItem
//                        {
//                            ProductId = freeProduct.Id,
//                            ProductName = freeProduct.Title,
//                            Alias = freeProduct.Alias,
//                            CategoryName = freeProduct.ProductCategory.Title,
//                            Quantity = promotion.GetQuantity ?? 1,
//                            Price = 0,
//                            OriginalPrice = freeProduct.SalePrice,
//                            TotalPrice = 0,
//                            ProductImg = freeProduct.ProductImage.FirstOrDefault(x => x.IsDefault)?.Image,
//                            IsGift = true
//                        };
//                        cart.Items.Add(giftItem);
//                    }
//                    else
//                    {
//                        existingGiftItem.Quantity = promotion.GetQuantity ?? 1;
//                    }
//                }
//            }
//        }
//    }
//}