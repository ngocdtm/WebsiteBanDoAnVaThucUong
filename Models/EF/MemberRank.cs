using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.EF
{
    [Table("MemberRanks")]
    public class MemberRank
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int Points { get; set; }
        public string Rank { get; set; }
        public decimal TotalSpent { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}


