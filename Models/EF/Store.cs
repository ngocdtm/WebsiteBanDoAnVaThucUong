using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("Store")]
    public class Store 
    {
        //public Store()
        //{
        //    this.WishlistStores = new HashSet<WishlistStore>();
        //}
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(250)]
        public string Alias { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }
        [StringLength(250)]
        public string Image { get; set; }
        [Required]
        public string IdManager { get; set; }
        //public virtual ICollection<WishlistStore> WishlistStores { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}