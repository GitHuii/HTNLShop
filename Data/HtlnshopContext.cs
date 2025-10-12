using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HTNLShop.Data;

public partial class HtlnshopContext : DbContext
{
    public HtlnshopContext()
    {
    }

    public HtlnshopContext(DbContextOptions<HtlnshopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=HTLNShop;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Carts__51BCD7B7AB710385");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carts__UserId__29572725");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => new { e.CartId, e.ProductId }, "UQ_Cart_Product").IsUnique();

            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Cart).WithMany()
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItems__CartI__32E0915F");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItems__Produ__33D4B598");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0BA9FE0B52");

            entity.Property(e => e.CategoryName).HasMaxLength(200);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCFBA652293");

            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ShippingAddress).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TotalPrice).HasDefaultValue(0.0);

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__UserId__38996AB5");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Order).WithMany()
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Order__3B75D760");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Produ__3C69FB99");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CD32BFF0F6");

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ProductDetail).HasMaxLength(1000);
            entity.Property(e => e.ProductName).HasMaxLength(500);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Catego__2F10007B");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A0DBA8EAB");

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C6C11C022");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(13)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__267ABA7A");
        });
        
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, CategoryName = "Laptop" },
            new Category { CategoryId = 2, CategoryName = "PC Gaming" },
            new Category { CategoryId = 3, CategoryName = "Màn hình" },
            new Category { CategoryId = 4, CategoryName = "Loa" },
            new Category { CategoryId = 5, CategoryName = "Chuột" }
        );

        modelBuilder.Entity<Product>().HasData(
        // Laptop
        new Product { ProductId = 1, ProductName = "Laptop gaming Acer Nitro V ANV15 51 500A", Price = 18390000,ProductDetail= "Laptop gaming mạnh mẽ, CPU Intel Gen 13, card RTX 4050, tản nhiệt kép.", StockQuantity = 10, ImageUrl = "/assets/img/1/1.png", CategoryId = 1 },
        new Product { ProductId = 2, ProductName = "Laptop gaming ASUS Vivobook K3605VC RP431W", Price = 18290000, ProductDetail = "Thiết kế mỏng nhẹ, hiệu năng cao, phù hợp cho học tập và làm việc.", StockQuantity = 8, ImageUrl = "/assets/img/1/2.jpg", CategoryId = 1 },
        new Product { ProductId = 3, ProductName = "Laptop MSI Prestige 14 Evo B13M 401VN", Price = 24490000,ProductDetail = "Laptop cao cấp, đạt chuẩn Intel Evo, tối ưu cho dân văn phòng và sáng tạo nội dung.", StockQuantity = 6, ImageUrl = "/assets/img/1/3.png", CategoryId = 1 },
        new Product { ProductId = 4, ProductName = "Laptop gaming Lenovo Legion 5 Pro 16IAX10 83F3003VVN", Price = 48990000,ProductDetail = "Hiệu năng đỉnh cao với Core i9 và RTX 4080, chuyên game và đồ họa.", StockQuantity = 5, ImageUrl = "/assets/img/1/4.png", CategoryId = 1 },

        // PC Gaming
        new Product { ProductId = 5, ProductName = "PC GVN AMD x ASUS R7-9800x3D/ VGA RTX 5090", Price = 162990000,ProductDetail= "Cấu hình khủng với Ryzen 9 và RTX 5090, chơi mọi tựa game ở 4K.", StockQuantity = 3, ImageUrl = "/assets/img/2/1.jpg", CategoryId = 2 },
        new Product { ProductId = 6, ProductName = "PC GVN Intel i7-14700F/VGA RTX 3050", Price = 28390000,ProductDetail= "Máy tính chơi game tầm trung, hiệu năng ổn định, tiết kiệm điện.", StockQuantity = 10, ImageUrl = "/assets/img/2/2.png", CategoryId = 2 },
        new Product { ProductId = 7, ProductName = "PC GVN Intel i7-14700F/ VGA RTX 3060", Price = 31690000, ProductDetail = "Cấu hình mạnh mẽ, chiến tốt mọi game Esport, hỗ trợ làm đồ họa.", StockQuantity = 7, ImageUrl = "/assets/img/2/3.png", CategoryId = 2 },
        new Product { ProductId = 8, ProductName = "PC GVN x AORUS XTREME ICE (Intel i9-14900K/ VGA RTX 4080 Super)", Price = 135990000, ProductDetail = "Cấu hình siêu cấp dành cho streamer và gamer chuyên nghiệp.", StockQuantity = 12, ImageUrl = "/assets/img/2/4.png", CategoryId = 2 },

        // Màn hình
        new Product { ProductId = 9, ProductName = "Màn hình LG 27G640A-B UltraGear 27\" IPS 2K 300Hz Gsync chuyên game", Price = 7290000,ProductDetail= "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/assets/img/3/1.jpg", CategoryId = 3 },
        new Product { ProductId = 10, ProductName = "Màn hình LG 29U531A-W 29\" IPS 100Hz USBC HDR10 UWFHD", Price = 4990000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/assets/img/3/2.jpg", CategoryId = 3 },
        new Product { ProductId = 11, ProductName = "Màn hình ViewSonic ColorPro VP2456A 24\" IPS 120Hz USBC chuyên đồ hoạ", Price = 5290000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/assets/img/3/3.jpg", CategoryId = 3 },
        new Product { ProductId = 12, ProductName = "Màn hình AORUS FO32U2P 32\" OLED 4K 240Hz chuyên game", Price = 33990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/3/4.jpg", CategoryId = 3 },

        // Loa
        new Product { ProductId = 13, ProductName = "Loa máy tính Edifier MR4", Price = 1790000, ProductDetail = "Âm thanh cân bằng, phù hợp nghe nhạc và làm việc tại nhà.", StockQuantity = 18, ImageUrl = "/assets/img/4/1.jpg", CategoryId = 4 },
        new Product { ProductId = 14, ProductName = "Loa Logitech G560", Price = 1090000, ProductDetail = "Loa gaming RGB đồng bộ ánh sáng theo trò chơi.", StockQuantity = 25, ImageUrl = "/assets/img/4/2.png", CategoryId = 4 },
        new Product { ProductId = 15, ProductName = "Loa Razer Nommo V2", Price = 3990000, ProductDetail = "Thiết kế độc đáo, âm thanh vòm 3D chân thực.", StockQuantity = 7, ImageUrl = "/assets/img/4/3.png", CategoryId = 4 },
        new Product { ProductId = 16, ProductName = "Loa SoundMax SB201 Grey", Price = 490000, ProductDetail = "Loa nhỏ gọn, công suất ổn, giá rẻ phù hợp học sinh sinh viên.", StockQuantity = 30, ImageUrl = "/assets/img/4/4.gif", CategoryId = 4 },

        // Chuột
        new Product { ProductId = 17, ProductName = "Chuột Razer DeathAdder Essential White", Price = 410000, ProductDetail = "Chuột gaming huyền thoại, cảm biến chính xác, thiết kế công thái học.", StockQuantity = 25, ImageUrl = "/assets/img/5/1.png", CategoryId = 5 },
        new Product { ProductId = 18, ProductName = "Chuột Logitech G102 Lightsync White", Price = 415000, ProductDetail = "Chuột gaming phổ biến, đèn RGB, độ bền cao.", StockQuantity = 40, ImageUrl = "/assets/img/5/2.jpg", CategoryId = 5 },
        new Product { ProductId = 19, ProductName = "Chuột không dây Logitech G304 Lightspeed", Price = 750000, ProductDetail = "Chuột không dây hiệu năng cao, pin lâu, trọng lượng nhẹ.", StockQuantity = 35, ImageUrl = "/assets/img/5/3.jpg", CategoryId = 5 },
        new Product { ProductId = 20, ProductName = "Chuột gaming ASUS ROG Strix Impact III", Price = 990000, ProductDetail = "Chuột chuyên game, tốc độ phản hồi nhanh, cảm giác bấm tốt.", StockQuantity = 12, ImageUrl = "/assets/img/5/4.jpg", CategoryId = 5 }
        );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
