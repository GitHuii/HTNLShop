using HTNLShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            if (id != null)
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

        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }
        public IActionResult Detail(int id)
        {
            var sanpham = db.Products.Include(p => p.Category).
                SingleOrDefault(p => p.ProductId == id);
            if (sanpham == null)
            {
                TempData["Message"] = $"Không tìm thấy sản phẩm với ID: {id}";
                return Redirect("/404");
                //return PageNotFound();
            }
            var res = new ViewModels.ProductDetailVM
            {
                ProductId = sanpham.ProductId,
                ProductName = sanpham.ProductName,
                ImageUrl = sanpham.ImageUrl,
                Price = sanpham.Price,
                ProductDetail = sanpham.ProductDetail,
                CategoryId = sanpham.CategoryId,
                CategoryName = sanpham.Category.CategoryName,
                StockQuantity = sanpham.StockQuantity,
                SameCategoryProduct = db.Products
                    .Where(p => p.CategoryId == sanpham.CategoryId && p.ProductId != sanpham.ProductId)
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
            return View(res);
        }
    }
}
