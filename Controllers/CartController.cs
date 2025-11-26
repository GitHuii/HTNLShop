using HTNLShop.Data;
using HTNLShop.Helpers;
using HTNLShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Security.Claims;

namespace HTNLShop.Controllers
{
    public class CartController : Controller
    {
        private readonly HtlnshopContext _context;
        const string CART_KEY = "MYCART";
        public CartController(HtlnshopContext context)
        {
            _context = context;
        }
        public List<CartItems> Cart => HttpContext.Session.Get<List<CartItems>>(CART_KEY) ?? new List<CartItems>();
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out int userId))
                {
                    var cart = _context.Carts
                        .Include(c => c.CartItems)
                        .ThenInclude(i => i.Product)
                        .FirstOrDefault(c => c.UserId == userId);

                    var model = cart?.CartItems.Select(ci => new CartItems
                    {
                        ProductId = ci.ProductId,
                        ProductName = ci.Product?.ProductName,
                        Price = ci.Product?.Price ?? 0,
                        Quantity = ci.Quantity,
                        Image = ci.Product?.ImageUrl
                    }).ToList() ?? new List<CartItems>();

                    return View(model);
                }
            }

            // Nếu chưa đăng nhập → lấy từ Session
            var sessionCart = HttpContext.Session.Get<List<CartItems>>("MYCART") ?? new List<CartItems>();
            return View(sessionCart);
        }
        #region Old AddToCart 
        //public IActionResult AddToCart(int id, int quantity = 1)
        //{
        //    if (User.Identity != null && User.Identity.IsAuthenticated)
        //    {

        //        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        if (!int.TryParse(userIdStr, out int userId))
        //            return Unauthorized();


        //        var cart =  _context.Carts
        //            .Include(c => c.CartItems)
        //            .FirstOrDefault(c => c.UserId == userId);

        //        if (cart == null)
        //        {
        //            cart = new Cart { UserId = userId, CartItems = new List<CartItem>() };
        //            _context.Carts.Add(cart);
        //        }

        //        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == id);
        //        if (existingItem != null)
        //        {
        //            existingItem.Quantity += quantity;
        //        }
        //        else
        //        {
        //            var product =  _context.Products.FindAsync(id);
        //            if (product == null)
        //                return NotFound();

        //            cart.CartItems.Add(new CartItem
        //            {
        //                ProductId = id,
        //                Quantity = quantity
        //            });
        //        }

        //         _context.SaveChangesAsync();
        //    }
        //    else
        //    {
        //        var cart = Cart;
        //        var item = cart.SingleOrDefault(p => p.ProductId == id);
        //        if (item == null)
        //        {
        //            var product = _context.Products.SingleOrDefault(p => p.ProductId == id);
        //            if (product == null)
        //            {
        //                TempData["Message"] = $"Không tìm thấy hàng hoá có mã {id}";
        //                return Redirect("/404");
        //            }
        //            item = new CartItems
        //            {
        //                ProductId = product.ProductId,
        //                ProductName = product.ProductName,
        //                Price = product.Price,
        //                Image = product.ImageUrl ?? string.Empty,
        //                Quantity = quantity
        //            };
        //            cart.Add(item);
        //        }
        //        else
        //        {
        //            item.Quantity += quantity;
        //        }
        //        HttpContext.Session.Set(CART_KEY, cart);
        //    }
        //    return RedirectToAction("Index");
        //}
        #endregion
        // Replace/Add the modified AddToCart, IncreaseQuantity and DecreaseQuantity methods
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int userId))
                    return Unauthorized();

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart { UserId = userId, CartItems = new List<CartItem>() };
                    _context.Carts.Add(cart);
                }

                var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == id);
                if (existingItem != null)
                {
                    var maxStock = existingItem.Product?.StockQuantity ?? (_context.Products.Find(id)?.StockQuantity ?? 0);
                    if (existingItem.Quantity + quantity > maxStock)
                    {
                        TempData["Message"] = "Không đủ tồn kho.";
                        return RedirectToAction("Index");
                    }
                    existingItem.Quantity += quantity;
                }
                else
                {
                    var product = await _context.Products.FindAsync(id);
                    if (product == null)
                        return NotFound();

                    if (quantity > product.StockQuantity)
                    {
                        TempData["Message"] = "Không đủ tồn kho.";
                        return RedirectToAction("Index");
                    }

                    cart.CartItems.Add(new CartItem
                    {
                        ProductId = id,
                        Quantity = quantity
                    });
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                var cart = Cart;
                var item = cart.SingleOrDefault(p => p.ProductId == id);
                var product = _context.Products.SingleOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hoá có mã {id}";
                    return Redirect("/404");
                }

                var max = product.StockQuantity;
                if (item == null)
                {
                    if (quantity > max)
                    {
                        TempData["Message"] = "Không đủ tồn kho.";
                        return RedirectToAction("Index");
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
                    if (item.Quantity + quantity > max)
                    {
                        TempData["Message"] = "Không đủ tồn kho.";
                        return RedirectToAction("Index");
                    }
                    item.Quantity += quantity;
                }
                HttpContext.Session.Set(CART_KEY, cart);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult IncreaseQuantity(int id)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

                var cartItem = _context.CartItems
                    .Include(ci => ci.Product)
                    .Include(ci => ci.Cart)
                    .FirstOrDefault(ci => ci.Cart.UserId == userId && ci.ProductId == id);

                if (cartItem == null) return Json(new { success = false, message = "Không tìm thấy sản phẩm.", maxStock = 0 });

                var maxStock = cartItem.Product?.StockQuantity ?? 0;
                if (cartItem.Quantity + 1 > maxStock)
                {
                    // trả về maxStock để client vô hiệu nút '+'
                    return Json(new { success = false, message = "Đã đạt tối đa tồn kho.", quantity = cartItem.Quantity, maxStock });
                }

                cartItem.Quantity++;
                _context.SaveChanges();

                var lineTotal = cartItem.Quantity * cartItem.Product.Price;
                var total = _context.CartItems
                    .Where(ci => ci.Cart.UserId == userId)
                    .Sum(ci => ci.Quantity * ci.Product.Price);

                return Json(new { success = true, quantity = cartItem.Quantity, total, lineTotal, maxStock });
            }

            var cart = Cart;
            var item = cart.FirstOrDefault(p => p.ProductId == id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy sản phẩm.", maxStock = 0 });

            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            var max = product?.StockQuantity ?? 0;
            if (item.Quantity + 1 > max)
                return Json(new { success = false, message = "Đã đạt tối đa tồn kho.", quantity = item.Quantity, maxStock = max });

            item.Quantity++;
            HttpContext.Session.Set(CART_KEY, cart);

            var totalSession = cart.Sum(i => i.Price * i.Quantity);
            var lineTotalSession = item.Quantity * item.Price;
            return Json(new { success = true, quantity = item.Quantity, total = totalSession, lineTotal = lineTotalSession, maxStock = max });
        }

        [HttpPost]
        public IActionResult DecreaseQuantity(int id)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

                var cartItem = _context.CartItems
                    .Include(ci => ci.Product)
                    .Include(ci => ci.Cart)
                    .FirstOrDefault(ci => ci.Cart.UserId == userId && ci.ProductId == id);

                if (cartItem == null) return Json(new { success = false, message = "Không tìm thấy sản phẩm.", maxStock = 0 });

                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    _context.SaveChanges();
                }
                else
                {
                    _context.CartItems.Remove(cartItem);
                    _context.SaveChanges();
                    // nếu xóa thì quantity = 0
                    return Json(new { success = true, quantity = 0, total = _context.CartItems.Where(ci => ci.Cart.UserId == userId).Sum(ci => ci.Quantity * ci.Product.Price), lineTotal = 0, maxStock = 0 });
                }

                var lineTotal = cartItem.Quantity * cartItem.Product.Price;
                var total = _context.CartItems
                    .Where(ci => ci.Cart.UserId == userId)
                    .Sum(ci => ci.Quantity * ci.Product.Price);

                return Json(new { success = true, quantity = cartItem.Quantity, total, lineTotal, maxStock = cartItem.Product.StockQuantity });
            }

            var cart = Cart;
            var item = cart.FirstOrDefault(p => p.ProductId == id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy sản phẩm.", maxStock = 0 });

            if (item.Quantity > 1)
            {
                item.Quantity--;
                HttpContext.Session.Set(CART_KEY, cart);
                var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
                var maxStock = product?.StockQuantity ?? 0;
                var totalSession = cart.Sum(i => i.Price * i.Quantity);
                var lineTotalSession = item.Quantity * item.Price;
                return Json(new { success = true, quantity = item.Quantity, total = totalSession, lineTotal = lineTotalSession, maxStock });
            }
            else
            {
                // remove
                cart.Remove(item);
                HttpContext.Session.Set(CART_KEY, cart);
                var totalSession = cart.Sum(i => i.Price * i.Quantity);
                return Json(new { success = true, quantity = 0, total = totalSession, lineTotal = 0, maxStock = 0 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCart(int productId)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int userId))
                    return Json(new { success = false, message = "Unauthorized" });

                var cartItem = await _context.CartItems
                    .Include(ci => ci.Product)
                    .Include(ci => ci.Cart)
                    .FirstOrDefaultAsync(ci => ci.Cart.UserId == userId && ci.ProductId == productId);

                if (cartItem == null)
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm." });

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                var total = await _context.CartItems
                    .Where(ci => ci.Cart.UserId == userId)
                    .SumAsync(ci => ci.Quantity * ci.Product.Price);

                return Json(new { success = true, total });
            }
            else
            {
                var cart = Cart;
                var item = cart.FirstOrDefault(p => p.ProductId == productId);

                if (item == null)
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm." });

                cart.Remove(item);
                HttpContext.Session.Set(CART_KEY, cart);

                var total = cart.Sum(i => i.Price * i.Quantity);
                return Json(new { success = true, total });
            }
        }
        public async Task MergeCartAsync(int userId, ISession session)
        {
            //const string sessionKey = "SessionCart";

            var sessionData = session.GetString(CART_KEY);
            if (string.IsNullOrEmpty(sessionData)) return;

            var sessionCart = JsonConvert.DeserializeObject<List<CartItems>>(sessionData);

            // Lấy hoặc tạo cart cho user trong DB
            if (sessionCart == null || sessionCart.Count == 0) return;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Lấy cart hiện có (kèm item)
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        CartItems = new List<CartItem>()
                    };
                    _context.Carts.Add(cart);
                }

                // Gộp các sản phẩm
                foreach (var item in sessionCart)
                {
                    var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == item.ProductId);
                    if (existingItem != null)
                        existingItem.Quantity += item.Quantity;
                    else
                        cart.CartItems.Add(new CartItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Xoá giỏ hàng session sau khi merge
                session.Remove(CART_KEY);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("❌ Lỗi khi MergeCartAsync: " + ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();
            var customer = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            ViewBag.Customer = customer;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Message"] = "Giỏ hàng trống, vui lòng thêm sản phẩm.";
                return RedirectToAction("Index");
            }

            var model = cart.CartItems.Select(ci => new CartItems
            {
                ProductId = ci.ProductId,
                ProductName = ci.Product.ProductName,
                Price = ci.Product.Price,
                Quantity = ci.Quantity,
                Image = ci.Product.ImageUrl
            }).ToList();

            return View(model);
        }

        //    [HttpGet]
        //    [Authorize]
        //    public IActionResult BuyNow(int productId, int quantity)
        //    {

        //        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        if (!int.TryParse(userIdStr, out int CustormerId)) return Unauthorized();

        //        var cart = _context.Carts
        //                .Include(c => c.CartItems)
        //                .ThenInclude(ci => ci.Product)
        //                .FirstOrDefault(c => c.UserId == CustormerId);
        //        if (cart == null)
        //        {
        //            return RedirectToAction("Index", "SanPham");
        //        }


        //        var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
        //        if (item == null)
        //        {
        //            return RedirectToAction("Index", "Cart");
        //        }
        //        item.Quantity = quantity;
        //        var model = new List<CartItems>
        //{
        //    new CartItems
        //    {
        //        ProductId = item.ProductId,
        //        ProductName = item.Product.ProductName,
        //        Price = item.Product.Price,
        //        Quantity = item.Quantity,
        //        Image = item.Product.ImageUrl
        //    }
        //};

        //        return View("CheckOut", model);
        //    }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BuyNow(int productId, int quantity)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int customerId))
                return Unauthorized();

            // ✅ THÊM DÒNG NÀY - Lấy thông tin customer
            var customer = await _context.Users.FirstOrDefaultAsync(u => u.UserId == customerId);
            ViewBag.Customer = customer;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == customerId);

            if (cart == null)
            {
                return RedirectToAction("Index", "SanPham");
            }

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (item == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            item.Quantity = quantity;

            var model = new List<CartItems>
    {
        new CartItems
        {
            ProductId = item.ProductId,
            ProductName = item.Product.ProductName,
            Price = item.Product.Price,
            Quantity = item.Quantity,
            Image = item.Product.ImageUrl
        }
    };

            return View("CheckOut", model);
        }
     
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckOut(CheckoutVM model, int? BuyNowProductId, int quantity)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();
            var customer = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (customer == null)
            {
                TempData["Message"] = "Không tìm thấy thông tin khách hàng.";
                return RedirectToAction("Index", "SanPham");
            }
            //var customer = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            ViewBag.Customer = customer; //
            if (BuyNowProductId.HasValue)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == BuyNowProductId);
                if (product == null)
                {
                    TempData["Message"] = "Sản phẩm không tồn tại.";
                    return RedirectToAction("Index", "SanPham");
                }

                if (product.StockQuantity < quantity)
                {
                    TempData["Message"] = "Không đủ hàng để mua.";
                    return RedirectToAction("Index", "SanPham");
                }

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    TotalPrice = product.Price * quantity,
                    ShippingAddress = model.GiongKhachHang ? model.Address : customer.Address,
                    Status = "Đã giao hàng"
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    SalePrice = product.Price
                });

                product.StockQuantity -= quantity;
                await _context.SaveChangesAsync();

                TempData["Message"] = "Đặt hàng thành công (Mua ngay)";
                // Xóa sản phẩm khỏi giỏ hàng (nếu có)
                var cart1 = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart1 != null)
                {
                    var item = cart1.CartItems.FirstOrDefault(ci => ci.ProductId == product.ProductId);
                    if (item != null)
                    {
                        _context.CartItems.Remove(item);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction("Index", "SanPham");
            }
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Message"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "SanPham");
            }

            using var tran = await _context.Database.BeginTransactionAsync();
            try
            {
            
                foreach (var item in cart.CartItems)
                {
                    if (item.Product.StockQuantity < item.Quantity)
                    {
                        TempData["Message"] = $"Sản phẩm {item.Product.ProductName} không đủ hàng.";
                        await tran.RollbackAsync();
                        return RedirectToAction("Index", "Cart");
                    }
                }

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    TotalPrice = cart.CartItems.Sum(i => i.Quantity * i.Product.Price),
                    ShippingAddress = model.GiongKhachHang ? model.Address : customer.Address,
                    Status = "Đã giao hàng"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

      
                foreach (var item in cart.CartItems)
                {
                    item.Product.StockQuantity -= item.Quantity;

                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                       // SalePrice = item.Product.Price
                    });
                }

                await _context.SaveChangesAsync();
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();

                await tran.CommitAsync();

                TempData["Message"] = "Đặt hàng thành công!";
                return RedirectToAction("Index", "SanPham");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                Console.WriteLine("❌ Lỗi CheckOut: " + ex.Message);
                TempData["Message"] = "Có lỗi xảy ra khi đặt hàng.";
                return RedirectToAction("Index", "Cart");
            }
        }

    }
}
