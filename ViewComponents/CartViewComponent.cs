//using HTNLShop.Helpers;
//using HTNLShop.ViewModels;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;

//namespace HTNLShop.ViewComponents
//{
//    public class CartViewComponent : ViewComponent
//    {
//        public IViewComponentResult Invoke()
//        {
//          var cart = HttpContext.Session.Get<List<CartItems>>("MYCART") ?? new List<CartItems>();
//            return View("CartPanel",
//                new CartModel
//                {
//                    Quantity = cart.Sum(x => x.Quantity),
//                    Total = cart.Sum(x => x.Pay)
//                }
//            );
//        }
//    }
//}
using HTNLShop.Data;
using HTNLShop.Helpers;
using HTNLShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HTNLShop.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly HtlnshopContext _context;
        const string CART_KEY = "MYCART";

        public CartViewComponent(HtlnshopContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            int quantity = 0;
            decimal total = 0;

            // ✅ Kiểm tra nếu đã đăng nhập
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out int userId))
                {
           
                    var cartItems = _context.CartItems
                        .Where(ci => ci.Cart.UserId == userId)
                        .Select(ci => new
                        {
                            ci.Quantity,
                            Total = ci.Quantity * ci.Product.Price
                        })
                        .ToList();

               
                    quantity = _context.CartItems
                        .Where(ci => ci.Cart.UserId == userId)
                        .Sum(ci => ci.Quantity);  
                    total = (decimal)cartItems.Sum(x => x.Total);
                }
            }
            else
            {
   
                var cart = HttpContext.Session.Get<List<CartItems>>(CART_KEY) ?? new List<CartItems>();
                quantity = cart.Sum(x => x.Quantity);
                total = (decimal)cart.Sum(x => x.Pay);
            }

            return View("CartPanel", new CartModel
            {
                Quantity = quantity,
                Total = (double)total
            });
        }
    }
}