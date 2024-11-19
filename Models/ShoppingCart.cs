using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Models
{
    public class ShoppingCart
    {
        public List<ShoppingCartItem> Items { get; set; }
        public decimal ShippingFee { get; set; }
        public int StoreId { get; set; }
        public ShoppingCart()
        {
            this.Items = new List<ShoppingCartItem>();
            this.ShippingFee = 0;
           
        }
        // Add validation method
        public bool IsValidForCheckout()
        {
            if (Items == null || !Items.Any())
                return false;

            // Check if all items are from the same store
            var storeIds = Items.Select(x => x.StoreId).Distinct();
            if (storeIds.Count() > 1)
                return false;

            // Validate each item
            foreach (var item in Items)
            {
                if (!item.IsValidForCheckout)
                    return false;

                // Validate customizations for beverages
                if (item.ProductTypeId == 2) // Beverage
                {
                    if (item.SelectedSizeIds?.Any() == true &&
                        !item.ProductSizes.Any(ps => item.SelectedSizeIds.Contains(ps.Id)))
                        return false;

                    if (item.SelectedToppingIds?.Any() == true &&
                        !item.ProductToppings.Any(pt => item.SelectedToppingIds.Contains(pt.Id)))
                        return false;
                }

                // Validate extras
                if (item.ProductTypeId == 1 && // Product with extras
                    item.SelectedExtraIds?.Any() == true &&
                    !item.ProductExtras.Any(pe => item.SelectedExtraIds.Contains(pe.Id)))
                    return false;
            }

            return true;
        }
        // Add method to get checkout-ready items
        public List<CheckoutItem> GetCheckoutItems()
        {
            return Items.Select(item => new CheckoutItem
            {
                ProductId = item.ProductId,
                StoreId = item.StoreId,
                Quantity = item.Quantity,
                BasePrice = item.BasePrice,
                TotalPrice = item.TotalPrice,
                DiscountAmount = item.DiscountAmount,
                SelectedSizeIds = item.SelectedSizeIds,
                SelectedToppingIds = item.SelectedToppingIds,
                SelectedExtraIds = item.SelectedExtraIds,
                IceLevel = item.IceLevel,
                SweetnessLevel = item.SweetnessLevel,
                Temperature = item.Temperature,
                ProductTypeId = item.ProductTypeId
            }).ToList();
        }

        // New class to represent checkout-ready items
        public class CheckoutItem
        {
            public int ProductId { get; set; }
            public int StoreId { get; set; }
            public int Quantity { get; set; }
            public decimal BasePrice { get; set; }
            public decimal TotalPrice { get; set; }
            public decimal DiscountAmount { get; set; }
            public List<int> SelectedSizeIds { get; set; }
            public List<int> SelectedToppingIds { get; set; }
            public List<int> SelectedExtraIds { get; set; }
            public string IceLevel { get; set; }
            public string SweetnessLevel { get; set; }
            public string Temperature { get; set; }
            public int ProductTypeId { get; set; }
        }
        public void AddToCart(ShoppingCartItem item, int Quantity, int StoreId)
        {
            var checkExits = Items.FirstOrDefault(x =>
             x.ProductId == item.ProductId &&
             x.StoreId == StoreId &&
             Enumerable.SequenceEqual(x.SelectedToppingIds.OrderBy(id => id), item.SelectedToppingIds.OrderBy(id => id)) &&
             Enumerable.SequenceEqual(x.SelectedSizeIds.OrderBy(id => id), item.SelectedSizeIds.OrderBy(id => id)) &&
             Enumerable.SequenceEqual(x.SelectedExtraIds.OrderBy(id => id), item.SelectedExtraIds.OrderBy(id => id)));

            if (checkExits != null)
            {
                checkExits.Quantity += Quantity;
                checkExits.CalculateTotalPrice();
            }
            else
            {
                Items.Add(item);
            }
        }


        public void Remove(int id)
        {
            var checkExits = Items.SingleOrDefault(x => x.ProductId == id);
            if (checkExits != null)
            {
                Items.Remove(checkExits);

                // Kiểm tra và xóa sản phẩm tặng nếu cần
                RemoveGiftItemIfNeeded();
            }
        }

        private void RemoveGiftItemIfNeeded()
        {
            // Kiểm tra điều kiện "Mua 2 tặng 1"
            var buyProducts = Items.Where(i => !i.IsGift).ToList();
            if (buyProducts.Sum(i => i.Quantity) < 2)
            {
                var giftItem = Items.FirstOrDefault(i => i.IsGift);
                if (giftItem != null)
                {
                    Items.Remove(giftItem);
                }
            }
        }

        public void UpdateQuantity(int id, int quantity, int storeId, int? sizeId = null, List<int> toppingIds = null, List<int> extraIds = null)
        {
            var checkExists = Items.FirstOrDefault(x =>
                x.ProductId == id &&
                x.StoreId == storeId &&
                // So sánh size
                ((!sizeId.HasValue && (x.SelectedSizeIds == null || x.SelectedSizeIds.Count == 0)) ||
                 (sizeId.HasValue && x.SelectedSizeIds != null && x.SelectedSizeIds.Contains(sizeId.Value))) &&
                // So sánh topping
                (toppingIds == null || toppingIds.Count == 0 && (x.SelectedToppingIds == null || x.SelectedToppingIds.Count == 0) ||
                 (x.SelectedToppingIds != null &&
                  x.SelectedToppingIds.OrderBy(t => t).SequenceEqual(toppingIds.OrderBy(t => t)))) &&
                // So sánh extra
                (extraIds == null || extraIds.Count == 0 && (x.SelectedExtraIds == null || x.SelectedExtraIds.Count == 0) ||
                 (x.SelectedExtraIds != null &&
                  x.SelectedExtraIds.OrderBy(e => e).SequenceEqual(extraIds.OrderBy(e => e)))));

            if (checkExists != null)
            {
                checkExists.Quantity = quantity;
                checkExists.CalculateTotalPrice();

                // Kiểm tra và xóa sản phẩm tặng nếu cần
                RemoveGiftItemIfNeeded();
            }
        }



        public decimal GetTotalPrice()
        {
            return Items.Sum(x => x.TotalPrice);
        }
        public int GetTotalQuantity()
        {
            return Items.Sum(x => x.Quantity);
        }
        public void ClearCart()
        {
            Items.Clear();
        }

        public decimal GetSubTotal()
        {
            return Items.Sum(x => x.Price * x.Quantity);
        }

        public decimal GetTotalDiscount()
        {
            return Items.Sum(x => x.DiscountAmount);
        }
   
    }

    public class ShoppingCartItem
    {
        public int StoreId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Alias { get; set; }
        public string CategoryName { get; set; }
        public string ProductImg { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public bool IsGift { get; set; }
        public decimal ShippingFee { get; set; }
        // Add these new properties for beverage customizations
        public string IceLevel { get; set; }
        public string SweetnessLevel { get; set; }
        public string Temperature { get; set; }
        public decimal BasePrice { get; set; }  // Original price without customizations

        // Add property to identify product type
        public int ProductTypeId { get; set; }  // To identify if it's a beverage (2) or has extras (1)

        // Add validation properties
        public bool IsValidForCheckout
        {
            get
            {
                if (IsGift)
                {
                    return Quantity > 0 &&
                           ProductId > 0 &&
                           StoreId > 0;
                }

                return Quantity > 0 &&
                       ProductId > 0 &&
                       StoreId > 0 &&
                       Price >= 0;
            }
        }
        // Selected IDs for options
        public List<int> SelectedToppingIds { get; set; }
        public List<int> SelectedSizeIds { get; set; }
        public List<int> SelectedExtraIds { get; set; }

       

        // Related entity collections
        public List<ProductTopping> ProductToppings { get; set; }
        public List<ProductSize> ProductSizes { get; set; }
        public List<ProductExtra> ProductExtras { get; set; }

        public ShoppingCartItem()
        {
            SelectedToppingIds = new List<int>();
            SelectedSizeIds = new List<int>();
            SelectedExtraIds = new List<int>();

            ProductToppings = new List<ProductTopping>();
            ProductSizes = new List<ProductSize>();
            ProductExtras = new List<ProductExtra>();
            // Initialize new properties
        

        }
        // Helper properties for display

        public string FormattedToppingNames => string.Join(", ",
            ProductToppings?.Select(t => t.Topping.NameTopping) ?? Enumerable.Empty<string>());
        public string FormattedSizeNames => string.Join(", ",
          ProductSizes?.Select(t => t.Size.NameSize) ?? Enumerable.Empty<string>());
        public string FormattedExtraNames => string.Join(", ",
        ProductExtras?.Select(t => t.Extra.Name) ?? Enumerable.Empty<string>());


        // Method to calculate total price including all options
        public void CalculateTotalPrice()
        {
            try
            {
                if (IsGift)
                {
                    TotalPrice = 0;
                    return;
                }
                decimal basePrice = Price;
                decimal customizationPrice = 0;

                // Calculate topping price with null check
                if (SelectedToppingIds?.Any() == true && ProductToppings?.Any() == true)
                {
                    customizationPrice += ProductToppings
                        .Where(pt => SelectedToppingIds.Contains(pt.Id))
                        .Sum(pt => pt.Topping?.PriceTopping ?? 0);
                }

                // Calculate size price with null check
                if (SelectedSizeIds?.Any() == true && ProductSizes?.Any() == true)
                {
                    customizationPrice += ProductSizes
                        .Where(ps => SelectedSizeIds.Contains(ps.Id))
                        .Sum(ps => ps.Size?.PriceSize ?? 0);
                }

                // Calculate extra price with null check
                if (SelectedExtraIds?.Any() == true && ProductExtras?.Any() == true)
                {
                    customizationPrice += ProductExtras
                        .Where(pe => SelectedExtraIds.Contains(pe.Id))
                        .Sum(pe => pe.Extra?.Price ?? 0);
                }


                // Store base price for later use
                BasePrice = basePrice;

                // Calculate final price before discount
                decimal unitPrice = basePrice + customizationPrice;
                decimal totalBeforeDiscount = unitPrice * Quantity;

                // Apply discount if available
                if (DiscountAmount > 0)
                {
                    // Calculate total discount for all items
                    decimal totalDiscount = DiscountAmount * Quantity;
                    // Set final price after discount
                    TotalPrice = totalBeforeDiscount - totalDiscount;
                }
                else
                {
                    TotalPrice = totalBeforeDiscount;
                }

                // Ensure price doesn't go below 0
                TotalPrice = Math.Max(0, TotalPrice);
            }

            catch (Exception ex)
            {
                // Log the error
                throw new Exception($"Error calculating total price: {ex.Message}");
            }
        }

    }
        

      
}