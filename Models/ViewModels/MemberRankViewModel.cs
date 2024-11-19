using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteBanDoAnVaThucUong.Models.ViewModels
{
    public class MemberRankViewModel
    {
        public string Rank { get; set; }
        public int Points { get; set; }
        public decimal TotalSpent { get; set; }
        public int PointsToNextRank { get; set; }
        public string NextRank { get; set; }
    }
}

