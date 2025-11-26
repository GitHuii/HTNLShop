using HTNLShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HTNLShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    //[Area("admin")]
    //[Route("/admin")]
    public class DashBoard : Controller
    {
        private readonly HtlnshopContext _context; // Thay YourDbContext bằng tên DbContext của bạn

        public DashBoard(HtlnshopContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                // Tổng số liệu
                TotalUsers = await _context.Users.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalRevenue = await _context.Orders
                    .Where(o => o.Status == "Completed" || o.Status.ToLower().Contains("Đã giao hàng"))
                    .SumAsync(o => o.TotalPrice ?? 0),

                // Đơn hàng chờ xử lý
                PendingOrders = await _context.Orders
                    .Where(o => o.Status == "Pending" || o.Status == "Chờ xử lý")
                    .CountAsync(),

                // Sản phẩm sắp hết hàng
                LowStockProducts = await _context.Products
                    .Where(p => p.StockQuantity < 10)
                    .CountAsync(),

                // Doanh thu theo tháng (12 tháng gần nhất)
                MonthlyRevenue = await GetMonthlyRevenue(),

                // Đơn hàng theo tháng
                MonthlyOrders = await GetMonthlyOrders(),

                // Top sản phẩm bán chạy
                TopSellingProducts = await _context.OrderItems
                    .GroupBy(oi => oi.ProductId)
                    .Select(g => new TopProductViewModel
                    {
                        ProductId = g.Key,
                        ProductName = g.First().Product.ProductName,
                        TotalQuantity = g.Sum(oi => oi.Quantity),
                        TotalRevenue = g.Sum(oi => g.First().Product.Price * oi.Quantity)
                    })
                    .OrderByDescending(p => p.TotalQuantity)
                    .Take(5)
                    .ToListAsync(),

                // Đơn hàng gần đây
                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .Select(o => new RecentOrderViewModel
                    {
                        OrderId = o.OrderId,
                        CustomerName = o.User.FullName ?? o.User.Username,
                        OrderDate = o.OrderDate ?? DateTime.Now,
                        TotalPrice = o.TotalPrice ?? 0,
                        Status = o.Status
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        private async Task<List<MonthlyDataViewModel>> GetMonthlyRevenue()
        {
            var currentDate = DateTime.Now;
            var monthlyData = new List<MonthlyDataViewModel>();

            for (int i = 11; i >= 0; i--)
            {
                var targetMonth = currentDate.AddMonths(-i);
                var revenue = await _context.Orders
                    .Where(o => o.OrderDate.HasValue
                        && o.OrderDate.Value.Year == targetMonth.Year
                        && o.OrderDate.Value.Month == targetMonth.Month
                        && (o.Status == "Completed"
                            || o.Status == "Đã giao"
                            || o.Status.ToLower().Contains("giao")
                            || o.Status.ToLower().Contains("hoàn thành")))
                    .SumAsync(o => o.TotalPrice ?? 0);

                monthlyData.Add(new MonthlyDataViewModel
                {
                    Month = targetMonth.ToString("MM/yyyy"),
                    Value = revenue
                });
            }

            return monthlyData;
        }

        private async Task<List<MonthlyDataViewModel>> GetMonthlyOrders()
        {
            var currentDate = DateTime.Now;
            var monthlyData = new List<MonthlyDataViewModel>();

            for (int i = 11; i >= 0; i--)
            {
                var targetMonth = currentDate.AddMonths(-i);
                var orderCount = await _context.Orders
                    .Where(o => o.OrderDate.HasValue
                        && o.OrderDate.Value.Year == targetMonth.Year
                        && o.OrderDate.Value.Month == targetMonth.Month)
                    .CountAsync();

                monthlyData.Add(new MonthlyDataViewModel
                {
                    Month = targetMonth.ToString("MM/yyyy"),
                    Value = orderCount
                });
            }

            return monthlyData;
        }

        public async Task<IActionResult> LowStockProducts()
        {
            var lowStockProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity < 10)
                .OrderBy(p => p.StockQuantity)
                .Select(p => new LowStockProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    CategoryName = p.Category.CategoryName,
                    StockQuantity = p.StockQuantity,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            return View(lowStockProducts);
        }
    }

    public class LowStockProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public int StockQuantity { get; set; }
        public double? Price { get; set; }
        public string ImageUrl { get; set; }
    }
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public double TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockProducts { get; set; }
        public List<MonthlyDataViewModel> MonthlyRevenue { get; set; }
        public List<MonthlyDataViewModel> MonthlyOrders { get; set; }
        public List<TopProductViewModel> TopSellingProducts { get; set; }
        public List<RecentOrderViewModel> RecentOrders { get; set; }
    }

    public class MonthlyDataViewModel
    {
        public string Month { get; set; }
        public double Value { get; set; }
    }

    public class TopProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
        public double TotalRevenue { get; set; }
    }

    public class RecentOrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; }
    }
}