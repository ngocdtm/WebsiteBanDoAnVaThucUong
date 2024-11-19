using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("StoreProduct")]
    public class StoreProduct
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int StoreId { get; set; }
        public int ProductId { get; set; }

        // Foreign key references
        public virtual Store Store { get; set; }
        public virtual Product Product { get; set; }

        // Optional: Additional properties like stock count, price in this store, etc.
        public int StockCount { get; set; }
        public int SellCount { get; set; }
    }
}