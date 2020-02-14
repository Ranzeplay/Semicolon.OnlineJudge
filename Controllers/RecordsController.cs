using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Semicolon.Auth.Models;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models.ViewModels.Records;

namespace Semicolon.OnlineJudge.Controllers
{
    public class RecordsController : Controller
    {
        private readonly UserManager<SemicolonUser> _userManager;

        private readonly ApplicationDbContext _context;

        public RecordsController(ApplicationDbContext context, UserManager<SemicolonUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new List<UploadModel>();
            var tracks = _context.Tracks.ToList();
            foreach(var track in tracks)
            {
                var user = await _userManager.FindByIdAsync(track.AuthorId);
                model.Add(new UploadModel(track, user.UserName));
            }

            return View(model);
        }
    }
}