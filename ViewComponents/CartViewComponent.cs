using HTNLShop.Helpers;
using HTNLShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HTNLShop.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
          var cart = HttpContext.Session.Get<List<CartItems>>("MYCART") ?? new List<CartItems>();
            return View("CartPanel",
                new CartModel
                {
                    Quantity = cart.Sum(x => x.Quantity),
                    Total = cart.Sum(x => x.Pay)
                }
            );
        }
    }
}
