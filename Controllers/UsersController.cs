using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Semicolon.OnlineJudge.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index(string id)
        {
            return View();
        }
    }
}