using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models;
using Semicolon.OnlineJudge.Models.Auth;

namespace Semicolon.OnlineJudge.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthProps _authProps;

        private readonly ApplicationDbContext _context;

        public AuthController(IOptions<AuthProps> options, ApplicationDbContext context)
        {
            _authProps = options.Value;
            _context = context;
        }

        [Route("/Auth")]
        public RedirectResult Index()
        {
            var url = new SemicolonUrl
            {
                UrlAddress = _authProps.ResourceServerUrl + "Auth/Login",
                Params = new Dictionary<string, string>
                {
                    { "appid", _authProps.AppId },
                    { "appsecret", _authProps.AppSecret },
                    { "returnUrl", _authProps.ClientServerUrl + "Auth/Code" }
                }
            };

            return Redirect(url.ToStringUrl());
        }

        public RedirectResult Code(string code)
        {
            var url = new SemicolonUrl
            {
                UrlAddress = _authProps.ResourceServerUrl + "Auth/Token",
                Params = new Dictionary<string, string>
                {
                    { "appid", _authProps.AppId },
                    { "appsecret", _authProps.AppSecret },
                    { "code", code },
                    { "returnUrl", _authProps.ClientServerUrl + "Auth/Token" }
                }
            };

            return Redirect(url.ToStringUrl());
        }

        public RedirectResult Token(string token)
        {
            var url = new SemicolonUrl
            {
                UrlAddress = _authProps.ResourceServerUrl + "Account/GetUserProfile",
                Params = new Dictionary<string, string>
                {
                    { "appid", _authProps.AppId },
                    { "token", token },
                    { "returnUrl", _authProps.ClientServerUrl + "Auth/SignIn" }
                }
            };

            return Redirect(url.ToStringUrl());
        }

        public async Task<IActionResult> SignIn(string profile)
        {
            var jsonProfile = Base64Decode(profile);
            var model = System.Text.Json.JsonSerializer.Deserialize<UserProfileModel>(jsonProfile);

            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", model.Id),
                new Claim("EmailAddress", model.Email),
                new Claim("UserName", model.UserName)
            }, "Auth");
            var claimsPricipal = new ClaimsPrincipal(claimsIdentity);

            AuthenticationProperties props = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.Now.Add(TimeSpan.FromDays(1)),
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPricipal, props);

            var user = _context.OJUsers.FirstOrDefault(u => u.Id == model.Id);
            if(user == null)
            {
                _context.OJUsers.Add(new Models.User.OJUser
                {
                    Id = model.Id,
                    Email = model.Email,
                    UserName = model.UserName,
                    NickName = model.UserName,
                    ProblemsPassedId = ""
                });
            }

            return RedirectToAction("Index", "Home");
        }

        private string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}