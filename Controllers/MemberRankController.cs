using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Models;
using WebsiteBanDoAnVaThucUong.Models.ViewModels;

namespace WebsiteBanDoAnVaThucUong.Controllers
{
    public class MemberRankController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult MemberRank()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userId = User.Identity.GetUserId();
            var memberRank = db.MemberRanks.FirstOrDefault(x => x.UserId == userId);

            var viewModel = new MemberRankViewModel
            {
                Rank = memberRank?.Rank ?? "Chưa có điểm",
                Points = memberRank?.Points ?? 0,
                TotalSpent = memberRank?.TotalSpent ?? 0
            };

            // Calculate points needed for next rank
            if (viewModel.Points <= 5 && viewModel.Points > 0)
            {
                viewModel.NextRank = "Đồng";
                viewModel.PointsToNextRank = 6 - viewModel.Points;
            }
            else if (viewModel.Points < 10 && viewModel.Points > 5)
            {
                viewModel.NextRank = "Bạc";
                viewModel.PointsToNextRank = 10 - viewModel.Points;
            }
            else if (viewModel.Points >= 10 && viewModel.Points < 20)
            {
                viewModel.NextRank = "Vàng";
                viewModel.PointsToNextRank = 20 - viewModel.Points;
            }
            else if (viewModel.Points < 5)
            {
                viewModel.NextRank = "Thành viên";
                viewModel.PointsToNextRank = 1 - viewModel.Points;
            }

            return View(viewModel);
        }
    }
}


