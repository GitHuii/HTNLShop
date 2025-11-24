using AutoMapper;
using HTNLShop.Data;
using HTNLShop.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HTNLShop.Controllers
{
    public class CustomerController : Controller
    {
        private readonly HtlnshopContext _context;
        private readonly IMapper _mapper;
        private readonly CartController _cartController;

        public CustomerController(HtlnshopContext context, IMapper mapper, CartController cartController)
        {
            _context = context;
            _mapper = mapper;
            _cartController = cartController;
        }
        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterVM();
            ViewData["Title"] = "Đăng ký";
            return View(model);
        }
        [HttpPost]
        public IActionResult Register(RegisterVM model)
        {
            var customer = new RegisterVM();
            if (ModelState.IsValid)
            {
                bool isExist = _context.Users.Any(u => u.Username == model.Username);
                if (isExist)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác.");
                    ViewData["Title"] = "Đăng ký";
                    return View(model);
                }
                var customerEntity = _mapper.Map<User>(model);
                customerEntity.RoleId = 1; // id for customer
                _context.Users.Add(customerEntity);
                _context.SaveChanges();
            }
            return View(customer);
        }
        #endregion
        #region Login
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            //ViewBag.ReturnUrl = ReturnUrl ?? Request.Form["ReturnUrl"].ToString();
            ViewBag.ReturnUrl = ReturnUrl ?? Request.Query["ReturnUrl"].ToString();
            //ViewBag.ReturnUrl = ReturnUrl;
            var model = new LoginVM();
            ViewData["Title"] = "Đăng Nhập";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
        {
            ReturnUrl ??= Request.Form["ReturnUrl"].ToString();
            ViewBag.ReturnUrl = ReturnUrl;

            if (ModelState.IsValid)
            {
                var customer = _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u =>
                        u.Username == model.Username &&
                        u.Password == model.Password);

                if (customer != null)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.UserId.ToString()),
                new Claim("CustomerId", customer.UserId.ToString()),
                new Claim(ClaimTypes.Name, customer.FullName ?? ""),
                new Claim(ClaimTypes.Email, customer.Email ?? ""),
                new Claim(ClaimTypes.MobilePhone, customer.PhoneNumber ?? ""),
                new Claim(ClaimTypes.StreetAddress, customer.Address ?? ""),
                new Claim(ClaimTypes.Role, customer.Role.RoleId == 1 ? "Admin" : "Customer")
            };

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );

                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        claimsPrincipal
                    );

                    if (customer.Role.RoleId == 1)
                        return RedirectToAction("Index", "Admin", new { area = "Admin" });

                    if (customer.Role.RoleId == 2)
                    {
                        await _cartController.MergeCartAsync(customer.UserId, HttpContext.Session);
                        return RedirectToAction("Index", "Cart");
                    }
                }

                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        #endregion
        [Authorize]
        public IActionResult ProFile()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Lấy role trước khi logout
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            // Redirect dựa trên role
            if (userRole == "Admin")
            {
                return RedirectToAction("Login", "Customer"); // Hoặc trang login admin nếu có
            }

            return RedirectToAction("Login", "Customer");
        }

    }
}
