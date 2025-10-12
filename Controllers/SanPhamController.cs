using HTNLShop.Data;
using Microsoft.AspNetCore.Mvc;

namespace HTNLShop.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly HtlnshopContext db;
        public SanPhamController(HtlnshopContext context)
        {
            db = context;
        }
        public IActionResult Index(int? id)
        {
            var sanphams = db.Products.AsQueryable();
            if(id != null)
            {
                sanphams = sanphams.Where(p => p.CategoryId == id);
            }

            var list = sanphams.Select(p => new ViewModels.ProductVM
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                ProductDetail = p.ProductDetail,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName
            }).ToList();

            return View(list);
        }

        public IActionResult Search(string query)
        {
            var sanphams = db.Products.AsQueryable();
            if (!string.IsNullOrEmpty(query))
            {
                sanphams = sanphams.Where(p => p.ProductName.ToLower().Contains(query.ToLower()));
            }
            
            var list = sanphams.Select(p => new ViewModels.ProductVM
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                ProductDetail = p.ProductDetail,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName
            }).ToList();

            return View(list);
        }
    }
}
