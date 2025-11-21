using HTNLShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace HTNLShop.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("/Admin")]
    public class AdminController : Controller
    {
        private readonly HtlnshopContext db;

        public AdminController(HtlnshopContext context)
        {
            db = context;
        }

        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View();
        }


        //[Route("QuanLySanPham")]
        //public IActionResult QuanLySanPham(int? page)
        //{
        //    int pageSize = 10;
        //    int pageNumber = page == null || page < 1 ? 1 : page.Value;

        //    var list = db.Products
        //                 .Include(p => p.Category)
        //                 .AsNoTracking()
        //                 .OrderBy(p => p.ProductId)
        //                 .ToPagedList(pageNumber, pageSize);

        //    return View(list);
        //}

        //[HttpGet("AddProduct")]
        //public IActionResult AddProduct()
        //{
        //    ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "RoleName");
        //    return View();
        //}

        //[HttpPost("AddProduct")]
        //public IActionResult AddProduct(Product product, IFormFile ImageUrl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Products.Add(product);
        //        db.SaveChanges();

        //        if (ImageUrl != null && ImageUrl.Length > 0)
        //        {
        //            // Tạo đường dẫn thư mục lưu ảnh
        //            var folderPath = Path.Combine(
        //                Directory.GetCurrentDirectory(),
        //                "wwwroot",
        //                "Assets",
        //                "img",
        //                "Products",
        //                product.CategoryId.ToString()
        //            );

        //            if (!Directory.Exists(folderPath))
        //                Directory.CreateDirectory(folderPath);

        //            var fileName = $"{product.ProductId}.jpg";

        //            var filePath = Path.Combine(folderPath, fileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                ImageUrl.CopyTo(stream);
        //            }

        //            product.ImageUrl = $"/Assets/img/Products/{product.CategoryId}/{fileName}";

        //            db.Products.Update(product);
        //            db.SaveChanges();
        //        }

        //        return RedirectToAction("QuanLySanPham");
        //    }

        //    ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName", product.CategoryId);
        //    return View(product);
        //}

        //[HttpGet("EditProduct/{id}")]
        //public IActionResult EditProduct(int id)
        //{
        //    var product = db.Products.Find(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName", product.CategoryId);
        //    return View(product);
        //}

        //[HttpPost("EditProduct/{id}")]
        //public IActionResult EditProduct(int id, Product product, IFormFile? ImageUrl)
        //{
        //    var existingProduct = db.Products.Find(id);
        //    if (existingProduct == null)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Cập nhật thông tin cơ bản
        //        existingProduct.ProductName = product.ProductName;
        //        existingProduct.Price = product.Price;
        //        existingProduct.ProductDetail = product.ProductDetail;
        //        existingProduct.StockQuantity = product.StockQuantity;
        //        existingProduct.CategoryId = product.CategoryId;

        //        // Nếu có ảnh mới thì lưu ảnh
        //        if (ImageUrl != null && ImageUrl.Length > 0)
        //        {
        //            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assets", "img", "Products", existingProduct.CategoryId.ToString());
        //            if (!Directory.Exists(folderPath))
        //                Directory.CreateDirectory(folderPath);

        //            string fileName = $"{existingProduct.ProductId}.jpg";
        //            string filePath = Path.Combine(folderPath, fileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                ImageUrl.CopyTo(stream);
        //            }

        //            // Lưu đường dẫn tương đối vào DB
        //            existingProduct.ImageUrl = $"/Assets/img/Products/{existingProduct.CategoryId}/{fileName}";
        //        }

        //        db.SaveChanges();
        //        return RedirectToAction("QuanLySanPham");
        //    }

        //    ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName", product.CategoryId);
        //    return View(product);
        //}

        //[HttpPost("DeleteProduct")]
        //public IActionResult DeleteProduct(int id)
        //{
        //    var product = db.Products.Find(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    // Xóa ảnh trong thư mục nếu có
        //    if (!string.IsNullOrEmpty(product.ImageUrl))
        //    {
        //        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl.TrimStart('/'));
        //        if (System.IO.File.Exists(imagePath))
        //        {
        //            System.IO.File.Delete(imagePath);
        //        }
        //    }

        //    db.Products.Remove(product);
        //    db.SaveChanges();

        //    return RedirectToAction("QuanLySanPham");
        //}
        //[Route("QuanLyDanhMuc")]
        //public IActionResult QuanLyDanhMuc(int? page)
        //{
        //    int pageSize = 10;
        //    int pageNumber = page == null || page < 1 ? 1 : page.Value;

        //    var list = db.Categories.OrderBy(p => p.CategoryId).ToPagedList(pageNumber, pageSize);

        //    return View(list);
        //}

        //[Route("QuanLyNguoiDung")]
        //public IActionResult QuanLyNguoiDung(int? page)
        //{
        //    int pageSize = 10;
        //    int pageNumber = page == null || page < 1 ? 1 : page.Value;

        //    var list = db.Users.OrderBy(p => p.UserId).ToPagedList();

        //    return View(list);
        //}
    }
}
