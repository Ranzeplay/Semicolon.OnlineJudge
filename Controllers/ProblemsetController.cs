using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Markdig;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Semicolon.Auth.Models;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models.Problemset;
using Semicolon.OnlineJudge.Models.ViewModels.Problemset;

namespace Semicolon.OnlineJudge.Controllers
{
    public class ProblemsetController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<SemicolonUser> _userManager;

        public ProblemsetController(ApplicationDbContext context, UserManager<SemicolonUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new IndexModel
            {
                ProblemModels = new List<ProblemModel>()
            };
            foreach (var problem in _context.Problems.ToList())
            {
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Build();

                var html = Markdown.ToHtml(problem.Description, pipeline);
                var raw = Markdown.ToPlainText(problem.Description);

                var author = await _userManager.FindByIdAsync(problem.AuthorId);

                model.ProblemModels.Add(new ProblemModel
                {
                    Id = problem.Id,
                    Title = problem.Title,
                    Description = problem.Description,
                    ContentRaw = raw,
                    ContentHtml = html,
                    AuthorId = problem.AuthorId,
                    Author = author.UserName,
                    ExampleData = problem.ExampleData,
                    JudgeProfile = problem.JudgeProfile,
                    PassRate = problem.PassRate,
                    PublishTime = problem.PublishTime
                });
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult New(NewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(NewTestData), model);
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult NewTestData(NewModel model)
        {
            model.TestDatas = new List<TestData>();
            for (int i = 0; i < model.TestDataNumber; i++)
            {
                model.TestDatas.Add(new TestData { Input = "Your data", Output = "Your data" });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> NewTestData(NewModel model, string status)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return Unauthorized();
            }

            var problem = new Problem
            {
                Title = model.Title,
                Description = model.Description,
                AuthorId = user.Id,
                PublishTime = DateTime.UtcNow,
            };

            var exampleData = new TestData
            {
                Input = model.ExampleData.Input,
                Output = model.ExampleData.Output
            };

            problem.SetExampleData(exampleData);

            var judgeProfile = new JudgeProfile
            {
                MemoryLimit = model.MemoryLimit,
                TimeLimit = model.TimeLimit,
                TestDatas = string.Empty
            };
            judgeProfile.SetTestDatas(model.TestDatas);
            problem.SetJudgeProfile(judgeProfile);

            problem.SetPassRate(new PassRate
            {
                Submit = 0,
                Pass = 0
            });

            _context.Problems.Add(problem);
            await _context.SaveChangesAsync();

            // _logger.Log(LogLevel.Information, $"[{DateTime.UtcNow}] User (Id: {user.Id}) created a new problem", problem);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string content)
        {
            if(content == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new IndexModel
            {
                ProblemModels = new List<ProblemModel>()
            };
            foreach (var p in _context.Problems.Where(x => x.Title.Contains(content)).ToList())
            {
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Build();

                var html = Markdown.ToHtml(p.Description, pipeline);
                var raw = Markdown.ToPlainText(p.Description);

                var author = await _userManager.FindByIdAsync(p.AuthorId);

                model.ProblemModels.Add(new ProblemModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    ContentRaw = raw,
                    ContentHtml = html,
                    AuthorId = p.AuthorId,
                    Author = author.UserName,
                    ExampleData = p.ExampleData,
                    JudgeProfile = p.JudgeProfile,
                    PassRate = p.PassRate,
                    PublishTime = p.PublishTime
                });
            }

            return View(model);
        }

        [HttpGet]
        // [Route("{id}")]
        public async Task<IActionResult> Details(long? id)
        {
            var problem = _context.Problems.FirstOrDefault(p => p.Id == id);

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Build();

            var html = Markdown.ToHtml(problem.Description, pipeline);
            var raw = Markdown.ToPlainText(problem.Description);

            var author = await _userManager.FindByIdAsync(problem.AuthorId);

            var model = new ProblemModel
            {
                Id = problem.Id,
                Title = problem.Title,
                Description = problem.Description,
                ContentRaw = raw,
                ContentHtml = html,
                AuthorId = problem.AuthorId,
                Author = author.UserName,
                ExampleData = problem.ExampleData,
                JudgeProfile = problem.JudgeProfile,
                PassRate = problem.PassRate,
                PublishTime = problem.PublishTime
            };

            return View(model);
        }
    }
}