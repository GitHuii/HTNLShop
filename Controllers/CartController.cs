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
        
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // ✅ Đã đăng nhập → lưu giỏ hàng vào DB
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int userId))
                    return Unauthorized();

                // Lấy hoặc tạo giỏ hàng cho user
                var cart =  _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefault(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart { UserId = userId, CartItems = new List<CartItem>() };
                    _context.Carts.Add(cart);
                }

                var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == id);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    var product =  _context.Products.FindAsync(id);
                    if (product == null)
                        return NotFound();

                    cart.CartItems.Add(new CartItem
                    {
                        ProductId = id,
                        Quantity = quantity
                    });
                }

                 _context.SaveChangesAsync();
            }
            else
            {
                var cart = Cart;
                var item = cart.SingleOrDefault(p => p.ProductId == id);
                if (item == null)
                {
                    var product = _context.Products.SingleOrDefault(p => p.ProductId == id);
                    if (product == null)
                    {
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
                HttpContext.Session.Set(CART_KEY, cart);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult RemoveCart(int productId)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

                var cartItem = _context.CartItems
                    .Include(ci => ci.Cart)
                    .FirstOrDefault(ci => ci.ProductId == productId && ci.Cart.UserId == userId);

                if (cartItem != null)
                {
                    _context.CartItems.Remove(cartItem);
                    _context.SaveChanges();

                    var total = _context.CartItems
                        .Where(ci => ci.Cart.UserId == userId)
                        .Sum(ci => ci.Quantity * ci.Product.Price);

                    return Json(new { success = true, total });
                }

                return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
            }

            // Xử lý cho Session
            var cart = Cart;
            var item = cart.FirstOrDefault(p => p.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.Set(CART_KEY, cart);
            }

            var totalSession = cart.Sum(i => i.Pay);
            return Json(new { success = true, total = totalSession });
        }

        //public IActionResult RemoveCart(int productId)
        //{
        //    var cart = Cart;
        //    var item = cart.FirstOrDefault(p => p.ProductId == productId);
        //    if (item != null)
        //    {
        //        cart.Remove(item);
        //        HttpContext.Session.Set(CART_KEY, cart);
        //    }
        //    return RedirectToAction("Index");
        //}
        //[HttpPost]
        //public IActionResult IncreaseQuantity(int id)
        //{
        //    var cart = Cart;
        //    var item = cart.FirstOrDefault(p => p.ProductId == id);
        //    if (item != null)
        //    {
        //        item.Quantity++;
        //        HttpContext.Session.Set(CART_KEY, cart);
        //    }
        //    return RedirectToAction("Index");
        //}
        //[HttpPost]
        //public IActionResult DecreaseQuantity(int id)
        //{
        //    var cart = Cart;
        //    var item = cart.FirstOrDefault(p => p.ProductId == id);
        //    if (item != null && item.Quantity > 1)
        //    {
        //        item.Quantity--;
        //        HttpContext.Session.Set(CART_KEY, cart);
        //    }
        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        public IActionResult IncreaseQuantity(int id)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

                var cartItem =  _context.CartItems
                    .Include(ci => ci.Product)
                    .FirstOrDefault(ci => ci.Cart.UserId == userId && ci.ProductId == id);

                if (cartItem != null)
                {
                    cartItem.Quantity++;
                     _context.SaveChanges();
                }

                var sum =  _context.CartItems
                    .Where(ci => ci.Cart.UserId == userId)
                    .Sum(ci => ci.Quantity * ci.Product.Price);

                return Json(new { success = true, quantity = cartItem?.Quantity ?? 0, sum });
            }
            var cart = Cart;
            var item = cart.FirstOrDefault(p => p.ProductId == id);
            if (item != null)
            {
                item.Quantity++;
                HttpContext.Session.Set(CART_KEY, cart);
            }
            var total = cart.Sum(i => i.Pay);
            return Json(new { success = true, quantity = item?.Quantity ?? 0, total });
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
                    .FirstOrDefault(ci => ci.Cart.UserId == userId && ci.ProductId == id);

                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.CartItems.Remove(cartItem);
                        _context.SaveChanges();
                    }
                }

                var sum = _context.CartItems
                    .Where(ci => ci.Cart.UserId == userId)
                    .Sum(ci => ci.Quantity * ci.Product.Price);
                
                return Json(new { success = true, quantity = cartItem?.Quantity ?? 0, sum });
            }
            var cart = Cart;
            var item = cart.FirstOrDefault(p => p.ProductId == id);
            if (item != null && item.Quantity > 1)
            {
                item.Quantity--;
                HttpContext.Session.Set(CART_KEY, cart);
            }
            var total = cart.Sum(i => i.Pay);
            return Json(new { success = true, quantity = item?.Quantity ?? 0, total });
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
        //[Authorize]
        //[HttpGet]
        //public IActionResult CheckOut()
        //{
        //    if (Cart.Count == 0)
        //    {
        //        TempData["Message"] = "Giỏ hàng trống, vui lòng thêm sản phẩm vào giỏ hàng";
        //        return RedirectToAction("Index");
        //    }
        //    return View(Cart);
        //}
        //[Authorize]
        //[HttpGet]
        //public IActionResult CheckOut()
        //{
        //    if (Cart.Count == 0)
        //    {
        //        TempData["Message"] = "Giỏ hàng trống, vui lòng thêm sản phẩm.";
        //        return RedirectToAction("Index");
        //    }

        //    // Lấy thông tin từ Claims
        //    var customer = new
        //    {
        //        Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
        //        FullName = User.FindFirstValue(ClaimTypes.Name),
        //        Address = User.FindFirstValue(ClaimTypes.StreetAddress),
        //        Phone = User.FindFirstValue(ClaimTypes.MobilePhone),
        //        Email = User.FindFirstValue(ClaimTypes.Email)
        //    };

        //    ViewBag.Customer = customer;

        //    return View(Cart);
        //}
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();
            var customer = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            ViewBag.Customer = customer; 

            // ✅ Lấy giỏ hàng từ DB nếu người dùng đã đăng nhập
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Message"] = "Giỏ hàng trống, vui lòng thêm sản phẩm.";
                return RedirectToAction("Index");
            }

            // Lấy thông tin người dùng từ Claims
            //var customer = new
            //{
            //    Id = userId,
            //    FullName = User.FindFirstValue(ClaimTypes.Name),
            //    Address = User.FindFirstValue(ClaimTypes.StreetAddress),
            //    Phone = User.FindFirstValue(ClaimTypes.MobilePhone),
            //    Email = User.FindFirstValue(ClaimTypes.Email)
            //};

            //ViewBag.Customer = customer;

            // ✅ Truyền model chính xác vào View (cart từ DB)
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

        [HttpGet]
        [Authorize]
        public IActionResult BuyNow(int productId,int quantity)
        {
            //var CustormerId = int.Parse(HttpContext.User.Claims.ElementAt(0).ToString());
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int CustormerId)) return Unauthorized();
            // Lấy sản phẩm từ giỏ hàng
            var cart = _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefault(c => c.UserId == CustormerId);
            if (cart == null)
            {
                return RedirectToAction("Index", "SanPham");
            }

            // Tìm sản phẩm trong giỏ hàng
            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (item == null)
            {
                return RedirectToAction("Index", "Cart"); // không có sản phẩm này trong giỏ
            }
            item.Quantity = quantity;

            // Tạo model chỉ gồm 1 sản phẩm để đưa sang trang CheckOut
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

        /*[Authorize]
        [HttpPost]
        public IActionResult CheckOut(CheckoutVM model)
        {
            if (!ModelState.IsValid)
            {
                //var CustormerId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CustomerId");
                var CustormerId = HttpContext.User.Claims.ElementAt(0);
                var phone = User.FindFirstValue(ClaimTypes.MobilePhone);
            }
            var Customer = new User();
            if (model.GiongKhachHang)
            {
                // var CustormerId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CustomerId");
                //var CustormerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var CustormerId = HttpContext.User.Claims.ElementAt(0);
                if (CustormerId != null)
                {
                    int id = int.Parse(CustormerId.Value.ToString());
                    //int id = CustormerId;
                    Customer = _context.Users.SingleOrDefault(u => u.UserId == id);
                    if (Customer != null)
                    {
                        var order = new Order
                        {
                            UserId = Customer.UserId,
                            OrderDate = DateTime.Now,
                            TotalPrice = Cart.Sum(i => i.Price * i.Quantity),
                            ShippingAddress = Customer.Address,
                            Status = "Done"
                        };
                        _context.Database.BeginTransaction();
                        try
                        {

                            _context.Add(order);
                            _context.SaveChanges();
                            foreach (var item in Cart)
                            {
                                var orderDetail = new OrderItem
                                {
                                    OrderId = order.OrderId,
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    SalePrice = 0
                                };
                                _context.Add(orderDetail);
                               
                            }
                            _context.SaveChanges();
                            _context.Database.CommitTransaction();
                            HttpContext.Session.Set<List<CartItems>>(CART_KEY, new List<CartItems>());
                            return RedirectToAction("Index", "SanPham");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("❌ Lỗi trong Checkout: " + ex.Message);
                            _context.Database.RollbackTransaction();

                        }
                    }
                }
            }
            return View(Cart);
        }*/
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckOut(CheckoutVM model, int? BuyNowProductId,int quantity)
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
                    Status = "Done"
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
            // ✅ Load cart từ DB thay vì Session
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
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    TotalPrice = cart.CartItems.Sum(i => i.Quantity * i.Product.Price),
                    ShippingAddress =model.GiongKhachHang? model.Address:customer.Address,
                    Status = "Done"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in cart.CartItems)
                {
                    //_context.OrderItems.Add(new OrderItem
                    //{
                    //    OrderId = order.OrderId,
                    //    ProductId = item.ProductId,
                    //    Quantity = item.Quantity,
                    //    SalePrice = item.Product.Price
                    //});
                    // Kiểm tra tồn kho trước khi trừ
                    if (item.Product.StockQuantity < item.Quantity)
                    {
                        TempData["Message"] = $"Sản phẩm {item.Product.ProductName} không đủ hàng.";
                        await tran.RollbackAsync();
                        return RedirectToAction("Index", "Cart");
                    }

                    // Giảm tồn kho
                    item.Product.StockQuantity -= item.Quantity;

                    // Thêm vào OrderItems
                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        SalePrice = item.Product.Price
                    });
                }

                await _context.SaveChangesAsync();

                // Xóa giỏ hàng sau khi đặt hàng thành công
                _context.CartItems.RemoveRange(
                _context.CartItems.Where(ci => ci.CartId == cart.CartId)
                );
                await _context.SaveChangesAsync();

                await tran.CommitAsync();

                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                Console.WriteLine("❌ Lỗi CheckOut: " + ex.Message);
                return View(model);
            }
        }

    }
}
