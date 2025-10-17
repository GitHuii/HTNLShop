using HTNLShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace HTNLShop.Areas.Admin.Controllers
{

    [Area("admin")]
    [Route("/admin")]
    public class HomeAdminController : Controller
    {
        private readonly HtlnshopContext db;

        public HomeAdminController(HtlnshopContext context)
        {
            db = context;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("QuanLySanPham")]
        public IActionResult QuanLySanPham(int? page)
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

        [Route("QuanLyDanhMuc")]
        public IActionResult QuanLyDanhMuc(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;

            var list = db.Categories.OrderBy(p => p.CategoryId).ToPagedList(pageNumber, pageSize);

            return View(list);
        }

        [Route("QuanLyNguoiDung")]
        public IActionResult QuanLyNguoiDung(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;

            var list = db.Users.OrderBy(p => p.UserId).ToPagedList();

            return View(list);
        }
    }
}
