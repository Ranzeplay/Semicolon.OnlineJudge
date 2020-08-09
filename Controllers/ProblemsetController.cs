using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
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
        public async Task<IActionResult> New(NewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
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
            };

            problem.SetJudgeProfile(judgeProfile);

            problem.SetPassRate(new PassRate
            {
                Submit = 0,
                Pass = 0
            });

            _context.Problems.Add(problem);
            await _context.SaveChangesAsync();

            // Save test data to local disk
            var judgeDataStorageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "JudgeDataStorage");
            if (!Directory.Exists(judgeDataStorageDirectory))
            {
                Directory.CreateDirectory(judgeDataStorageDirectory);
            }

            // Copy zip file to target directory
            var problemDirectory = Path.Combine(judgeDataStorageDirectory, _context.Problems.LongCount().ToString());
            Directory.CreateDirectory(problemDirectory);
            var zipFilePath = Path.Combine(problemDirectory, "judge.zip");

            var stream = new FileStream(zipFilePath, FileMode.Create);
            model.TestDatas.CopyTo(stream);
            stream.Close();

            // Unzip file
            var targetDirectory = Path.Combine(problemDirectory, "data");
            var fastZip = new FastZip();
            fastZip.ExtractZip(zipFilePath, targetDirectory, null);

            // _logger.Log(LogLevel.Information, $"[{DateTime.UtcNow}] User (Id: {user.Id}) created a new problem", problem);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string content)
        {
            if (content == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new IndexModel
            {
                ProblemModels = new List<ProblemModel>()
            };

            if (long.TryParse(content, out long number))
            {
                var p = _context.Problems.FirstOrDefault(p => p.Id == number);

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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(long id)
        {
            var problem = _context.Problems.FirstOrDefault(p => p.Id == id);
            var user = await _userManager.GetUserAsync(User);

            if (problem != null)
            {
                if (problem.AuthorId == user.Id)
                {
                    _context.Problems.Remove(problem);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("ProblemsCreated", "Users");
                }

                return Unauthorized();
            }

            return NotFound();
        }
    }
}