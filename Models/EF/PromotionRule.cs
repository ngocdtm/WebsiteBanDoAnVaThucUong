//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Web;

//namespace WebsiteBanDoAnVaThucUong.Models.EF
//{
//    [Table("PromotionRule")]
//    public class PromotionRule
//    {
//        [Key]
//        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
//        public int Id { get; set; }
//        [ForeignKey("Promotion")]
//        public int PromotionId { get; set; }
//        [ForeignKey("PromotionType")]
//        public int PromotionTypeId { get; set; }
//        public string Condition { get; set; }
//        public string Reward { get; set; }
//        public virtual Promotion Promotion { get; set; }
//        public virtual PromotionType PromotionType { get; set; }
//    }
//}