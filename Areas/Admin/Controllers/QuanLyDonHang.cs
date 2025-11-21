using HTNLShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HTNLShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class QuanLyDonHang : Controller
    {
        private readonly HtlnshopContext db;
        
        public QuanLyDonHang(HtlnshopContext context)
        {
            db = context;
        }
        public IActionResult Index(string status = "", string searchTerm = "")
        {

            var ordersQuery = db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsQueryable();

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                ordersQuery = ordersQuery.Where(o => o.Status == status);
            }

            // Tìm kiếm theo OrderId hoặc tên khách hàng
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ordersQuery = ordersQuery.Where(o =>
                    o.OrderId.ToString().Contains(searchTerm) ||
                    o.User.FullName.Contains(searchTerm) ||
                    o.User.Email.Contains(searchTerm)
                );
            }

            var orders = ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            ViewBag.CurrentStatus = status;
            ViewBag.SearchTerm = searchTerm;

            return View(orders);
        }
    }
}
