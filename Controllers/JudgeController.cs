using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Semicolon.Auth.Models;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models.Judge;
using Semicolon.OnlineJudge.Models.ViewModels.Judge;

namespace Semicolon.OnlineJudge.Controllers
{
    public class JudgeController : Controller
    {
        private readonly UserManager<SemicolonUser> _userManager;

        private readonly ApplicationDbContext _context;

        public JudgeController(ApplicationDbContext context, UserManager<SemicolonUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        // [Route("{id}")]
        [Authorize]
        public IActionResult Submit(long? id)
        {
            if (id != null)
            {
                var problem = _context.Problems.FirstOrDefault(p => p.Id == id);
                if (problem != null)
                {
                    return View(new SubmitModel { Id = id.GetValueOrDefault() });
                }
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Submit(SubmitModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var problem = await _context.Problems.FirstOrDefaultAsync(x => x.Id == model.Id);
                var track = new Track
                {
                    AuthorId = user.Id,
                    CreateTime = DateTime.UtcNow,
                    ProblemId = model.Id,
                    CodeEncoded = Base64Encode(model.Code),
                    Status = JudgeStatus.Pending,
                    Language = model.Language
                };

                if (problem != null)
                {
                    List<Point> points = new List<Point>();

                    var problemDirectory = Path.Combine(Directory.GetCurrentDirectory(), "JudgeDataStorage", problem.Id.ToString(), "data");
                    var subDirectories = Directory.EnumerateDirectories(problemDirectory);
                    for (int i = 0; i < subDirectories.Count(); i++)
                    {
                        points.Add(new Point
                        {
                            Id = i,
                            Status = PointStatus.Pending
                        });
                    }

                    var passRate = problem.GetPassRate();
                    passRate.Submit += 1;
                    problem.SetPassRate(passRate);
                    track.SetPointStatus(points);
                    _context.Problems.Update(problem);

                    _context.Tracks.Add(track);
                    await _context.SaveChangesAsync();

                    var trackNew = await _context.Tracks.FirstOrDefaultAsync(t => t.CreateTime == track.CreateTime);

                    // _logger.Log(LogLevel.Information, $"[{DateTime.UtcNow}] User (Id: {user.Id}) started a new track for problem #{problem.Id}", track);

                    return RedirectToAction(nameof(Status), new { trackNew.Id });
                }
            }

            return View(model);
        }

        [HttpGet]
        // [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Status(long id)
        {
            var track = await _context.Tracks.FirstOrDefaultAsync(x => x.Id == id);
            if(track != null)
            {
                return View(track);
            }

            return NotFound();
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}