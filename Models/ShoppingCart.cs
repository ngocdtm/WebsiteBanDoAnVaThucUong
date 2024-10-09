using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models
{
    public class ShoppingCart
    {
        public List<ShoppingCartItem> Items { get; set; }
        public ShoppingCart()
        {
            this.Items = new List<ShoppingCartItem>();
        }

        public void AddToCart(ShoppingCartItem item, int Quantity)
        {
            var checkExits = Items.FirstOrDefault(x => x.ProductId == item.ProductId);
            if (checkExits != null)
            {
                checkExits.Quantity += Quantity;
                checkExits.TotalPrice = checkExits.Price * checkExits.Quantity;
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

        public void UpdateQuantity(int id, int quantity)
        {
            var checkExits = Items.SingleOrDefault(x => x.ProductId == id);
            if (checkExits != null)
            {
                checkExits.Quantity = quantity;
                checkExits.TotalPrice = checkExits.Price * checkExits.Quantity;

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
    }
}