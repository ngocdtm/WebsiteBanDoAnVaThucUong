using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string StreetAddress { get; set; }

        [Required]
        public int ProvinceId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProvinceName { get; set; }

        [Required]
        public int DistrictId { get; set; }

        [Required]
        [StringLength(100)]
        public string DistrictName { get; set; }

        [Required]
        public int WardId { get; set; }

        [Required]
        [StringLength(100)]
        public string WardName { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        // Thời gian tạo và cập nhật
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}