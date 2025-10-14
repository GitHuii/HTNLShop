using HTNLShop.Data;
using HTNLShop.Helpers;
using HTNLShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HTNLShop.Controllers
{
    public class CartController : Controller
    {
        private readonly HtlnshopContext _context;
        const string CART_KEY = "MYCART";
        public CartController(HtlnshopContext context) {
            _context = context;
        }
        public List<CartItems> Cart => HttpContext.Session.Get< List < CartItems >> (CART_KEY) ?? new List<CartItems> ();
        public IActionResult Index()
        {
            return View(Cart);
        }
        public IActionResult AddToCart(int id,int quantity = 1)
        {
            var cart = Cart;
            var item = cart.SingleOrDefault(p => p.CartId == id);
            if (item == null) {
                var product = _context.Products.SingleOrDefault(p => p.ProductId == id);
                if (product == null) {
                    TempData["Message"] = $"Không tìm thấy hàng hoá có mã {id}";
                    return Redirect("/404");
                }
                item = new CartItems
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Image = product.ImageUrl ?? string.Empty,
                    Quantity = quantity
                };
                cart.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }
            HttpContext.Session.Set(CART_KEY,cart);
            return RedirectToAction("Index");
        }
       
        public IActionResult RemoveCart(int productId)
        {
            var cart = Cart;
            var item = cart.FirstOrDefault(p => p.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.Set(CART_KEY, cart);
            }
            return RedirectToAction("Index");
        }
    }
}
