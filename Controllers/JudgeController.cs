using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Semicolon.OnlineJudge.Data;
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
            if(id != null)
            {
                var problem = _context.Problems.FirstOrDefault(p => p.Id == id);
                if(problem != null)
                {
                    return View(new SubmitModel { Id = id.GetValueOrDefault() });
                }
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Submit(SubmitModel model)
        {
            return View(model);
        }
    }
}