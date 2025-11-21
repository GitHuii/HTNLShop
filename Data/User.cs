using System;
using System.Collections.Generic;

namespace HTNLShop.Data;

public partial class User
{
    public int UserId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Cart>? Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order>? Orders { get; set; } = new List<Order>();

    public virtual Role? Role { get; set; } = null!;
}
