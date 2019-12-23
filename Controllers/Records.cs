using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models.ViewModels.Records;

namespace Semicolon.OnlineJudge.Controllers
{
    public class Records : Controller
    {
        private readonly ApplicationDbContext _context;

        public Records(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new List<UploadModel>();
            var tracks = _context.Tracks.ToList();
            foreach(var track in tracks)
            {
                var user = _context.OJUsers.FirstOrDefault(u => u.Id == track.AuthorId);
                model.Add(new UploadModel(track, user.NickName));
            }

            return View(model);
        }
    }
}