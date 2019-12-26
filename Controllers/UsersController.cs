using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using Microsoft.AspNetCore.Mvc;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models.Judge;
using Semicolon.OnlineJudge.Models.User;
using Semicolon.OnlineJudge.Models.ViewModels.Users;

namespace Semicolon.OnlineJudge.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string id)
        {
            var user = _context.OJUsers.ToList().FirstOrDefault(u => u.Id == id);

            if (User.Identity.IsAuthenticated && id == null)
            {
                user = GetUserProfile();
            }

            var tracks = _context.Tracks.Where(t => t.AuthorId == user.Id).ToList();
            tracks.Reverse();

            var model = new IndexModel
            {
                User = user,
                Tracks = tracks
            };

            ViewData["summaryChart"] = GeneratePieChart(tracks);

            return View(model);
        }

        private Chart GeneratePieChart(List<Track> tracks)
        {
            Chart chart = new Chart
            {
                Type = Enums.ChartType.Pie,
            };

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data
            {
                Labels = Enum.GetNames(typeof(JudgeStatus))
            };

            PieDataset dataset = new PieDataset
            {
                Label = "Summary",

                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#28A745"),
                    ChartColor.FromHexString("#FFC720"),
                    ChartColor.FromHexString("#DC3545"),
                    ChartColor.FromHexString("#1B6EC2"),
                    ChartColor.FromHexString("#6C757D")
                },

                HoverBackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#28A745"),
                    ChartColor.FromHexString("#FFC720"),
                    ChartColor.FromHexString("#DC3545"),
                    ChartColor.FromHexString("#1B6EC2"),
                    ChartColor.FromHexString("#6C757D")
                },

                Data = new List<double> 
                {
                    tracks.LongCount(x => x.Status == JudgeStatus.Accept), 
                    tracks.LongCount(x => x.Status == JudgeStatus.WrongAnswer),
                    tracks.LongCount(x => x.Status == JudgeStatus.CompileError),
                    tracks.LongCount(x => x.Status == JudgeStatus.UnknownError),
                    tracks.LongCount(x => x.Status == JudgeStatus.Pending)
                },
            };

            data.Datasets = new List<Dataset>
            {
                dataset
            };

            chart.Data = data;

            return chart;
        }

        private OJUser GetUserProfile()
        {
            var user = new OJUser();

            try
            {
                var id = HttpContext.User.FindFirst("UserId").Value;
                user = _context.OJUsers.FirstOrDefault(u => u.Id == id);
            }
            catch
            {
                return null;
            }

            return user;
        }
    }
}