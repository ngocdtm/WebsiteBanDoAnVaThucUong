//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;

//namespace WebsiteBanDoAnVaThucUong.Models.EF
//{
//    [Table("WishlistStore")]
//    public class WishlistStore
//    {
//        [Key]
//        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
//        public int Id { get; set; }
//        [ForeignKey("Store")]
//        public int StoreId { get; set; }
//        public string UserName { get; set; }
//        public DateTime CreatedDate { get; set; }

//        public virtual Store Store { get; set; }
//    }
//}