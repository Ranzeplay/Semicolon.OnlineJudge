using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Semicolon.Auth.Models;
using Semicolon.Auth.Models.ViewModels.Account;
using Semicolon.OnlineJudge.Data;

namespace Semicolon.Auth.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<SemicolonUser> _userManager;
        private readonly SignInManager<SemicolonUser> _signInManager;

        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<SemicolonUser> userManager, SignInManager<SemicolonUser> signInManager , ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> My()
        {
            var user = await _userManager.GetUserAsync(User);

            return View(new UserModel { Email = user.Email, UserName = user.UserName });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> My(UserModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                user.Email = model.Email;
                user.UserName = model.UserName;

                await _userManager.UpdateAsync(user);
            }

            return View(new UserModel { Email = user.Email, UserName = user.UserName });
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(My));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(DeleteAccountModel model)
        {
            if (ModelState.IsValid)
            {
                var userLoggedIn = await _userManager.GetUserAsync(User);
                if(userLoggedIn.Email == model.Email)
                {
                    var passwordValidateResult = await _userManager.CheckPasswordAsync(userLoggedIn, model.Password);
                    if (passwordValidateResult)
                    {
                        await _signInManager.SignOutAsync();
                        await _userManager.DeleteAsync(userLoggedIn);

                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "Incorrect password");
                }

                ModelState.AddModelError(string.Empty, "The email you input is not equal to your own account");
            }

            return View(model);
        }
    }
}