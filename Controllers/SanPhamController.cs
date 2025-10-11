using Microsoft.AspNetCore.Mvc;

namespace HTNLShop.Controllers
{
    public class SanPhamController : Controller
    {
        public IActionResult Index(int? id)
        {
            return View();
        }
    }
}
