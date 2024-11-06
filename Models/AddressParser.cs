using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models
{
    public class AddressParser
    {
        public class ParsedAddress
        {
            public string StreetAddress { get; set; }
            public string Ward { get; set; }
            public string District { get; set; }
            public string Province { get; set; }
        }

        public static ParsedAddress ParseVietnameseAddress(string fullAddress)
        {
            var parts = fullAddress.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            var result = new ParsedAddress();

            // Đảo ngược mảng để xử lý từ địa chỉ lớn đến nhỏ
            Array.Reverse(parts);

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i].Trim();

                // Xác định tỉnh/thành phố
                if (part.Contains("Việt Nam"))
                    continue;

                if (i == 0 && (part.Contains("Thành phố") || part.Contains("Tỉnh") || part == "Hồ Chí Minh"))
                {
                    result.Province = part;
                    continue;
                }

                // Xác định quận/huyện
                if (part.StartsWith("Quận") || part.StartsWith("Huyện") || part == "Tân Bình" || part == "Hóc Môn")
                {
                    result.District = part;
                    continue;
                }

                // Xác định phường/xã
                if (part.StartsWith("Phường") || part.StartsWith("Xã") || part.StartsWith("ấp"))
                {
                    result.Ward = part;
                    continue;
                }

                // Phần còn lại là địa chỉ đường
                if (result.StreetAddress == null)
                {
                    result.StreetAddress = part;
                }
                else
                {
                    result.StreetAddress = part + ", " + result.StreetAddress;
                }
            }

            return result;
        }
    }
}