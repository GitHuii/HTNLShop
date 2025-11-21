using HTNLShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HTNLShop.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly HTNLShop.Data.HtlnshopContext _context;

        public ReviewController(HtlnshopContext context)
        {
            _context = context;
        }

        public IActionResult _Review(int ProductId)
        {
            return PartialView();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
