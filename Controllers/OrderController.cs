using Microsoft.AspNetCore.Mvc;

namespace HTNLShop.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            var orders = new[]
         {
            new {
                OrderId = 1,
                OrderCode = "ODR10001",
                Date = "2025-01-01",
                Status = "Delivered",
                Image = "https://via.placeholder.com/100",
                ProductName = "Fresh Mango",
                Price = 5.99,
                Total = 11.98
            },
            new {
                OrderId = 2,
                OrderCode = "ODR10002",
                Date = "2025-01-05",
                Status = "Pending",
                Image = "https://via.placeholder.com/100",
                ProductName = "Organic Banana",
                Price = 2.99,
                Total = 5.98
            }
        };

            return View(orders);
        }
    }
}
