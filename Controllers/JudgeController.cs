using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models.Judge;
using Semicolon.OnlineJudge.Models.User;
using Semicolon.OnlineJudge.Models.ViewModels.Judge;

namespace Semicolon.OnlineJudge.Controllers
{
    public class JudgeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JudgeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        // [Route("{id}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
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
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Submit(SubmitModel model)
        {
            if (ModelState.IsValid)
            {
                var user = GetUserProfile();
                var problem = await _context.Problems.FirstOrDefaultAsync(x => x.Id == model.Id);
                var track = new Track
                {
                    AuthorId = user.Id,
                    CreateTime = DateTime.UtcNow,
                    ProblemId = model.Id,
                    CodeEncoded = model.Code,
                    Status = JudgeStatus.Pending,
                    Language = model.Language
                };

                if (problem != null)
                {
                    List<Point> points = new List<Point>();

                    for (int i = 0; i < problem.GetJudgeProfile().GetTestDatas().Count; i++)
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

                    return RedirectToAction(nameof(Status), new { trackNew.Id });
                }
            }

            return View(model);
        }

        [HttpGet]
        // [Route("{id}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Status(long id)
        {
            var track = await _context.Tracks.FirstOrDefaultAsync(x => x.Id == id);
            if(track != null)
            {
                return View(track);
            }

            return NotFound();
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