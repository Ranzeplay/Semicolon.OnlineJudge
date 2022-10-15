using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Semicolon.Auth.Models;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models.Judge;
using Semicolon.OnlineJudge.Models.ViewModels.Users;

namespace Semicolon.OnlineJudge.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<SemicolonUser> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<SemicolonUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        [Route("Users/My")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            return Redirect($"/Users/{user.Id}");
        }

        [Route("Users/{Id}")]
        public async Task<IActionResult> Index(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var tracksQuery = _context.Tracks.Where(t => t.AuthorId == user.Id);
            var tracks = tracksQuery != null ? tracksQuery.ToList() : new List<Track>();
            tracks.Reverse();

            var model = new IndexModel
            {
                User = user,
                Tracks = tracks
            };

            ViewData["summaryChart"] = GeneratePieChart(tracks);

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> My()
        {
            var user = await _userManager.GetUserAsync(User);

            return RedirectToAction(nameof(Index), new { id = user.Id });
        }

        [Authorize]
        [Route("Users/ProblemsCreated")]
        public async Task<IActionResult> ProblemsCreated()
        {
            var user = await _userManager.GetUserAsync(User);

            return View(new ProblemsCreatedModel
            {
                Problems = _context.Problems.Where(p => p.AuthorId == user.Id).ToList()
            });
        }

        private static Chart GeneratePieChart(List<Track> tracks)
        {
            Chart chart = new()
            {
                Type = Enums.ChartType.Pie,
            };

            ChartJSCore.Models.Data data = new()
            {
                Labels = Enum.GetNames(typeof(JudgeStatus))
            };

            PieDataset dataset = new()
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

                Data = new List<double?>
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
    }
}