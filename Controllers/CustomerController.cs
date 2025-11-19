using AutoMapper;
using HTNLShop.Data;
using HTNLShop.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                var customer = _context.Users.FirstOrDefault(u =>
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
                new Claim(ClaimTypes.Role, "Customer")
            };

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );

                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // ✅ Cách đúng
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        claimsPrincipal
                    );
                    // Sau khi xác thực thành công:
                    await _cartController.MergeCartAsync(customer.UserId, HttpContext.Session);
                    return RedirectToAction("Index", "Cart");
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
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Xóa session nếu bạn dùng Session (bạn đang dùng trong giỏ hàng)
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Customer");
        }

    }
}
//using AutoMapper;
//using HTNLShop.Data;
//using HTNLShop.ViewModels;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http.Extensions;
//using Microsoft.Extensions.Logging;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace HTNLShop.Controllers
//{
//    public class CustomerController : Controller
//    {
//        private readonly HtlnshopContext _context;
//        private readonly IMapper _mapper;
//        private readonly ILogger<CustomerController> _logger;

//        public CustomerController(HtlnshopContext context, IMapper mapper, ILogger<CustomerController> logger)
//        {
//            _context = context;
//            _mapper = mapper;
//            _logger = logger;
//        }
//        #region Register
//        [HttpGet]
//        public IActionResult Register()
//        {
//            var model = new RegisterVM();
//            ViewData["Title"] = "Đăng ký";
//            return View(model);
//        }
//        [HttpPost]
//        public IActionResult Register(RegisterVM model)
//        {
//            var customer = new RegisterVM();
//            if (ModelState.IsValid)
//            {
//                bool isExist = _context.Users.Any(u => u.Username == model.Username);
//                if (isExist)
//                {
//                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác.");
//                    ViewData["Title"] = "Đăng ký";
//                    return View(model);
//                }
//                var customerEntity = _mapper.Map<User>(model);
//                customerEntity.RoleId = 1; // id for customer
//                _context.Users.Add(customerEntity);
//                _context.SaveChanges();
//            }
//            return View(customer);
//        }
//        #endregion
//        #region Login
//        [HttpGet]
//        //public IActionResult Login(string? ReturnUrl)
//        //{
//        //    // ưu tiên querystring nếu có
//        //    ViewBag.ReturnUrl = ReturnUrl ?? Request.Query["ReturnUrl"].ToString();
//        //    var model = new LoginVM();
//        //    ViewData["Title"] = "Đăng Nhập";
//        //    return View(model);
//        //}
//        public IActionResult Login(string? ReturnUrl)
//        {
//            // lấy ReturnUrl từ query nếu có
//            var rv = ReturnUrl ?? Request.Query["ReturnUrl"].ToString();

//            // nếu ReturnUrl trỏ về chính trang Login hoặc là root, coi như không có ReturnUrl
//            var loginPath = Url.Action("Login", "Customer"); // -> "/Customer/Login"
//            if (string.IsNullOrWhiteSpace(rv) || rv == "/" || rv == loginPath || rv.StartsWith(loginPath + "?"))
//            {
//                rv = null;
//            }

//            ViewBag.ReturnUrl = rv;
//            var model = new LoginVM();
//            ViewData["Title"] = "Đăng Nhập";
//            return View(model);
//        }
//        [HttpPost]
//        public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
//        {
//            // fallback lấy từ form nếu parameter rỗng
//            ReturnUrl ??= Request.Form["ReturnUrl"].ToString();
//            ViewBag.ReturnUrl = ReturnUrl;

//            // --- DEBUG: log để biết nguồn và giá trị thực ---
//            _logger.LogDebug("Login POST invoked. Request URL: {FullUrl}", Request.GetDisplayUrl());
//            _logger.LogDebug("Query ReturnUrl: '{QueryReturnUrl}'", Request.Query["ReturnUrl"].ToString());
//            _logger.LogDebug("Form ReturnUrl: '{FormReturnUrl}'", Request.Form["ReturnUrl"].ToString());
//            _logger.LogDebug("Binder ReturnUrl param: '{BinderReturnUrl}'", ReturnUrl);
//            // -------------------------------------------------

//            if (ModelState.IsValid)
//            {
//                var customer = _context.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
//                if (customer != null)
//                {
//                    var claims = new List<Claim>
//                    {
//                        new Claim(ClaimTypes.NameIdentifier, customer.UserId.ToString()),
//                        new Claim(ClaimTypes.Name, customer.FullName),
//                        new Claim(ClaimTypes.Email, customer.Email),
//                        new Claim(ClaimTypes.StreetAddress, customer.Address),
//                        new Claim(ClaimTypes.Role, "Customer")
//                    };
//                    var ClaimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//                    var claimsPrincipal = new ClaimsPrincipal(ClaimsIdentity);

//                   // await HttpContext.SignInAsync(claimsPrincipal);
//                   await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
//                    return RedirectToAction("ProFile", "Customer");

//                    // nếu ReturnUrl null/empty/"/" thì redirect về mặc định
//                    if (string.IsNullOrEmpty(ReturnUrl) || ReturnUrl == "/")
//                    {
//                        _logger.LogDebug("ReturnUrl is empty or root '/', redirecting to SanPham/Index");
//                        return RedirectToAction("Index", "SanPham");
//                    }

//                    // nếu local -> redirect an toàn
//                    if (Url.IsLocalUrl(ReturnUrl))
//                    {
//                        _logger.LogDebug("ReturnUrl is local, redirecting to {ReturnUrl}", ReturnUrl);
//                        return LocalRedirect(ReturnUrl);
//                    }

//                    // nếu absolute cùng host -> lấy PathAndQuery và redirect
//                    if (Uri.TryCreate(ReturnUrl, UriKind.Absolute, out var uri)
//                        && string.Equals(uri.Host, Request.Host.Host, StringComparison.OrdinalIgnoreCase)
//                        && (Request.Host.Port == null || uri.Port == Request.Host.Port))
//                    {
//                        var localPath = uri.PathAndQuery;
//                        _logger.LogDebug("ReturnUrl absolute same-host -> redirect to {LocalPath}", localPath);
//                        return LocalRedirect(localPath);
//                    }

//                    _logger.LogWarning("ReturnUrl '{ReturnUrl}' rejected, redirect to default", ReturnUrl);
//                    return RedirectToAction("Index", "SanPham");
//                }
//                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
//            }
//            return View(model);
//        }
//        #endregion
//        [Authorize]
//        public IActionResult ProFile()
//        {
//            return View();
//        }
//    }
//}