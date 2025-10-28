using System;
using System.Collections.Generic;

namespace HTNLShop.Data;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public double Price { get; set; }

    public string? ProductDetail { get; set; }

    public int StockQuantity { get; set; }

    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
