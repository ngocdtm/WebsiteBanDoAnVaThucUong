using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebsiteBanDoAnVaThucUong.Models.EF;

namespace WebsiteBanDoAnVaThucUong.Models
{
    public class RankingService
    {
        private readonly ApplicationDbContext _db;

        public RankingService(ApplicationDbContext db)
        {
            _db = db;
        }

        public void UpdateMemberRank(string userId, decimal orderAmount)
        {
            var memberRank = _db.MemberRanks.FirstOrDefault(x => x.UserId == userId)
                ?? new MemberRank { UserId = userId };

            memberRank.TotalSpent += orderAmount;
            memberRank.Points += CalculatePoints(orderAmount);
            memberRank.Rank = DetermineRank(memberRank.Points);

            if (memberRank.Id == 0)
                _db.MemberRanks.Add(memberRank);

            _db.SaveChanges();
        }

        private int CalculatePoints(decimal amount)
        {
            return (int)(amount / 10000);
        }

        private string DetermineRank(int points)
        {
            if (points <= 5 && points > 0) return "Thành viên";
            if (points >= 20) return "Vàng";
            if (points >= 10 && points < 20) return "Bạc";
            if (points < 10 && points > 5) return "Đồng";
            return "Chưa có điểm";
        }
    }
}

