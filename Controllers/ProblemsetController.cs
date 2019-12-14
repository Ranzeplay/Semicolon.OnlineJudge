using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Semicolon.OnlineJudge.Controllers
{
    public class ProblemsetController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}