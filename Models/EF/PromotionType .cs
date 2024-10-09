//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Web;

//namespace WebsiteBanDoAnVaThucUong.Models.EF
//{
//    [Table("PromotionType")]
//    public class PromotionType
//    {
//        //public PromotionType()
//        //{
//        //    this.PromotionRule = new HashSet<PromotionRule>();
//        //}

//        [Key]
//        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
//        public int Id { get; set; }
//        public string NameType { get; set; }
//        public string Description { get; set; }

//        //public virtual ICollection<PromotionRule> PromotionRule { get; set; }
//    }
//}