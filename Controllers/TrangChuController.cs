
using Microsoft.AspNetCore.Mvc;
using HTNLShop.Data;
namespace HTNLShop.Controllers
{
    public class TrangChuController : Controller
    {
        private readonly HtlnshopContext _context;

        public TrangChuController(HtlnshopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var vm = new ViewModels.TrangChuVM
            {
                Laptops = _context.Products
                    .Where(p => p.CategoryId == 1)
                    .Take(4)
                    .Select(p => new ViewModels.ProductVM
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        ImageUrl = p.ImageUrl,
                        Price = p.Price,
                        ProductDetail = p.ProductDetail,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category.CategoryName
                    }).ToList(),
                PCs = _context.Products
                .Where(p => p.CategoryId == 2)
                .Take(4)
                .Select(p => new ViewModels.ProductVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    ProductDetail = p.ProductDetail,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                }).ToList(),
                Mices = _context.Products
                .Where(p => p.CategoryId == 5)
                .Take(4)
                .Select(p => new ViewModels.ProductVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    ProductDetail = p.ProductDetail,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                }).ToList(),
                Keyboards = _context.Products
                .Where(p => p.CategoryId == 7)
                .Take(4)
                .Select(p => new ViewModels.ProductVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    ProductDetail = p.ProductDetail,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                }).ToList(),
                Monitors = _context.Products
                .Where(p => p.CategoryId == 3)
                .Take(7)
                .Select(p => new ViewModels.ProductVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    ProductDetail = p.ProductDetail,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                }).ToList()
            };

            return View(vm);
        }
    }
}
