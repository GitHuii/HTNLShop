using System;
using System.Collections.Generic;

namespace HTNLShop.Data;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string Status { get; set; } = null!;

    public double? TotalPrice { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public int UserId { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
}
