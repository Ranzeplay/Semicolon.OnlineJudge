using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Semicolon.Auth.Models;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models;
using Semicolon.OnlineJudge.Models.ViewModels.Home;

namespace Semicolon.OnlineJudge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;

        private readonly UserManager<SemicolonUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<SemicolonUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["ProblemsPassed"] = _context.Tracks.Where(t => t.Status == Models.Judge.JudgeStatus.Accept).Where(t => t.AuthorId == user.Id).LongCount();
                ViewData["ProblemsSubmitted"] = _context.Tracks.Where(t => t.AuthorId == user.Id).LongCount();
            }
            else
            {
                ViewData["ProblemsPassed"] = "登录以查看更多信息";
                ViewData["ProblemsSubmitted"] = "登录以查看更多信息";
            }

            return View(new IndexModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
