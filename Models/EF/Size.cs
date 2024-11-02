using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
    {
        [Table("Size")]
        public class Size
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            [StringLength(50)]
            public string NameSize { get; set; } // Ví dụ: "S", "M", "L"

            public decimal PriceSize { get; set; } // Giá tăng thêm cho size

            [ForeignKey("Product")]
            public int ProductId { get; set; }
            public virtual Product Product { get; set; }
        }
    }

