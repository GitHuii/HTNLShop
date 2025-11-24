using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using HTNLShop.Data;
using HTNLShop.ViewModels;
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
            // Lấy tháng và năm hiện tại
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            // Lấy danh mục có sản phẩm bán chạy theo tháng hiện tại
            var orderItemsWithProducts = _context.OrderItems
                .Include(oi => oi.Order) // Include Order để lấy OrderDate
                .Include(oi => oi.Product)
                .ThenInclude(p => p.Category)
                .Where(oi => oi.Order.OrderDate.HasValue
                          && oi.Order.OrderDate.Value.Month == currentMonth
                          && oi.Order.OrderDate.Value.Year == currentYear)
                .Select(oi => new
                {
                    oi.ProductId,
                    oi.Quantity,
                    ProductName = oi.Product.ProductName,
                    ProductDetail = oi.Product.ProductDetail,
                    Price = oi.Product.Price,
                    CategoryId = oi.Product.CategoryId,
                    CategoryName = oi.Product.Category.CategoryName,
                    ImageUrl = oi.Product.ImageUrl
                })
                .ToList();

            var categories = orderItemsWithProducts
                .GroupBy(oi => new { oi.CategoryId, oi.CategoryName })
                .Select(categoryGroup => new CategoryBestSellingVM
                {
                    CategoryId = categoryGroup.Key.CategoryId,
                    CategoryName = categoryGroup.Key.CategoryName,
                    TopProducts = categoryGroup
                        .GroupBy(oi => new
                        {
                            oi.ProductId,
                            oi.ProductName,
                            oi.ProductDetail,
                            oi.Price,
                            oi.CategoryId,
                            oi.ImageUrl
                        })
                        .Select(productGroup => new BestSellingProductVM
                        {
                            ProductId = productGroup.Key.ProductId,
                            ProductName = productGroup.Key.ProductName,
                            ProductDetail = productGroup.Key.ProductDetail,
                            Price = productGroup.Key.Price,
                            CategoryId = productGroup.Key.CategoryId,
                            CategoryName = categoryGroup.Key.CategoryName,
                            ImageUrl = productGroup.Key.ImageUrl,
                            TotalQuantitySold = productGroup.Sum(oi => oi.Quantity)
                        })
                        .OrderByDescending(p => p.TotalQuantitySold)
                        .Take(4)
                        .ToList()
                })
                .Where(c => c.TopProducts.Any())
                .ToList();

            // Lấy sản phẩm Monitors
            var monitors = _context.Products
                .Where(p => p.CategoryId == 3)
                .Take(7)
                .Select(p => new ProductVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    ProductDetail = p.ProductDetail,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                }).ToList();

            // Gộp vào ViewModel
            var model = new TrangChuVM
            {
                Categories = categories,
                Monitors = monitors
            };

            return View(model);
        }
    }
}
