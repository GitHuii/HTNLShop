using HTNLShop.Data;
using HTNLShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HTNLShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly HtlnshopContext _context;

        public OrderController(HtlnshopContext context)
        {
            _context = context;
        }

        //public async Task<IActionResult> Index()
        //{
        //    // Lấy userId từ session hoặc authentication
        //    // Ví dụ: var userId = HttpContext.Session.GetInt32("UserId");
        //    var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var userId = int.Parse(userIdString);

        //    var orders = await _context.Orders
        //        .Include(o => o.OrderItems)
        //            .ThenInclude(oi => oi.Product)
        //        .Where(o => o.UserId == userId)
        //        .OrderByDescending(o => o.OrderDate)
        //        .Select(o => new OrderVM
        //        {
        //            OrderId = o.OrderId,
        //            OrderCode = $"ORD{o.OrderId:D6}",
        //            Date = o.OrderDate.HasValue
        //                ? o.OrderDate.Value.ToString("dd/MM/yyyy")
        //                : "",
        //            Status = o.Status,
        //            Total = (decimal)(o.TotalPrice ?? 0),
        //            ShippingAddress = o.ShippingAddress,

        //            // Lấy sản phẩm đầu tiên để hiển thị
        //            ProductName = o.OrderItems.FirstOrDefault() != null
        //                ? o.OrderItems.FirstOrDefault().Product.ProductName
        //                : "",
        //            Image = o.OrderItems.FirstOrDefault() != null
        //                ? o.OrderItems.FirstOrDefault().Product.ImageUrl
        //                : "/Assets/img/default-product.png",
        //            Price = o.OrderItems.FirstOrDefault() != null
        //                ? (decimal)o.OrderItems.FirstOrDefault().SalePrice
        //                : 0,
        //            Quantity = o.OrderItems.FirstOrDefault() != null
        //                ? o.OrderItems.FirstOrDefault().Quantity
        //                : 0,

        //            // Lấy tất cả items của order
        //            Items = o.OrderItems.Select(oi => new OrderItemVM
        //            {
        //                ProductName = oi.Product.ProductName,
        //                Image = oi.Product.ImageUrl,
        //                Price = (decimal)oi.SalePrice,
        //                Quantity = oi.Quantity
        //            }).ToList()
        //        })
        //        .ToListAsync();

        //    return View(orders);
        //}
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = int.Parse(userIdString);

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderVM
                {
                    OrderId = o.OrderId,
                    OrderCode = $"ORD{o.OrderId:D6}",
                    Date = o.OrderDate.HasValue
                        ? o.OrderDate.Value.ToString("dd/MM/yyyy")
                        : "",
                    Status = o.Status,

                    // ✅ FIX: Ép kiểu rõ ràng
                    Total = o.TotalPrice > 0
                        ? Convert.ToDecimal(o.TotalPrice)
                        : Convert.ToDecimal(o.OrderItems.Sum(oi => oi.Quantity * oi.SalePrice)),

                    ShippingAddress = o.ShippingAddress,
                    ProductName = o.OrderItems.FirstOrDefault() != null
                        ? o.OrderItems.FirstOrDefault().Product.ProductName
                        : "",
                    Image = o.OrderItems.FirstOrDefault() != null
                        ? o.OrderItems.FirstOrDefault().Product.ImageUrl
                        : "/Assets/img/default-product.png",
                    Price = o.OrderItems.FirstOrDefault() != null
                        ? Convert.ToDecimal(o.OrderItems.FirstOrDefault().SalePrice)
                        : 0,
                    Quantity = o.OrderItems.FirstOrDefault() != null
                        ? o.OrderItems.FirstOrDefault().Quantity
                        : 0,
                    Items = o.OrderItems.Select(oi => new OrderItemVM
                    {
                        ProductName = oi.Product.ProductName,
                        Image = oi.Product.ImageUrl,
                        Price = Convert.ToDecimal(oi.SalePrice),
                        Quantity = oi.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return View(orders);
        }

        // Action để filter theo status
        public async Task<IActionResult> FilterOrders(string status)
        {
            var userId = 1; // Thay bằng userId thực tế

            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId);

            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                query = query.Where(o => o.Status.ToLower() == status.ToLower());
            }

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderVM
                {
                    OrderId = o.OrderId,
                    OrderCode = $"ORD{o.OrderId:D6}",
                    Date = o.OrderDate.HasValue
                        ? o.OrderDate.Value.ToString("dd/MM/yyyy")
                        : "",
                    Status = o.Status,
                    Total = (decimal)(o.TotalPrice ?? 0),
                    ShippingAddress = o.ShippingAddress,
                    ProductName = o.OrderItems.FirstOrDefault() != null
                        ? o.OrderItems.FirstOrDefault().Product.ProductName
                        : "",
                    Image = o.OrderItems.FirstOrDefault() != null
                        ? o.OrderItems.FirstOrDefault().Product.ImageUrl
                        : "/Assets/img/default-product.png",
                    Price = o.OrderItems.FirstOrDefault() != null
                        ? (decimal)o.OrderItems.FirstOrDefault().SalePrice
                        : 0,
                    Quantity = o.OrderItems.FirstOrDefault() != null
                        ? o.OrderItems.FirstOrDefault().Quantity
                        : 0,
                    Items = o.OrderItems.Select(oi => new OrderItemVM
                    {
                        ProductName = oi.Product.ProductName,
                        Image = oi.Product.ImageUrl,
                        Price = (decimal)oi.SalePrice,
                        Quantity = oi.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return PartialView("_OrdersList", orders);
        }

        public async Task<IActionResult> OrderDetail(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = int.Parse(userIdString);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderId == id && o.UserId == userId)
                .Select(o => new OrderVM
                {
                    OrderId = o.OrderId,
                    OrderCode = $"ORD{o.OrderId:D6}",
                    Date = o.OrderDate.HasValue
                        ? o.OrderDate.Value.ToString("dd/MM/yyyy HH:mm")
                        : "",
                    Status = o.Status,

                    Total = o.TotalPrice > 0
                        ? (decimal)o.TotalPrice
                        : (decimal)o.OrderItems.Sum(oi => oi.Quantity * oi.SalePrice),

                    ShippingAddress = o.ShippingAddress,
                    Items = o.OrderItems.Select(oi => new OrderItemVM
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.ProductName,
                        Image = oi.Product.ImageUrl,
                        Price = (decimal)oi.SalePrice,
                        Quantity = oi.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // Action để xem chi tiết order
        //public async Task<IActionResult> OrderDetail(int id)
        //{
        //    var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var userId = int.Parse(userIdString);

        //    var order = await _context.Orders
        //        .Include(o => o.OrderItems)
        //            .ThenInclude(oi => oi.Product)
        //        .Where(o => o.OrderId == id && o.UserId == userId)
        //        .Select(o => new OrderVM
        //        {
        //            OrderId = o.OrderId,
        //            OrderCode = $"ORD{o.OrderId:D6}",
        //            Date = o.OrderDate.HasValue
        //                ? o.OrderDate.Value.ToString("dd/MM/yyyy HH:mm")
        //                : "",
        //            Status = o.Status,
        //            Total = (decimal)(o.TotalPrice ?? 0),
        //            ShippingAddress = o.ShippingAddress,
        //            Items = o.OrderItems.Select(oi => new OrderItemVM
        //            {
        //                ProductId = oi.ProductId,
        //                ProductName = oi.Product.ProductName,
        //                Image = oi.Product.ImageUrl,
        //                Price = (decimal)oi.SalePrice,
        //                Quantity = oi.Quantity
        //            }).ToList()
        //        })
        //        .FirstOrDefaultAsync();

        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(order);
        //}
    }
}