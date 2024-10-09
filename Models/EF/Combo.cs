//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Web;

//namespace WebsiteBanDoAnVaThucUong.Models.EF
//{
//    [Table("Combo")]
//    public class Combo
//    {
//        public Combo ()
//        {
//            this.ComboProduct = new HashSet<ComboProduct>();
//        }

//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public string Description { get; set; }
//        public decimal DiscountPercentage { get; set; }
//        public int MinimumQuantity { get; set; }
//        public bool IsActive { get; set; }
//        public DateTime CreatedDate { get; set; }
//        public DateTime ModifiedDate { get; set; }
//        public virtual Promotion Promotions { get; set; }
//        public virtual ICollection<ComboProduct> ComboProduct { get; set; }
//    }
//}