using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Semicolon.Auth.Models;
using Semicolon.Auth.Models.ViewModels.Auth;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models;
using Semicolon.OnlineJudge.Models.Auth;

namespace Semicolon.OnlineJudge.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<SemicolonUser> _userManager;
        private readonly SignInManager<SemicolonUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AuthController(UserManager<SemicolonUser> userManager, SignInManager<SemicolonUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Login));
                    }

                    ModelState.AddModelError(string.Empty, "An error occured while logging in!");
                }

                ModelState.AddModelError(string.Empty, "No such user!");

            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new SemicolonUser
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    CreateTime = DateTime.UtcNow,
                    // The user doesn't authorized any applications, so leave it empty
                    authorizedApps = ""
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Auth");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}