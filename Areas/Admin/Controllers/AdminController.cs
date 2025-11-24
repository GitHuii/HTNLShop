using HTNLShop.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using X.PagedList;
using X.PagedList.Extensions;

namespace HTNLShop.Areas.Admin.Controllers
{

    [Area("admin")]
    [Route("/admin")]
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly HtlnshopContext db;

        public AdminController(HtlnshopContext context)
        {
            db = context;
        }

        [Route("")]
        [Route("Index")]

        //public async Task<IActionResult> IndexAsync()
        //{
        //    return View();
        //}

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Route("Logout")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return Redirect("/Customer/Login");
        }
    }
}
