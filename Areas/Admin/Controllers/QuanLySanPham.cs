using Azure;
using HTNLShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace HTNLShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class QuanLySanPham : Controller
    {
        private readonly HtlnshopContext db;

        public QuanLySanPham(HtlnshopContext context)
        {
            db = context;
        }

        public IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;

            var list = db.Products
                         .Include(p => p.Category)
                         .AsNoTracking()
                         .OrderBy(p => p.ProductId)
                         .ToPagedList(pageNumber, pageSize);

            return View(list);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            if (db != null && db.Categories != null)
            {
                ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName"); // sửa RoleName thành CategoryName
            }
            else
            {
                ViewBag.CategoryId = new SelectList(new List<Category>(), "CategoryId", "CategoryName");
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(Product product, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();

                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    // Tạo đường dẫn thư mục lưu ảnh
                    var folderPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "Assets",
                        "img",
                        "Products",
                        product.CategoryId.ToString()
                    );

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var fileName = $"{product.ProductId}.jpg";

                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageUrl.CopyTo(stream);
                    }

                    product.ImageUrl = $"/Assets/img/Products/{product.CategoryId}/{fileName}";

                    db.Products.Update(product);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        public IActionResult EditProduct(Product product, IFormFile? ImageUrl)
        {
            var existingProduct = db.Products.Find(product.ProductId);
            if (existingProduct == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Cập nhật thông tin cơ bản
                existingProduct.ProductName = product.ProductName;
                existingProduct.Price = product.Price;
                existingProduct.ProductDetail = product.ProductDetail;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.CategoryId = product.CategoryId;

                // Nếu có ảnh mới thì lưu ảnh
                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assets", "img", "Products", existingProduct.CategoryId.ToString());
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string fileName = $"{existingProduct.ProductId}.jpg";
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageUrl.CopyTo(stream);
                    }

                    // Lưu đường dẫn tương đối vào DB
                    existingProduct.ImageUrl = $"/Assets/img/Products/{existingProduct.CategoryId}/{fileName}";
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }


        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            // Xóa ảnh trong thư mục nếu có
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
