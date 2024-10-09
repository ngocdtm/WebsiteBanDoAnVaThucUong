//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Web;

//namespace WebsiteBanDoAnVaThucUong.Models.EF
//{
//    [Table("ComboProduct")]
//    public class ComboProduct
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int Id { get; set; }
//        [ForeignKey("Product")]
//        public int IdProduct { get; set; }
//        [ForeignKey("Combo")]
//        public int IdCombo { get; set; }
//        public int Quantity { get; set; }
//        public virtual Product Product { get; set; }
//        public virtual Combo Combo { get; set; }
//    }
//}