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
            new Category { CategoryId = 5, CategoryName = "Chuột" },
            new Category { CategoryId = 6, CategoryName = "Tai nghe"},
            new Category { CategoryId = 7, CategoryName = "Bàn phím" }, 
            new Category { CategoryId = 8, CategoryName = "Nguồn" },
            new Category { CategoryId = 9, CategoryName = "Main" }
        );

        modelBuilder.Entity<Product>().HasData(
        // Laptop
        //Acer
        new Product { ProductId = 1, ProductName = "Laptop Acer Nitro V ANV15 51 500A", Price = 18390000,ProductDetail= "Laptop gaming mạnh mẽ, CPU Intel Gen 13, card RTX 4050, tản nhiệt kép.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/1/1.jpg", CategoryId = 1 },
        new Product { ProductId = 2, ProductName = "Laptop Acer Swift 3 SF314 511 55QE", Price = 11990000, ProductDetail = "Thiết kế mỏng nhẹ, vỏ nhôm cao cấp, hiệu năng mạnh mẽ với Intel Core i5 Gen 11, pin bền, phù hợp cho sinh viên và dân văn phòng.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/1/2.jpg", CategoryId = 1 },
        new Product { ProductId = 3, ProductName = "Laptop Acer Aspire Lite 14 AL14 71P 55P9", Price = 14990000, ProductDetail = "Laptop giá tốt, hiệu năng ổn định, màn hình 14 inch Full HD, thiết kế thanh lịch – lựa chọn lý tưởng cho nhu cầu học tập và làm việc cơ bản.", StockQuantity = 6, ImageUrl = "/Assets/img/Products/1/3.jpg", CategoryId = 1 },
        new Product { ProductId = 4, ProductName = "Laptop Acer Swift X14 SFX14 72G 77F9", Price = 33990000, ProductDetail = "Hiệu năng cao với chip Intel Gen 12 và GPU RTX, đáp ứng tốt nhu cầu đồ họa, lập trình và sáng tạo nội dung trong thiết kế gọn nhẹ.", StockQuantity = 5, ImageUrl = "/Assets/img/Products/1/4.jpg", CategoryId = 1 },
        new Product { ProductId = 5, ProductName = "Laptop ACER Swift Lite 14 SFL14 51M 56HS", Price = 18390000, ProductDetail = "Mỏng nhẹ chỉ khoảng 1,2kg, pin lâu, cấu hình mạnh mẽ, phù hợp người dùng thường xuyên di chuyển và làm việc ngoài văn phòng.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/1/5.jpg", CategoryId = 1 },
        new Product { ProductId = 6, ProductName = "Laptop Acer Swift Go SFG14 74 58FJ", Price = 28990000, ProductDetail = "Trang bị CPU Intel Gen 14 mới nhất, màn OLED sắc nét, hiệu năng mạnh mẽ và thiết kế sang trọng – tối ưu cho công việc lẫn giải trí.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/1/6.jpg", CategoryId = 1 },
        //Asus
        new Product { ProductId = 7, ProductName = "Laptop ASUS Vivobook K3605VC RP431W", Price = 18290000, ProductDetail = "Thiết kế mỏng nhẹ, hiệu năng cao, phù hợp cho học tập và làm việc.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/1/7.jpg", CategoryId = 1 },
        new Product { ProductId = 8, ProductName = "Laptop ASUS Vivobook S14 S3407CA LY095WS", Price = 20490000, ProductDetail = "Thiết kế mỏng nhẹ, màn hình OLED 14 inch, CPU Intel Core Ultra 5, pin bền và hiệu năng ổn định cho học tập và làm việc.", StockQuantity = 6, ImageUrl = "/Assets/img/Products/1/8.jpg", CategoryId = 1 },
        new Product { ProductId = 9, ProductName = "Laptop ASUS Vivobook S14 S3407CA LY096WS", Price = 22990000, ProductDetail = "Trang bị chip Intel Core Ultra 7, màn hình OLED sắc nét, thiết kế hiện đại, hiệu năng mạnh cho người dùng văn phòng và sáng tạo nội dung.", StockQuantity = 9, ImageUrl = "/Assets/img/Products/1/9.jpg", CategoryId = 1 },
        new Product { ProductId = 10, ProductName = "Laptop ASUS Zenbook 14 UX3405CA PZ204WS", Price = 34990000, ProductDetail = "Ultrabook cao cấp với màn OLED 2.8K, chip Intel Core Ultra 7, siêu mỏng nhẹ, pin lâu – lựa chọn hoàn hảo cho doanh nhân di chuyển nhiều.", StockQuantity = 7, ImageUrl = "/Assets/img/Products/1/10.jpg", CategoryId = 1 },
        new Product { ProductId = 11, ProductName = "Laptop ASUS ExpertBook B1 BM1403CDA S60974W", Price = 12190000, ProductDetail = "Laptop doanh nhân bền bỉ, CPU AMD Ryzen 5, RAM 16GB, hiệu năng ổn định, phù hợp làm việc văn phòng và học tập chuyên nghiệp.", StockQuantity = 5, ImageUrl = "/Assets/img/Products/1/11.jpg", CategoryId = 1 },
        new Product { ProductId = 12, ProductName = "Laptop ASUS Expertbook P1403CVA-i716-50W", Price = 17990000, ProductDetail = "Hiệu năng mạnh mẽ với Intel Core i7 Gen 13, thiết kế chắc chắn, bảo mật cao – tối ưu cho môi trường doanh nghiệp.", StockQuantity = 4, ImageUrl = "/Assets/img/Products/1/12.jpg", CategoryId = 1 },
        //Dell
        new Product { ProductId = 13, ProductName = "Laptop Dell 15 DC15250 i7U161W11SLU", Price = 20990000, ProductDetail = "Hiệu năng mạnh với Intel Core i7 Gen 13, RAM 16GB, SSD 512GB, thiết kế hiện đại – phù hợp làm việc và giải trí.", StockQuantity = 12, ImageUrl = "/Assets/img/Products/1/13.jpg", CategoryId = 1 },
        new Product { ProductId = 14, ProductName = "Laptop Dell Inspiron 5440-PUS i5-1334U/512GB/16GB DDR5/14/FHD/W11/Carbon Black", Price = 16990000, ProductDetail = "Laptop mỏng nhẹ, CPU Intel Core i5 Gen 13, RAM 16GB, SSD 512GB, màn 14 FHD – lý tưởng cho sinh viên và dân văn phòng.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/1/14.jpg", CategoryId = 1 },
        new Product { ProductId = 15, ProductName = "Laptop Dell Inspirion N3530 i5U165W11SLU (1334U)", Price = 15990000, ProductDetail = "Thiết kế tinh tế, chip i5 Gen 13, hiệu năng ổn định, pin tốt – đáp ứng nhu cầu học tập và làm việc hằng ngày.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/1/15.jpg", CategoryId = 1 },
        new Product { ProductId = 16, ProductName = "Laptop Dell XPS 9350 XPS9350-U5IA165W11GR-FP", Price = 54990000, ProductDetail = "Ultrabook cao cấp, vỏ nhôm nguyên khối, màn hình sắc nét, chip i7, RAM 16GB – dành cho người dùng chuyên nghiệp và sáng tạo.", StockQuantity = 7, ImageUrl = "/Assets/img/Products/1/16.jpg", CategoryId = 1 },
        //HP
        new Product { ProductId = 17, ProductName = "Laptop gaming HP Victus 16-r0369TX AY8Y2PA", Price = 29490000, ProductDetail = "Laptop gaming hiệu năng cao, CPU Intel Core i7 Gen 13, GPU RTX 4050, màn 16.1 – chơi game và làm đồ họa mượt mà.", StockQuantity = 13, ImageUrl = "/Assets/img/Products/1/17.jpg", CategoryId = 1 },
        new Product { ProductId = 18, ProductName = "Laptop gaming HP Victus 16-s1149AX AZ0D4PA", Price = 19990000, ProductDetail = "Trang bị Ryzen 7 7840HS và RTX 4060, hiệu năng mạnh, tản nhiệt tốt – phù hợp game thủ và nhà sáng tạo nội dung.", StockQuantity = 11, ImageUrl = "/Assets/img/Products/1/18.jpg", CategoryId = 1 },
        new Product { ProductId = 19, ProductName = "Laptop gaming HP VICTUS 15 fb3116AX BX8U4PA", Price = 20990000, ProductDetail = "Thiết kế trẻ trung, chip Ryzen 5 7535HS, GPU RTX 2050, hiệu năng ổn định, chơi game và học tập tốt.", StockQuantity = 9, ImageUrl = "/Assets/img/Products/1/19.jpg", CategoryId = 1 },
        new Product { ProductId = 20, ProductName = "Laptop gaming HP OMEN 16-am0180TX BX8Y6PA", Price = 31990000, ProductDetail = "Dòng cao cấp, CPU i7 Gen 13, GPU RTX 4070, màn QHD 165Hz, bàn phím RGB – trải nghiệm gaming đỉnh cao.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/1/20.jpg", CategoryId = 1 },
        //Lenovo
        new Product { ProductId = 21, ProductName = "Laptop gaming Lenovo Legion 5 15IRX10 83LY00A7VN", Price = 37490000, ProductDetail = "Hiệu năng mạnh với Intel Core i7 Gen 13, GPU RTX 4060, tản nhiệt hiệu quả – chiến mượt mọi tựa game nặng.", StockQuantity = 14, ImageUrl = "/Assets/img/Products/1/21.jpg", CategoryId = 1 },
        new Product { ProductId = 22, ProductName = "Laptop gaming Lenovo LOQ 15ARP9 83JC00M3VN", Price = 22290000, ProductDetail = "Trang bị Ryzen 7 7840HS, RTX 4050, thiết kế gaming hiện đại, bàn phím RGB – hiệu suất cao cho game và đồ họa.", StockQuantity = 12, ImageUrl = "/Assets/img/Products/1/22.jpg", CategoryId = 1 },
        new Product { ProductId = 23, ProductName = "Laptop gaming Lenovo Legion 5 15AHP10 83M0002YVN", Price = 36990000, ProductDetail = "Sức mạnh từ Ryzen 7 8845HS và RTX 4070, màn 165Hz mượt mà – tối ưu cho game thủ chuyên nghiệp.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/1/23.jpg", CategoryId = 1 },
        new Product { ProductId = 24, ProductName = "Laptop gaming Lenovo LOQ 15AHP10 83JG0047VN", Price = 30790000, ProductDetail = "Laptop gaming tầm trung, CPU Ryzen 5 8645HS, GPU RTX 3050, hiệu năng ổn định – cân tốt các game phổ biến.", StockQuantity = 9, ImageUrl = "/Assets/img/Products/1/24.jpg", CategoryId = 1 },
        //MSI
        new Product { ProductId = 25, ProductName = "Laptop MSI Prestige 14 Evo B13M 401VN", Price = 22490000, ProductDetail = "Ultrabook mỏng nhẹ, CPU Intel Core i7 Gen 13, RAM 16GB, pin lâu, tối ưu cho sáng tạo nội dung và văn phòng.", StockQuantity = 7, ImageUrl = "/Assets/img/Products/1/25.jpg", CategoryId = 1 },
        new Product { ProductId = 26, ProductName = "Laptop MSI Prestige 14 AI+ Evo C2VMG 020VN", Price = 18290000, ProductDetail = "Laptop mỏng nhẹ, chip Intel Core i5, RAM 8GB, SSD 512GB – hiệu năng ổn định cho học tập và làm việc cơ bản.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/1/26.jpg", CategoryId = 1 },
        new Product { ProductId = 27, ProductName = "Laptop MSI Modern 14 F13MG 466VN", Price = 25490000, ProductDetail = "Ultrabook cao cấp, CPU Intel Core i7 Gen 13, GPU RTX 4060, màn 16 sắc nét – dành cho thiết kế đồ họa và multimedia.", StockQuantity = 7, ImageUrl = "/Assets/img/Products/1/27.jpg", CategoryId = 1 },
        new Product { ProductId = 28, ProductName = "Laptop MSI Prestige 16 AI+ Mercedes AMG B2VMG 088VN", Price = 38990000, ProductDetail = "Hiệu năng đỉnh cao với Core i9 và RTX 4080, chuyên game và đồ họa.", StockQuantity = 3, ImageUrl = "/Assets/img/Products/1/28.jpg", CategoryId = 1 },
        new Product { ProductId = 29, ProductName = "Laptop MSI Modern 15 H C13M 216VN", Price = 17390000, ProductDetail = "Laptop gaming mạnh mẽ, CPU Intel Gen 13, card RTX 4050, tản nhiệt kép.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/1/29.jpg", CategoryId = 1 },

        // PC Gaming
        //i3
        new Product { ProductId = 30, ProductName = "PC GVN Intel i3-12100F/ VGA RX 6500XT (Powered by ASUS)", Price = 10490000,ProductDetail= "Cấu hình khủng với i3-12100F VGA RX 6500XT, chơi mọi tựa game ở 4K.", StockQuantity = 3, ImageUrl = "/Assets/img/Products/2/30.jpg", CategoryId = 2 },
        new Product { ProductId = 31, ProductName = "PC GVN Intel i3-12700F/ VGA RX 6500XT (Powered by ASUS)", Price = 12390000,ProductDetail= "Máy tính chơi game tầm trung, hiệu năng ổn định, tiết kiệm điện.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/2/31.jpg", CategoryId = 2 },
        new Product { ProductId = 32, ProductName = "PC GVN Homework i3 12100", Price = 31690000, ProductDetail = "Cấu hình mạnh mẽ, chiến tốt mọi game Esport, hỗ trợ làm đồ họa.", StockQuantity = 7, ImageUrl = "/Assets/img/Products/2/32.jpg", CategoryId = 2 },
        new Product { ProductId = 33, ProductName = "PC GVN Homework I3 14100", Price = 135990000, ProductDetail = "Cấu hình siêu cấp dành cho streamer và gamer chuyên nghiệp.", StockQuantity = 12, ImageUrl = "/Assets/img/Products/2/33.jpg", CategoryId = 2 },
        new Product { ProductId = 34, ProductName = "PC GVN Homework i3 - GT", Price = 162990000, ProductDetail = "Cấu hình khủng với i3 - GT, chơi mọi tựa game ở 4K.", StockQuantity = 3, ImageUrl = "/Assets/img/Products/2/34.jpg", CategoryId = 2 },
        //i5
        new Product { ProductId = 35, ProductName = "PC GVN INTEL I5-12400F/VGA RTX 5050", Price = 17490000, ProductDetail = "Máy tính chơi game tầm trung, hiệu năng ổn định, tiết kiệm điện.", StockQuantity = 9, ImageUrl = "/Assets/img/Products/2/35.jpg", CategoryId = 2 },
        new Product { ProductId = 36, ProductName = "PC GVN x Corsair iCUE (Intel i5-14400F/ VGA RTX 5060)", Price = 28990000, ProductDetail = "Cấu hình mạnh mẽ, chiến tốt mọi game Esport, hỗ trợ làm đồ họa.", StockQuantity = 6, ImageUrl = "/Assets/img/Products/2/36.jpg", CategoryId = 2 },
        new Product { ProductId = 37, ProductName = "PC GVN Intel i5-12400F/ VGA RTX 5060 (Main H)", Price = 18490000, ProductDetail = "Cấu hình siêu cấp dành cho streamer và gamer chuyên nghiệp.", StockQuantity = 5, ImageUrl = "/Assets/img/Products/2/37.jpg", CategoryId = 2 },
        new Product { ProductId = 38, ProductName = "PC GVN Intel i5-12400F/ VGA RTX 4060", Price = 17990000, ProductDetail = "Cấu hình khủng với Ryzen 9 và RTX 4060, chơi mọi tựa game ở 4K.", StockQuantity = 2, ImageUrl = "/Assets/img/Products/2/38.jpg", CategoryId = 2 },
        new Product { ProductId = 39, ProductName = "PC GVN x MSI PROJECT ZERO WHITE (Intel i5-14400F/ VGA RTX 5060)", Price = 30990000, ProductDetail = "Máy tính chơi game tầm trung, hiệu năng ổn định, tiết kiệm điện.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/2/39.jpg", CategoryId = 2 },
        new Product { ProductId = 40, ProductName = "PC GVN Intel i5-14400F/ VGA RTX 5060 Ti", Price = 30790000, ProductDetail = "Máy tính chơi game tầm trung, hiệu năng ổn định, tiết kiệm điện.", StockQuantity = 4, ImageUrl = "/Assets/img/Products/2/40.jpg", CategoryId = 2 },
        //i7
        new Product { ProductId = 41, ProductName = "PC GVN Intel i7-14700F/ VGA RTX 3050", Price = 31690000, ProductDetail = "Cấu hình mạnh mẽ, chiến tốt mọi game Esport, hỗ trợ làm đồ họa.", StockQuantity = 7, ImageUrl = "/Assets/img/Products/2/41.jpg", CategoryId = 2 },
        new Product { ProductId = 42, ProductName = "PC GVN Intel i7-14700F/ VGA RTX 3060", Price = 235990000, ProductDetail = "Cấu hình siêu cấp dành cho streamer và gamer chuyên nghiệp.", StockQuantity = 12, ImageUrl = "/Assets/img/Products/2/42.jpg", CategoryId = 2 },
        new Product { ProductId = 43, ProductName = "PC GVN Intel i7-14700F/ VGA RTX 4060", Price = 462990000, ProductDetail = "Cấu hình khủng với i7-14700F và VGA RTX 4060, chơi mọi tựa game ở 4K.", StockQuantity = 3, ImageUrl = "/Assets/img/Products/2/43.jpg", CategoryId = 2 },
        new Product { ProductId = 44, ProductName = "PC GVN Intel i7-14700F/ VGA RTX 5080", Price = 58390000, ProductDetail = "Máy tính chơi game tầm trung, hiệu năng ổn định, tiết kiệm điện.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/2/44.jpg", CategoryId = 2 },
        new Product { ProductId = 45, ProductName = "PC GVN Intel i7-14700F/ VGA RTX 5060", Price = 51690000, ProductDetail = "Cấu hình mạnh mẽ, chiến tốt mọi game Esport, hỗ trợ làm đồ họa.", StockQuantity = 7, ImageUrl = "/Assets/img/Products/2/45.jpg", CategoryId = 2 },
        //i9
        new Product { ProductId = 46, ProductName = "PC GVN x AORUS XTREME ICE (Intel i9-14900K/ VGA RTX 4080 Super)", Price = 135000000, ProductDetail = "Cấu hình siêu cấp dành cho streamer và gamer chuyên nghiệp.", StockQuantity = 2, ImageUrl = "/Assets/img/Products/2/46.jpg", CategoryId = 2 },
        new Product { ProductId = 47, ProductName = "PC GVN Intel i9-14900K/ VGA RTX 4060 Ti", Price = 52990000, ProductDetail = "Cấu hình siêu cấp dành cho streamer và gamer chuyên nghiệp ko chuyên.", StockQuantity = 3, ImageUrl = "/Assets/img/Products/2/47.jpg", CategoryId = 2 },
        new Product { ProductId = 48, ProductName = "PC GVN Intel Core Ultra 7 265KF/ VGA RTX 5060", Price = 38990000, ProductDetail = "Cấu hình khủng với core Ultra 7 và VGA RTX 5060, chơi mọi tựa game ở 4K.", StockQuantity = 1, ImageUrl = "/Assets/img/Products/2/48.jpg", CategoryId = 2 },
        new Product { ProductId = 49, ProductName = "PC GVN Intel Core Ultra 7 265KF/ VGA RTX 5080 (Powered by MSI)", Price = 75990000, ProductDetail = "Máy tính chơi game tầm cao, hiệu năng đỉnh, tiết kiệm điện.", StockQuantity = 4, ImageUrl = "/Assets/img/Products/2/49.jpg", CategoryId = 2 },
        new Product { ProductId = 50, ProductName = "PC GVN Intel Core Ultra 7 265KF/ VGA RTX 5080", Price = 71190000, ProductDetail = "Cấu hình mạnh mẽ, chiến tốt mọi game Esport, hỗ trợ làm đồ họa.", StockQuantity = 6, ImageUrl = "/Assets/img/Products/2/50.jpg", CategoryId = 2 },
        new Product { ProductId = 51, ProductName = "PC GVN Intel Core Ultra 7 265KF/ VGA RTX 5070", Price = 50490000, ProductDetail = "Cấu hình mạnh mẽ, chiến tốt mọi game Esport, hỗ trợ làm đồ họa.", StockQuantity = 5, ImageUrl = "/Assets/img/Products/2/51.jpg", CategoryId = 2 },
        new Product { ProductId = 52, ProductName = "PC GVN Intel Core Ultra 7 265KF/ VGA RTX 5070 Ti (Powered by MSI)", Price = 58990000, ProductDetail = "Cấu hình siêu cấp dành cho streamer và gamer chuyên nghiệp.", StockQuantity = 2, ImageUrl = "/Assets/img/Products/2/52.jpg", CategoryId = 2 },
        new Product { ProductId = 53, ProductName = "PC GVN INT x MSI Dragon ACE (Intel Core Ultra 9 285K/ VGA RTX 5090) (Powered by MSI)", Price = 162990000, ProductDetail = "Cấu hình khủng với Ultra core i9 và VGA RTX 5090, chơi mọi tựa game ở 4K.", StockQuantity = 3, ImageUrl = "/Assets/img/Products/2/53.jpg", CategoryId = 2 },
        new Product { ProductId = 54, ProductName = "PC GVN AMD R9-7900/ VGA RTX 3060", Price = 39990000, ProductDetail = "Máy tính chơi game tầm trung, hiệu năng ổn định, tiết kiệm điện.", StockQuantity = 4, ImageUrl = "/Assets/img/Products/2/54.jpg", CategoryId = 2 },
        new Product { ProductId = 55, ProductName = "PC GVN AMD R9-9950X/ VGA RTX 4090", Price = 138000000, ProductDetail = "Cấu hình mạnh mẽ, chiến tốt mọi game Esport, hỗ trợ làm đồ họa.", StockQuantity = 6, ImageUrl = "/Assets/img/Products/2/55.jpg", CategoryId = 2 },

        // Màn hình
        //Acer
        new Product { ProductId = 56, ProductName = "Màn hình Acer SA272U-E White 27\" IPS 2K 100Hz", Price = 3890000,ProductDetail= "Màn hình 27 inch, độ phân giải 2K, tấm nền IPS cho màu sắc trung thực, tần số quét 100Hz, thiết kế trắng hiện đại, phù hợp làm việc và giải trí.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/3/56.jpg", CategoryId = 3 },
        new Product { ProductId = 57, ProductName = "Màn hình Acer KA272 G0 27\" IPS 120Hz", Price = 2550000, ProductDetail = "Màn hình 27 inch IPS, tần số quét 120Hz mượt mà, hiển thị sắc nét, thích hợp chơi game và làm việc văn phòng.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/3/57.jpg", CategoryId = 3 },
        new Product { ProductId = 58, ProductName = "Màn hình Acer EK251Q G 25\" IPS 120Hz", Price = 2290000, ProductDetail = "Màn hình 25 inch, IPS 120Hz, thời gian phản hồi nhanh, lý tưởng cho game và giải trí hàng ngày.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/3/58.jpg", CategoryId = 3 },
        new Product { ProductId = 59, ProductName = "Màn hình Acer VG271U M3 27\" IPS 2K 180Hz chuyên game", Price = 41990000, ProductDetail = "Màn hình 27 inch, độ phân giải 2K, IPS, tần số quét siêu cao 180Hz, tối ưu cho game thủ, hình ảnh mượt mà và sắc nét.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/59.jpg", CategoryId = 3 },
        new Product { ProductId = 60, ProductName = "Màn hình ACER EK221Q E3 22\" IPS 100Hz", Price = 17900000, ProductDetail = "Màn hình 22 inch, IPS, tần số quét 100Hz, hiển thị màu sắc chính xác, gọn nhẹ, phù hợp học tập và làm việc văn phòng.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/60.jpg", CategoryId = 3 },
        //AOC
        new Product { ProductId = 61, ProductName = "Màn hình cong AOC C27G4Z 27\" 280Hz Adaptive Sync chuyên game", Price = 7290000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/3/61.jpg", CategoryId = 3 },
        new Product { ProductId = 62, ProductName = "Màn hình AOC Q27G40E 27\" Fast IPS 2K 180Hz chuyên game", Price = 4990000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/3/62.jpg", CategoryId = 3 },
        new Product { ProductId = 63, ProductName = "Màn hình AOC 25G4K 25\" Fast IPS 420Hz chuyên game", Price = 5290000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/3/63.jpg", CategoryId = 3 },
        new Product { ProductId = 64, ProductName = "Màn hình AOC 24G11ZE 24\" Fast IPS 240Hz chuyên game", Price = 33990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/64.jpg", CategoryId = 3 },
        new Product { ProductId = 65, ProductName = "Màn hình AOC Agon Pro AG276QSD 27\" OLED 2K 360Hz chuyên game", Price = 33990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/65.jpg", CategoryId = 3 },
        //Asus
        new Product { ProductId = 66, ProductName = "Màn hình di động Asus ZenScreen MB169CK 16\" IPS FHD USBC", Price = 2990000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/3/66.jpg", CategoryId = 3 },
        new Product { ProductId = 67, ProductName = "Màn hình ASUS ProArt PA278QV 27\" IPS 2K 75Hz chuyên đồ họa", Price = 7690000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/3/67.jpg", CategoryId = 3 },
        new Product { ProductId = 68, ProductName = "Màn hình ASUS ProArt PA27JCV 27\" IPS 5K USBC chuyên đồ họa", Price = 20500000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/3/68.jpg", CategoryId = 3 },
        new Product { ProductId = 69, ProductName = "Màn hình ASUS VZ249HG 24\" IPS 120Hz viền mỏng", Price = 2350000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/69.jpg", CategoryId = 3 },
        new Product { ProductId = 70, ProductName = "Màn hình cong Asus ROG Swift PG39WCDM 39\" WOLED 2K 240Hz USBC chuyên game", Price = 40990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/70.jpg", CategoryId = 3 },
        //Dell
        new Product { ProductId = 71, ProductName = "Màn hình Dell Pro Plus P2725D 27\" IPS 2K 100Hz", Price = 6890000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/3/71.jpg", CategoryId = 3 },
        new Product { ProductId = 72, ProductName = "Màn hình Dell UltraSharp U2725QE 27\" IPS 4K 120Hz USBC chuyên đồ họa", Price = 16490000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/3/72.jpg", CategoryId = 3 },
        new Product { ProductId = 73, ProductName = "Màn hình Dell UltraSharp U2424HE 24\" IPS 120Hz USBC", Price = 6490000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/3/73.jpg", CategoryId = 3 },
        new Product { ProductId = 74, ProductName = "Màn hình cong Dell Alienware AW3423DW 34\" QD-OLED 2K 175Hz G-Sync Ultimate", Price = 23790000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/74.jpg", CategoryId = 3 },
        new Product { ProductId = 75, ProductName = "Màn hình Dell UltraSharp U4323QE 43\" IPS 4K chuyên đồ họa", Price = 23990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/75.jpg", CategoryId = 3 },
        //Gigabyte
        new Product { ProductId = 76, ProductName = "Màn hình GIGABYTE G25F2 25\" IPS 200Hz chuyên game", Price = 2990000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/3/76.jpg", CategoryId = 3 },
        new Product { ProductId = 77, ProductName = "Màn hình GIGABYTE MO27Q28G 27\" WOLED 2K 280Hz chuyên game", Price = 16490000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/3/77.jpg", CategoryId = 3 },
        new Product { ProductId = 78, ProductName = "Màn hình GIGABYTE FO32U2P 32\" OLED 4K 240Hz chuyên game", Price = 33990000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/3/78.jpg", CategoryId = 3 },
        new Product { ProductId = 79, ProductName = "Màn hình GIGABYTE FO27Q5P 27\" OLED 2K 500Hz chuyên game", Price = 33990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/79.jpg", CategoryId = 3 },
        new Product { ProductId = 80, ProductName = "Màn hình cong GIGABYTE G34WQC2 34\" 2K 200Hz chuyên game", Price = 7590000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/80.jpg", CategoryId = 3 },
        //HKC
        new Product { ProductId = 81, ProductName = "Màn hình cong HKC MG34H18Q 34\" 2K 165Hz USBC", Price = 6690000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/3/81.jpg", CategoryId = 3 },
        new Product { ProductId = 82, ProductName = "Màn hình HKC MB24V9-U 24\" IPS 100Hz", Price = 1790000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/3/82.jpg", CategoryId = 3 },
        new Product { ProductId = 83, ProductName = "Màn hình HKC MG27H7F 27\" Fast IPS 165Hz Gsync", Price = 3090000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/3/83.jpg", CategoryId = 3 },
        new Product { ProductId = 84, ProductName = "Màn hình HKC MG27S9QS 27\" Fast IPS 2K 155Hz Gsync", Price = 4890000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/84.jpg", CategoryId = 3 },
        new Product { ProductId = 85, ProductName = "Màn hình HKC M27A9X-W Black 75Hz", Price = 2190000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/85.jpg", CategoryId = 3 },
        //LG
        new Product { ProductId = 86, ProductName = "Màn hình LG 27US500-W Ultrafine 27\" IPS 4K HDR10", Price = 5390000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/3/86.jpg", CategoryId = 3 },
        new Product { ProductId = 87, ProductName = "Màn hình LG 22U401A-B 22\" 100Hz HDR10", Price = 2090000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/3/87.jpg", CategoryId = 3 },
        new Product { ProductId = 88, ProductName = "Màn hình LG 27GR75Q-B UltraGear 27\" IPS 2K 165Hz Gsync chuyên game", Price = 6090000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/3/88.jpg", CategoryId = 3 },
        new Product { ProductId = 89, ProductName = "Màn hình LG 27U411A-B 27\" IPS 120Hz HDR10 siêu mỏng", Price = 2790000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/89.jpg", CategoryId = 3 },
        new Product { ProductId = 90, ProductName = "Màn hình LG 27U631A-B 27\" IPS 2K 100Hz HDR10", Price = 4690000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/90.jpg", CategoryId = 3 },
        //MSI
        new Product { ProductId = 91, ProductName = "Màn hình cong MSI MAG 275CF X24 27\" 240Hz chuyên game", Price = 3490000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/3/91.jpg", CategoryId = 3 },
        new Product { ProductId = 92, ProductName = "Màn hình MSI MAG 272F X24 27\" Rapid IPS 240Hz chuyên game", Price = 3990000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/3/92.jpg", CategoryId = 3 },
        new Product { ProductId = 93, ProductName = "Màn hình MSI MPG 321URXW QD-OLED 32\" QD-LED 4K 240Hz chuyên game", Price = 30590000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/3/93.jpg", CategoryId = 3 },
        new Product { ProductId = 94, ProductName = "Màn hình MSI MAG 255XF 25\" Rapid IPS 300Hz FreeSync Premium chuyên game", Price = 4290000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/94.jpg", CategoryId = 3 },
        new Product { ProductId = 95, ProductName = "Màn hình MSI PRO MP251 E2 25\" IPS 120Hz", Price = 2190000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/95.jpg", CategoryId = 3 },
        //SamSung
        new Product { ProductId = 96, ProductName = "Màn hình cong Samsung LC34G55 34\" 2K 165Hz", Price = 7450000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/3/96.jpg", CategoryId = 3 },
        new Product { ProductId = 97, ProductName = "Màn hình cong Samsung Odyssey G9 LS49CG934 49\" OLED 2K 240Hz", Price = 28490000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/3/97.jpg", CategoryId = 3 },
        new Product { ProductId = 98, ProductName = "Màn hình Samsung Odyssey G8 LS32FG812SEXXV 32\" OLED 4K 240Hz", Price = 30990000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/3/98.jpg", CategoryId = 3 },
        new Product { ProductId = 99, ProductName = "Màn hình Samsung Odyssey 3D LS27FG900XEXXV 27\" IPS 4K 165Hz", Price = 36990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/99.jpg", CategoryId = 3 },
        new Product { ProductId = 100, ProductName = "Màn hình cong Samsung Odyssey G9 LS49DG930 49\" OLED 2K 240Hz USBC", Price = 36890000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/100.jpg", CategoryId = 3 },
        //ViewSonic
        new Product { ProductId = 101, ProductName = "Màn hình ViewSonic VX2480-2K-SHD-2 24\" IPS 2K 100Hz", Price = 3890000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/3/101.jpg", CategoryId = 3 },
        new Product { ProductId = 102, ProductName = "Màn hình ViewSonic ColorPro VP2456A 24\" IPS 120Hz USBC chuyên đồ hoạ", Price = 5290000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/3/102.jpg", CategoryId = 3 },
        new Product { ProductId = 103, ProductName = "Màn hình Viewsonic VA2432-H 24\" IPS 100Hz viền mỏng", Price = 1990000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/3/103.jpg", CategoryId = 3 },
        new Product { ProductId = 104, ProductName = "Màn hình Viewsonic VA2432-H-2 24\" IPS 100Hz viền mỏng", Price = 1990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/104.jpg", CategoryId = 3 },
        new Product { ProductId = 105, ProductName = "Màn hình ViewSonic VX2779-HD-PRO 27\" IPS 180Hz chuyên game", Price = 3290000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/3/105.jpg", CategoryId = 3 },

        // Loa
        new Product { ProductId = 106, ProductName = "Loa Edifier Bluetooth ES20 Black", Price = 890000, ProductDetail = "Âm thanh cân bằng, phù hợp nghe nhạc và làm việc tại nhà.", StockQuantity = 18, ImageUrl = "/Assets/img/Products/4/106.jpg", CategoryId = 4 },
        new Product { ProductId = 107, ProductName = "Loa Edifier Bluetooth ES300 Ivory", Price = 41900000, ProductDetail = "Loa gaming RGB đồng bộ ánh sáng theo trò chơi.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/4/107.jpg", CategoryId = 4 },
        new Product { ProductId = 108, ProductName = "Loa máy tính Edifier MR4 White", Price = 1790000, ProductDetail = "Thiết kế độc đáo, âm thanh vòm 3D chân thực.", StockQuantity = 7, ImageUrl = "/Assets/img/Products/4/108.jpg", CategoryId = 4 },
        new Product { ProductId = 109, ProductName = "Loa máy tính Edifier MR4", Price = 1790000, ProductDetail = "Loa nhỏ gọn, công suất ổn, giá rẻ phù hợp học sinh sinh viên.", StockQuantity = 30, ImageUrl = "/Assets/img/Products/4/109.jpg", CategoryId = 4 },
        new Product { ProductId = 110, ProductName = "Loa Edifier Bluetooth ES20 Ivory", Price = 940000, ProductDetail = "Âm thanh cân bằng, phù hợp nghe nhạc và làm việc tại nhà.", StockQuantity = 28, ImageUrl = "/Assets/img/Products/4/110.jpg", CategoryId = 4 },
        new Product { ProductId = 111, ProductName = "Loa Logitech G560", Price = 3890000, ProductDetail = "Thiết kế độc đáo, âm thanh vòm 3D chân thực.", StockQuantity = 26, ImageUrl = "/Assets/img/Products/4/111.jpg", CategoryId = 4 },
        new Product { ProductId = 112, ProductName = "Loa Razer Leviathan V2", Price = 5790000, ProductDetail = "Loa nhỏ gọn, công suất ổn, giá rẻ phù hợp học sinh sinh viên.", StockQuantity = 17, ImageUrl = "/Assets/img/Products/4/112.jpg", CategoryId = 4 },
        new Product { ProductId = 113, ProductName = "Loa Razer Leviathan V2 X", Price = 2490000, ProductDetail = "Loa gaming RGB đồng bộ ánh sáng theo trò chơi.", StockQuantity = 14, ImageUrl = "/Assets/img/Products/4/113.jpg", CategoryId = 4 },
        new Product { ProductId = 114, ProductName = "Loa Razer Nommo V2 X", Price = 3490000, ProductDetail = "Loa nhỏ gọn, công suất ổn, giá rẻ phù hợp học sinh sinh viên.", StockQuantity = 24, ImageUrl = "/Assets/img/Products/4/114.jpg", CategoryId = 4 },
        new Product { ProductId = 115, ProductName = "Loa Razer Nommo V2", Price = 6100000, ProductDetail = "Thiết kế độc đáo, âm thanh vòm 3D chân thực.", StockQuantity = 23, ImageUrl = "/Assets/img/Products/4/115.jpg", CategoryId = 4 },
        new Product { ProductId = 116, ProductName = "Loa SoundMax SB201 Grey", Price = 490000, ProductDetail = "Thiết kế độc đáo, âm thanh vòm 3D chân thực.", StockQuantity = 13, ImageUrl = "/Assets/img/Products/4/116.jpg", CategoryId = 4 },
        new Product { ProductId = 117, ProductName = "Loa Razer Leviathan V2", Price = 5790000, ProductDetail = "Loa nhỏ gọn, công suất ổn, giá rẻ phù hợp học sinh sinh viên.", StockQuantity = 17, ImageUrl = "/Assets/img/Products/4/117.jpg", CategoryId = 4 },
        new Product { ProductId = 118, ProductName = "Loa Razer Leviathan V2 X", Price = 2490000, ProductDetail = "Loa gaming RGB đồng bộ ánh sáng theo trò chơi.", StockQuantity = 14, ImageUrl = "/Assets/img/Products/4/118.jpg", CategoryId = 4 },
        new Product { ProductId = 119, ProductName = "Loa Razer Nommo V2 X", Price = 3490000, ProductDetail = "Loa nhỏ gọn, công suất ổn, giá rẻ phù hợp học sinh sinh viên.", StockQuantity = 24, ImageUrl = "/Assets/img/Products/4/119.jpg", CategoryId = 4 },


        // Chuột
        new Product { ProductId = 120, ProductName = "Chuột Logitech G102 LightSync Black", Price = 405000, ProductDetail = "Chuột gaming Logitech G102 LightSync Black – cảm biến chính xác, thiết kế RGB nổi bật.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/120.jpg", CategoryId = 5 },
        new Product { ProductId = 121, ProductName = "Chuột ASUS ROG Strix Impact III Wireless", Price = 1090000, ProductDetail = "Chuột không dây ASUS ROG Strix Impact III – độ trễ thấp, hiệu năng chơi game cao.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/121.jpg", CategoryId = 5 },
        new Product { ProductId = 122, ProductName = "Chuột Razer DeathAdder Essential", Price = 400000, ProductDetail = "Chuột Razer DeathAdder Essential – huyền thoại gaming với thiết kế công thái học thoải mái.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/122.jpg", CategoryId = 5 },
        new Product { ProductId = 123, ProductName = "Chuột ASUS ROG Strix Impact III Wireless White", Price = 1090000, ProductDetail = "Chuột ASUS ROG Strix Impact III Wireless White – phong cách trắng tinh tế, hiệu năng cao.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/123.jpg", CategoryId = 5 },
        new Product { ProductId = 124, ProductName = "Chuột Corsair Katar Pro Wireless", Price = 790000, ProductDetail = "Chuột Corsair Katar Pro Wireless – kết nối không dây nhanh, cảm biến chính xác.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/124.jpg", CategoryId = 5 },
        new Product { ProductId = 125, ProductName = "Chuột Razer DeathAdder Essential White", Price = 410000, ProductDetail = "Chuột Razer DeathAdder Essential White – chuột gaming huyền thoại với cảm biến chính xác.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/125.jpg", CategoryId = 5 },
        new Product { ProductId = 126, ProductName = "Chuột Logitech G304 Wireless", Price = 750000, ProductDetail = "Chuột không dây Logitech G304 – hiệu năng cao, tuổi thọ pin dài.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/126.jpg", CategoryId = 5 },
        new Product { ProductId = 127, ProductName = "Chuột Razer Basilisk V3", Price = 990000, ProductDetail = "Chuột Razer Basilisk V3 – cảm biến cao cấp, nút tùy chỉnh linh hoạt.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/127.jpg", CategoryId = 5 },
        new Product { ProductId = 128, ProductName = "Chuột Razer Không dây Viper V3 Pro Đen", Price = 3690000, ProductDetail = "Chuột Razer Viper V3 Pro Đen – siêu nhẹ, hiệu suất chuyên nghiệp.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/128.jpg", CategoryId = 5 },
        new Product { ProductId = 129, ProductName = "Chuột Logitech G502 Hero Gaming", Price = 790000, ProductDetail = "Chuột Logitech G502 Hero – độ chính xác cao, thiết kế công thái học.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/129.jpg", CategoryId = 5 },
        new Product { ProductId = 130, ProductName = "Chuột DareU Không dây EM911X RGB Đen", Price = 380000, ProductDetail = "Chuột DareU EM911X RGB Đen – giá rẻ, hiệu năng tốt, thiết kế đẹp.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/130.jpg", CategoryId = 5 },
        new Product { ProductId = 131, ProductName = "Chuột Asus TUF Gaming M3 Gen II", Price = 400000, ProductDetail = "Chuột Asus TUF Gaming M3 Gen II – bền bỉ, thiết kế chuẩn gaming.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/131.jpg", CategoryId = 5 },
        new Product { ProductId = 132, ProductName = "Chuột không dây Logitech M331 Silent Black", Price = 330000, ProductDetail = "Chuột Logitech M331 Silent – yên tĩnh, thoải mái, tiết kiệm năng lượng.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/132.jpg", CategoryId = 5 },
        new Product { ProductId = 133, ProductName = "Chuột Logitech G102 LightSync White", Price = 415000, ProductDetail = "Chuột Logitech G102 LightSync White – đèn RGB rực rỡ, độ chính xác cao.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/133.jpg", CategoryId = 5 },
        new Product { ProductId = 134, ProductName = "Chuột Logitech G Pro X Superlight 2 Dex Wireless Black", Price = 3290000, ProductDetail = "Chuột Logitech G Pro X Superlight 2 – siêu nhẹ, tốc độ và chính xác cao.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/134.jpg", CategoryId = 5 },
        new Product { ProductId = 135, ProductName = "Chuột Logitech G304 Wireless White", Price = 750000, ProductDetail = "Chuột Logitech G304 White – hiệu năng mạnh mẽ trong thiết kế nhỏ gọn.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/135.jpg", CategoryId = 5 },
        new Product { ProductId = 136, ProductName = "Chuột Logitech MX Master 3S Graphite", Price = 2390000, ProductDetail = "Chuột Logitech MX Master 3S – công thái học đỉnh cao, cuộn nhanh siêu êm.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/136.jpg", CategoryId = 5 },
        new Product { ProductId = 137, ProductName = "Chuột Logitech G Pro X Superlight 2 Dex Wireless White", Price = 2950000, ProductDetail = "Chuột Logitech G Pro X Superlight 2 White – trọng lượng nhẹ, phản hồi nhanh.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/137.jpg", CategoryId = 5 },
        new Product { ProductId = 138, ProductName = "Chuột Logitech Pebble Mouse 2 M350S Graphite", Price = 450000, ProductDetail = "Chuột Logitech Pebble M350S Graphite – nhỏ gọn, yên tĩnh, hiện đại.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/138.jpg", CategoryId = 5 },
        new Product { ProductId = 139, ProductName = "Chuột DareU Không dây EM911X RGB Trắng", Price = 380000, ProductDetail = "Chuột DareU EM911X RGB Trắng – phong cách trẻ trung, pin bền lâu.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/139.jpg", CategoryId = 5 },
        new Product { ProductId = 140, ProductName = "Chuột Razer Cobra", Price = 990000, ProductDetail = "Chuột Razer Cobra – tốc độ và độ chính xác cao cho game thủ.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/140.jpg", CategoryId = 5 },
        new Product { ProductId = 141, ProductName = "Chuột Razer Viper V3 HyperSpeed", Price = 1620000, ProductDetail = "Chuột Razer Viper V3 HyperSpeed – kết nối không dây siêu nhanh.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/141.jpg", CategoryId = 5 },
        new Product { ProductId = 142, ProductName = "Chuột Logitech G Pro X Superlight 2 Black", Price = 3290000, ProductDetail = "Chuột Logitech G Pro X Superlight 2 Black – chuẩn eSports chuyên nghiệp.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/142.jpg", CategoryId = 5 },
        new Product { ProductId = 143, ProductName = "Chuột Logitech Pebble Mouse 2 M350S Rose", Price = 450000, ProductDetail = "Chuột Logitech Pebble M350S Rose – màu hồng dễ thương, yên tĩnh.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/143.jpg", CategoryId = 5 },
        new Product { ProductId = 144, ProductName = "Chuột Razer Không dây Viper V3 Pro Trắng", Price = 3650000, ProductDetail = "Chuột Razer Viper V3 Pro Trắng – siêu nhẹ, hiệu năng cực đỉnh.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/144.jpg", CategoryId = 5 },
        new Product { ProductId = 145, ProductName = "Chuột ASUS ROG Strix Impact III", Price = 990000, ProductDetail = "Chuột ASUS ROG Strix Impact III – thiết kế nhỏ gọn, độ nhạy cao.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/145.jpg", CategoryId = 5 },
        new Product { ProductId = 146, ProductName = "Chuột Logitech MX Anywhere 3S Graphite", Price = 1590000, ProductDetail = "Chuột Logitech MX Anywhere 3S – linh hoạt, kết nối đa thiết bị.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/146.jpg", CategoryId = 5 },
        new Product { ProductId = 147, ProductName = "Chuột không dây DareU LM115G Black", Price = 150000, ProductDetail = "Chuột DareU LM115G Black – giá rẻ, phù hợp văn phòng, pin lâu.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/147.jpg", CategoryId = 5 },
        new Product { ProductId = 148, ProductName = "Chuột công thái học Logitech Lift Vertical", Price = 1250000, ProductDetail = "Chuột công thái học Logitech Lift Vertical – giảm căng thẳng cổ tay, thiết kế sang trọng.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/148.jpg", CategoryId = 5 },
        new Product { ProductId = 149, ProductName = "Chuột không dây Logitech M190 Black", Price = 250000, ProductDetail = "Chuột Logitech M190 Black – yên tĩnh, thoải mái, pin lâu.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/5/149.jpg", CategoryId = 5 },


        //Tai nghe
        new Product { ProductId = 150, ProductName = "Tai nghe Gaming Rapoo VH160S Black", Price = 450000, ProductDetail = "Tai nghe gaming Rapoo, thiết kế hiện đại, âm thanh sống động cho trải nghiệm chơi game thoải mái.", StockQuantity = 32, ImageUrl = "/Assets/img/Products/6/150.jpg", CategoryId = 6 },
        new Product { ProductId = 151, ProductName = "Tai nghe HP HyperX Alpha 2 Wireless Black", Price = 8390000, ProductDetail = "Tai nghe không dây HyperX Alpha 2, chất lượng âm thanh mạnh mẽ, phù hợp cho game thủ chuyên nghiệp.", StockQuantity = 18, ImageUrl = "/Assets/img/Products/6/151.jpg", CategoryId = 6 },
        new Product { ProductId = 152, ProductName = "Tai nghe Onikuma In-ear T302 TWS RGB Bluetooth Hồng", Price = 640000, ProductDetail = "Tai nghe Onikuma T302 màu hồng, kết nối Bluetooth TWS với hiệu ứng RGB độc đáo.", StockQuantity = 27, ImageUrl = "/Assets/img/Products/6/152.jpg", CategoryId = 6 },
        new Product { ProductId = 153, ProductName = "Tai nghe Onikuma In-ear T302 TWS RGB Bluetooth Xám Trắng", Price = 640000, ProductDetail = "Tai nghe Onikuma T302 phiên bản xám trắng, âm thanh rõ ràng, thiết kế hiện đại.", StockQuantity = 19, ImageUrl = "/Assets/img/Products/6/153.jpg", CategoryId = 6 },
        new Product { ProductId = 154, ProductName = "Tai nghe Onikuma In-ear T301 TWS Bluetooth Trắng", Price = 540000, ProductDetail = "Tai nghe Onikuma T301 trắng, nhỏ gọn, âm thanh trong trẻo, kết nối Bluetooth ổn định.", StockQuantity = 46, ImageUrl = "/Assets/img/Products/6/154.jpg", CategoryId = 6 },
        new Product { ProductId = 155, ProductName = "Tai nghe Onikuma In-ear T209 Bluetooth Live Translation Đen", Price = 590000, ProductDetail = "Tai nghe Onikuma T209 hỗ trợ dịch trực tiếp, thiết kế màu đen sang trọng.", StockQuantity = 24, ImageUrl = "/Assets/img/Products/6/155.jpg", CategoryId = 6 },
        new Product { ProductId = 156, ProductName = "Tai nghe Onikuma In-ear T20 TWS Bluetooth Trắng", Price = 640000, ProductDetail = "Tai nghe Onikuma T20 trắng, công nghệ Bluetooth TWS cho âm thanh không dây ổn định.", StockQuantity = 34, ImageUrl = "/Assets/img/Products/6/156.jpg", CategoryId = 6 },
        new Product { ProductId = 157, ProductName = "Tai nghe Onikuma In-ear T20 TWS Bluetooth Đen", Price = 640000, ProductDetail = "Tai nghe Onikuma T20 đen, âm thanh chất lượng, thiết kế tiện lợi.", StockQuantity = 21, ImageUrl = "/Assets/img/Products/6/157.jpg", CategoryId = 6 },
        new Product { ProductId = 158, ProductName = "Tai nghe Onikuma In-ear T18 ENC Bluetooth Trắng", Price = 590000, ProductDetail = "Tai nghe Onikuma T18 trắng, tích hợp ENC giảm ồn, âm thanh rõ ràng.", StockQuantity = 48, ImageUrl = "/Assets/img/Products/6/158.jpg", CategoryId = 6 },
        new Product { ProductId = 159, ProductName = "Tai nghe Onikuma In-ear T18 ENC Bluetooth Đen", Price = 590000, ProductDetail = "Tai nghe Onikuma T18 đen, thiết kế sang trọng, công nghệ ENC chống ồn hiệu quả.", StockQuantity = 42, ImageUrl = "/Assets/img/Products/6/159.jpg", CategoryId = 6 },
        new Product { ProductId = 160, ProductName = "Tai nghe Onikuma Tai Mèo B5 RGB Tri Mode Trắng", Price = 990000, ProductDetail = "Tai nghe Onikuma B5 tai mèo trắng, hỗ trợ 3 chế độ kết nối và hiệu ứng RGB.", StockQuantity = 36, ImageUrl = "/Assets/img/Products/6/160.jpg", CategoryId = 6 },
        new Product { ProductId = 161, ProductName = "Tai nghe Onikuma Tai Mèo B5 RGB Tri Mode Hồng", Price = 990000, ProductDetail = "Tai nghe Onikuma B5 tai mèo hồng, thiết kế đáng yêu, kết nối đa năng và RGB rực rỡ.", StockQuantity = 28, ImageUrl = "/Assets/img/Products/6/161.jpg", CategoryId = 6 },
        new Product { ProductId = 162, ProductName = "Tai nghe Razer Kraken Kitty V2 BT Quartz", Price = 2290000, ProductDetail = "Tai nghe Razer Kraken Kitty V2 Quartz, thiết kế tai mèo màu hồng, kết nối Bluetooth hiện đại.", StockQuantity = 14, ImageUrl = "/Assets/img/Products/6/162.jpg", CategoryId = 6 },
        new Product { ProductId = 163, ProductName = "Tai nghe Razer Kraken Kitty V2 BT White", Price = 2290000, ProductDetail = "Tai nghe Razer Kraken Kitty V2 màu trắng, âm thanh sống động và thiết kế đẹp mắt.", StockQuantity = 38, ImageUrl = "/Assets/img/Products/6/163.jpg", CategoryId = 6 },
        new Product { ProductId = 164, ProductName = "Tai nghe Razer Kraken Kitty V2 BT Black", Price = 2290000, ProductDetail = "Tai nghe Razer Kraken Kitty V2 màu đen, âm thanh mạnh mẽ và pin bền bỉ.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/6/164.jpg", CategoryId = 6 },
        new Product { ProductId = 165, ProductName = "Tai nghe DareU EH925 MAX Black Red", Price = 890000, ProductDetail = "Tai nghe DareU EH925 MAX, màu đen đỏ thể thao, chất lượng âm thanh nổi bật.", StockQuantity = 30, ImageUrl = "/Assets/img/Products/6/165.jpg", CategoryId = 6 },
        new Product { ProductId = 166, ProductName = "Tai nghe Razer Kraken Kitty V3 X White", Price = 1790000, ProductDetail = "Tai nghe Razer Kraken Kitty V3 X màu trắng, thiết kế thời trang và âm thanh cao cấp.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/6/166.jpg", CategoryId = 6 },
        new Product { ProductId = 167, ProductName = "Tai nghe Razer Kraken Kitty V3 X Quartz", Price = 1790000, ProductDetail = "Tai nghe Razer Kraken Kitty V3 X màu hồng Quartz, phong cách và hiệu suất vượt trội.", StockQuantity = 13, ImageUrl = "/Assets/img/Products/6/167.jpg", CategoryId = 6 },
        new Product { ProductId = 168, ProductName = "Tai nghe Razer Kraken Kitty V3 X Black", Price = 1790000, ProductDetail = "Tai nghe Razer Kraken Kitty V3 X màu đen, thiết kế đẹp và âm thanh sống động.", StockQuantity = 44, ImageUrl = "/Assets/img/Products/6/168.jpg", CategoryId = 6 },
        new Product { ProductId = 169, ProductName = "Tai nghe DareU EH732 RGB Red", Price = 390000, ProductDetail = "Tai nghe DareU EH732 RGB màu đỏ, hiệu ứng ánh sáng đẹp mắt và âm thanh tốt.", StockQuantity = 17, ImageUrl = "/Assets/img/Products/6/169.jpg", CategoryId = 6 },
        new Product { ProductId = 170, ProductName = "Tai nghe DareU EH732 RGB Yellow", Price = 390000, ProductDetail = "Tai nghe DareU EH732 RGB màu vàng, thiết kế bền và đeo thoải mái.", StockQuantity = 23, ImageUrl = "/Assets/img/Products/6/170.jpg", CategoryId = 6 },
        new Product { ProductId = 171, ProductName = "Tai nghe Logitech G Pro X SE", Price = 2100000, ProductDetail = "Tai nghe Logitech G Pro X SE cao cấp, dành cho game thủ chuyên nghiệp.", StockQuantity = 31, ImageUrl = "/Assets/img/Products/6/171.jpg", CategoryId = 6 },
        new Product { ProductId = 172, ProductName = "Tai nghe Razer BlackShark V3 X HyperSpeed", Price = 2990000, ProductDetail = "Tai nghe Razer BlackShark V3 X HyperSpeed, kết nối không dây tốc độ cao.", StockQuantity = 27, ImageUrl = "/Assets/img/Products/6/172.jpg", CategoryId = 6 },
        new Product { ProductId = 173, ProductName = "Tai nghe Razer BlackShark V3", Price = 4790000, ProductDetail = "Tai nghe Razer BlackShark V3, chất lượng âm thanh đỉnh cao và thiết kế chắc chắn.", StockQuantity = 16, ImageUrl = "/Assets/img/Products/6/173.jpg", CategoryId = 6 },
        new Product { ProductId = 174, ProductName = "Tai nghe Razer BlackShark V3 Pro White", Price = 7890000, ProductDetail = "Tai nghe Razer BlackShark V3 Pro màu trắng, công nghệ âm thanh cao cấp.", StockQuantity = 43, ImageUrl = "/Assets/img/Products/6/174.jpg", CategoryId = 6 },
        new Product { ProductId = 175, ProductName = "Tai nghe Razer BlackShark V3 Pro Black", Price = 7890000, ProductDetail = "Tai nghe Razer BlackShark V3 Pro màu đen, âm thanh mạnh mẽ và sang trọng.", StockQuantity = 41, ImageUrl = "/Assets/img/Products/6/175.jpg", CategoryId = 6 },
        new Product { ProductId = 176, ProductName = "Tai nghe Razer BlackShark V2 X For PlayStation", Price = 1050000, ProductDetail = "Tai nghe Razer BlackShark V2 X dành cho PlayStation, hiệu suất và sự thoải mái tuyệt vời.", StockQuantity = 37, ImageUrl = "/Assets/img/Products/6/176.jpg", CategoryId = 6 },
        new Product { ProductId = 177, ProductName = "Tai nghe Razer Barracuda X Chroma Black", Price = 3490000, ProductDetail = "Tai nghe Razer Barracuda X Chroma Black, thiết kế hiện đại và đèn RGB nổi bật.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/6/177.jpg", CategoryId = 6 },
        new Product { ProductId = 178, ProductName = "Tai nghe HP HyperX Cloud III S Wireless Black", Price = 4090000, ProductDetail = "Tai nghe HyperX Cloud III S Wireless Black, không dây, âm thanh đắm chìm.", StockQuantity = 26, ImageUrl = "/Assets/img/Products/6/178.jpg", CategoryId = 6 },
        new Product { ProductId = 179, ProductName = "Tai nghe HP HyperX Cloud III S Wireless Black Red", Price = 4090000, ProductDetail = "Tai nghe HyperX Cloud III S Wireless Black Red, phong cách, pin lâu và âm thanh mạnh mẽ.", StockQuantity = 33, ImageUrl = "/Assets/img/Products/6/179.jpg", CategoryId = 6 },


        //Bàn phím
        new Product { ProductId = 180, ProductName = "Bàn phím Leobog Hi86 TM", Price = 2090000, ProductDetail = "sản phẩm nổi bật trong dòng bàn phím cơ gaming với thiết kế hiện đại và nhiều tính năng ưu việt.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/6/180.jpg", CategoryId = 6 },
        new Product { ProductId = 181, ProductName = "Bàn phím Rapoo NK1900 màu đen", Price = 170000, ProductDetail = "Bàn phím giả cơ giá rẻ, phù hợp cho văn phòng và học tập. Thiết kế full-size, phím bấm êm, hành trình phím tốt và được khắc laser chống mờ. Có khả năng chống tràn nước hiệu quả.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/6/181.jpg", CategoryId = 7 },
        new Product { ProductId = 182, ProductName = "Bàn phím cơ E-Dra không dây EK375 Pro Beta Red Switch", Price = 1290000, ProductDetail = "Bàn phím cơ layout 75% nhỏ gọn, kết nối đa dạng với 3 chế độ: USB-C, Bluetooth 5.0, và 2.4Ghz. Sử dụng Red Switch cho cảm giác gõ mượt mà, lý tưởng cho cả gaming và gõ văn bản.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/6/182.jpg", CategoryId = 7 },
        new Product { ProductId = 183, ProductName = "Bàn phím cơ HyperX Alloy Origins Core TKL RGB Aqua Switch", Price = 2090000, ProductDetail = "Thiết kế TKL (Tenkeyless) chuyên nghiệp cho game thủ. Thân phím hoàn toàn bằng nhôm máy bay chắc chắn, trang bị switch HyperX Aqua (Tactile) cho phản hồi nhanh và chính xác. LED RGB rực rỡ tùy chỉnh qua phần mềm NGENUITY.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/6/183.jpg", CategoryId = 7 },
        new Product { ProductId = 184, ProductName = "Bàn phím cơ gaming E-Dra EK375v2 Beta Linear switch", Price = 690000, ProductDetail = "Phiên bản nâng cấp của E-Dra EK375, layout 75% tối ưu không gian. Sử dụng Beta Linear switch cho trải nghiệm gõ trơn tru, mượt mà. Keycap PBT double-shot bền bỉ, LED RGB 16.8 triệu màu.", StockQuantity = 16, ImageUrl = "/Assets/img/Products/6/184.jpg", CategoryId = 7 },
        new Product { ProductId = 185, ProductName = "Bàn phím cơ Gaming DAREU EK87 v2 Gray Black Dream Switch", Price = 550000, ProductDetail = "Bàn phím TKL (87 phím) giá cả phải chăng, sử dụng Dream switch độc quyền của DareU cho cảm giác gõ êm ái. Keycap ABS Doubleshot và hệ thống LED Rainbow tạo điểm nhấn.", StockQuantity = 11, ImageUrl = "/Assets/img/Products/6/185.jpg", CategoryId = 7 },
        new Product { ProductId = 186, ProductName = "Bàn phím AKKO 5075B Plus Red World Tour VIET NAM", Price = 1790000, ProductDetail = "Phiên bản đặc biệt với chủ đề 'World Tour Việt Nam', thiết kế đậm chất văn hóa Việt. Layout 75% nhỏ gọn, kết cấu Gasket Mount êm ái, hỗ trợ 3 chế độ kết nối và tính năng Hotswap tiện lợi.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/6/186.jpg", CategoryId = 7},
        new Product { ProductId = 187, ProductName = "Bàn phím cơ gaming E-Dra EK375v2 Alpha Linear switch", Price = 690000, ProductDetail = "Tương tự phiên bản Beta, E-Dra EK375v2 Alpha mang đến trải nghiệm gõ Linear mượt mà, không khấc. Layout 75%, keycap PBT bền bỉ và LED RGB sống động, là lựa chọn tuyệt vời trong phân khúc.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/6/187.jpg", CategoryId = 7 },
        new Product { ProductId = 188, ProductName = "Bàn phím SKYLOONG GK104 PRO Twilight Tri-mode CuCl2 Silent Switch", Price = 2490000, ProductDetail = "Bàn phím full-size cao cấp với 3 chế độ kết nối (Tri-mode). Trang bị CuCl2 Silent Switch siêu yên tĩnh, lý tưởng cho môi trường làm việc chung. Thiết kế Gasket Mount và keycap PBT chất lượng cao.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/6/188.jpg", CategoryId = 7 },
        new Product { ProductId = 189, ProductName = "Bàn phím AKKO TAC87 3 MODE Prunus Lannesiana Stellar Rose switch", Price = 1290000, ProductDetail = "Thiết kế TKL 87 phím với tông màu Prunus Lannesiana (Hoa Anh Đào) nghệ thuật. Kết nối 3 chế độ, sử dụng Stellar Rose switch (Linear) cho cảm giác gõ nhẹ nhàng và êm ái.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/6/189.jpg", CategoryId = 7 },
        new Product { ProductId = 190, ProductName = "Bàn phím cơ AKKO 5075B Plus TM Chicago Piano V3 Pro Switch", Price = 1490000, ProductDetail = "Layout 75% với phối màu Chicago cổ điển. Sử dụng switch AKKO V3 Pro (Piano) cao cấp, kết cấu Gasket Mount và foam tiêu âm cho âm thanh gõ đặc trưng, thỏa mãn người dùng khó tính.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/6/190.jpg", CategoryId = 7 },
        new Product { ProductId = 191, ProductName = "Bàn phím AULA F68 TM (Xanh lá/ Ice Crystal switch ) F68G", Price = 2490000, ProductDetail = "Bàn phím nhỏ gọn (68 phím) với kết cấu Gasket Mount. Ice Crystal switch trong suốt độc đáo cho cảm giác gõ mượt mà và âm thanh trong trẻo. Hỗ trợ 3 chế độ kết nối, keycap PBT double-shot.", StockQuantity = 11, ImageUrl = "/Assets/img/Products/6/191.jpg", CategoryId = 7 },
        new Product { ProductId = 192, ProductName = "Bàn phím AULA S100 PRO TM (Xanh dương+trắng+tím đậm/ Brown switch) S100PRO02", Price = 750000, ProductDetail = "Bàn phím full-size 100 phím nhỏ gọn, tiết kiệm không gian. Kết nối 3 chế độ tiện lợi, sử dụng Brown switch cho phản hồi xúc giác rõ ràng, cân bằng giữa chơi game và làm việc.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/6/192.jpg", CategoryId = 7 },
        new Product { ProductId = 193, ProductName = "Bàn phím Leobog AMG65 TM (Tím đen/ Jadeite switch) AMG6501", Price = 2090000, ProductDetail = "Thiết kế 65% độc đáo với vỏ nhôm CNC cao cấp. Jadeite switch mang lại trải nghiệm gõ đầm tay và âm thanh nổ giòn. Kết nối 3 chế độ, LED RGB và núm xoay kim loại đa chức năng.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/6/193.jpg", CategoryId = 7 },
        new Product { ProductId = 194, ProductName = "Bàn phím AULA F2058 có dây (Đen Red switch) F205802", Price = 690000, ProductDetail = "Bàn phím cơ có dây full-size với thiết kế hầm hố và kê tay đi kèm. Red switch mang lại lợi thế trong game nhờ tốc độ phản hồi nhanh. LED RGB Rainbow nhiều hiệu ứng đẹp mắt.", StockQuantity = 18, ImageUrl = "/Assets/img/Products/6/194.jpg", CategoryId = 7 },
        new Product { ProductId = 195, ProductName = "Bàn phím FL-Esports CMK75 Không dây Desert Grey Blue FLCMMK Ice Pink", Price = 1490000, ProductDetail = "Layout 75% kèm núm xoay tiện lợi, kết cấu Gasket Mount êm ái. Phối màu Desert Grey Blue, sử dụng Ice Pink switch (Linear) cho cảm giác gõ nhẹ và mượt, hỗ trợ 3 chế độ kết nối.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/6/195.jpg", CategoryId = 7 },
        new Product { ProductId = 196, ProductName = "Bàn phím Leobog AMG65 TM (Trắng/ Jadeite switch) AMG6502", Price = 2090000, ProductDetail = "Phiên bản màu trắng tinh khôi của dòng AMG65 vỏ nhôm. Layout 65% nhỏ gọn, Jadeite switch cho âm thanh gõ độc đáo. Kết nối 3 chế độ, núm xoay kim loại và LED RGB.", StockQuantity = 19, ImageUrl = "/Assets/img/Products/6/196.jpg", CategoryId = 7 },
        new Product { ProductId = 197, ProductName = "Bàn phím AKKO TAC87 3 MODE Matcha Red Bean Stellar Rose switch", Price = 1290000, ProductDetail = "Lấy cảm hứng từ Trà xanh Đậu đỏ, chiếc bàn phím TKL này có phối màu độc đáo. Kết nối 3 chế độ linh hoạt, Stellar Rose switch (Linear) nhẹ nhàng, phù hợp cho người dùng yêu thẩm mỹ.", StockQuantity = 23, ImageUrl = "/Assets/img/Products/6/197.jpg", CategoryId = 7 },
        new Product { ProductId = 198, ProductName = "Bàn phím AKKO TAC87 3 MODE Horizon Mirror switch", Price = 1290000, ProductDetail = "Bàn phím TKL với thiết kế Horizon độc đáo, kết nối 3 chế độ. Sử dụng Mirror switch (Linear) cho hành trình phím mượt mà, là một lựa chọn cân bằng giữa công việc và giải trí.", StockQuantity = 26, ImageUrl = "/Assets/img/Products/6/198.jpg", CategoryId = 7 },
        new Product { ProductId = 199, ProductName = "Bàn phím SKYLOONG GK104 PRO Quantum Mech 2 Tri-mode CuCl2 Silent Switch", Price = 2580000, ProductDetail = "Bàn phím full-size cao cấp dành cho người dùng chuyên nghiệp. Kết nối 3 chế độ, CuCl2 Silent Switch siêu tĩnh, kết cấu Gasket Mount và nhiều lớp foam tiêu âm cho trải nghiệm gõ yên tĩnh tuyệt đối.", StockQuantity = 16, ImageUrl = "/Assets/img/Products/6/199.jpg", CategoryId = 7 },
        new Product { ProductId = 200, ProductName = "Bàn phím cơ DareU EK98L Grey Black Dream switch", Price = 590000, ProductDetail = "Layout 98 phím độc đáo, giữ lại cụm numpad nhưng vẫn nhỏ gọn hơn full-size. Dream switch độc quyền của DareU cho cảm giác gõ êm, phù hợp cho cả game và văn phòng.", StockQuantity = 28, ImageUrl = "/Assets/img/Products/6/200.jpg", CategoryId = 7 },
        new Product { ProductId = 201, ProductName = "Bàn phím Logitech G515 TKL Lightspeed Wireless RGB Đen", Price = 2700000, ProductDetail = "Bàn phím low-profile siêu mỏng, kết nối không dây LIGHTSPEED tốc độ cao. Thiết kế TKL, switch GL Tactile cho phản hồi nhanh và chính xác. LED RGB LIGHTSYNC tùy chỉnh chuyên nghiệp.", StockQuantity = 30, ImageUrl = "/Assets/img/Products/6/1201.jpg", CategoryId = 7 },
        new Product { ProductId = 202, ProductName = "Bàn phím AULA M75 TM (Xanh + Trắng + Tím/ Purple Fire V2 Switch) M7503", Price = 1990000, ProductDetail = "Layout 75% có núm xoay, kết cấu Gasket Mount. Sử dụng Purple Fire V2 switch (Tactile) cho cảm giác gõ có khấc rõ ràng. Hỗ trợ 3 chế độ kết nối và keycap PBT chất lượng cao.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/6/202.jpg", CategoryId = 7 },
        new Product { ProductId = 203, ProductName = "Bàn phím FL-Esports CMK75 Không dây Lake Placid Blue FLCMMK Ice Pink", Price = 1990000, ProductDetail = "Phiên bản màu Lake Placid Blue của CMK75, layout 75% với núm xoay đa năng. Kết cấu Gasket mount, kết nối 3 chế độ, và switch Ice Pink (Linear) mang lại trải nghiệm gõ mượt mà.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/6/203.jpg", CategoryId = 7 },
        new Product { ProductId = 204, ProductName = "Bàn phím FL-Esports CMK75 Không dây Desert Grey Blue FLCMMK Ice Violet", Price = 1490000, ProductDetail = "Phiên bản Desert Grey Blue của CMK75 sử dụng Ice Violet switch (Tactile), mang lại phản hồi xúc giác rõ rệt khi gõ. Bàn phím có layout 75%, núm xoay và kết nối 3 chế độ.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/6/204.jpg", CategoryId = 7 },
        new Product { ProductId = 205, ProductName = "Bàn phím SKYLOONG GK104 PRO Quantum Mech 5 Tri-mode CuCl2 Silent Switch", Price = 2580000, ProductDetail = "Một phiên bản khác của dòng GK104 Pro, thiết kế full-size chuyên nghiệp, kết nối 3 chế độ và switch CuCl2 siêu tĩnh. Lý tưởng cho người cần sự yên tĩnh tối đa khi làm việc.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/6/205.jpg", CategoryId = 7 },
        new Product { ProductId = 206, ProductName = "Bàn phím Leobog Hi75C PRO TM (Đen contour/ Turquoise switch) HI7509", Price = 1650000, ProductDetail = "Bàn phím layout 75% vỏ nhôm cao cấp, kết cấu Gasket Mount. Turquoise switch (Linear) cho cảm giác gõ êm và mượt. Hỗ trợ 3 chế độ kết nối, núm xoay và LED RGB.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/6/206.jpg", CategoryId = 7 },
        new Product { ProductId = 207, ProductName = "Bàn phím AULA F81 TM (Tím/ Crystal switch) F8104", Price = 1990000, ProductDetail = "Layout 75% độc đáo, kết nối 3 chế độ và kết cấu Gasket Mount. Crystal switch (Linear) trong suốt cho hành trình phím mượt mà, kết hợp với LED RGB tạo hiệu ứng ánh sáng ấn tượng.", StockQuantity = 7, ImageUrl = "/Assets/img/Products/6/207.jpg", CategoryId = 7 },
        new Product { ProductId = 208, ProductName = "Bàn phím AULA F68 TM (Tím/ Ice Crystal switch ) F68V", Price = 2490000, ProductDetail = "Phiên bản màu tím của AULA F68, layout 68 phím nhỏ gọn. Sử dụng Ice Crystal switch (Linear) trong suốt, kết cấu Gasket Mount, kết nối 3 chế độ và keycap PBT bền bỉ.", StockQuantity = 9, ImageUrl = "/Assets/img/Products/6/208.jpg", CategoryId = 7 },
        new Product { ProductId = 209, ProductName = "Bàn phím cơ DareU EK75 Rapid Trigger Black", Price = 1750000, ProductDetail = "Bàn phím gaming chuyên nghiệp với công nghệ Rapid Trigger, cho phép kích hoạt và reset phím ở bất kỳ điểm nào trên hành trình phím, mang lại lợi thế tốc độ vượt trội trong các game đối kháng.", StockQuantity = 5, ImageUrl = "/Assets/img/Products/6/209.jpg", CategoryId = 7 },

        //Nguồn
     
        new Product { ProductId = 210, ProductName = "Nguồn ASUS ROG THOR 1600T3", Price = 24490000, ProductDetail = "Nguồn máy tính cao cấp ASUS ROG THOR, công suất 1600W, chuẩn Titanium.", StockQuantity = 25, ImageUrl = "/Assets/img/Products/8/210.jpg", CategoryId = 8 },
        new Product { ProductId = 211, ProductName = "Nguồn Máy Tính ASUS ROG THOR 1200 P3 1200W", Price = 13990000, ProductDetail = "Nguồn ASUS ROG THOR P3, công suất 1200W, chuẩn Platinum.", StockQuantity = 15, ImageUrl = "/Assets/img/Products/8/211.jpg", CategoryId = 8 },
        new Product { ProductId = 212, ProductName = "Nguồn máy tính Cooler Master V", Price = 7890000, ProductDetail = "Nguồn Cooler Master V series, hiệu suất cao và ổn định.", StockQuantity = 18, ImageUrl = "/Assets/img/Products/8/212.jpg", CategoryId = 8 },
        new Product { ProductId = 213, ProductName = "Nguồn máy tính MSI MEG AI1600T PCIE5", Price = 13990000, ProductDetail = "Nguồn MSI MEG AI series, 1600W, chuẩn Titanium, hỗ trợ PCIe 5.0.", StockQuantity = 7, ImageUrl = "/Assets/img/Products/8/213.jpg", CategoryId = 8 },
        new Product { ProductId = 214, ProductName = "Nguồn máy tính Cooler Master MWE GOLD 1250W V2 ATX3.1", Price = 5490000, ProductDetail = "Nguồn Cooler Master MWE Gold V2, 1250W, chuẩn 80 Plus Gold, tương thích ATX 3.1.", StockQuantity = 22, ImageUrl = "/Assets/img/Products/8/214.jpg", CategoryId = 8 },
        new Product { ProductId = 215, ProductName = "Nguồn máy tính Cooler Master MWE GOLD 1050W V2 ATX3.1", Price = 4490000, ProductDetail = "Nguồn Cooler Master MWE Gold V2, 1050W, chuẩn 80 Plus Gold, tương thích ATX 3.1.", StockQuantity = 28, ImageUrl = "/Assets/img/Products/8/215.jpg", CategoryId = 8 },
        new Product { ProductId = 216, ProductName = "Nguồn máy tính Cooler Master MWE 650", Price = 1390000, ProductDetail = "Nguồn Cooler Master MWE 650W, hiệu suất ổn định cho hệ thống tầm trung.", StockQuantity = 30, ImageUrl = "/Assets/img/Products/8/216.jpg", CategoryId = 8 },
        new Product { ProductId = 217, ProductName = "Nguồn máy tính Corsair RM1200x SHIFT White ATX 3.1", Price = 6150000, ProductDetail = "Nguồn Corsair RM1200x SHIFT, 1200W, màu trắng, ATX 3.1, thiết kế cắm cạnh hông.", StockQuantity = 11, ImageUrl = "/Assets/img/Products/8/217.jpg", CategoryId = 8 },
        new Product { ProductId = 218, ProductName = "Nguồn máy tính Cooler Master MWE GOLD 850W V2 ATX3.1", Price = 2990000, ProductDetail = "Nguồn Cooler Master MWE Gold V2, 850W, chuẩn 80 Plus Gold, hỗ trợ ATX 3.1.", StockQuantity = 19, ImageUrl = "/Assets/img/Products/8/218.jpg", CategoryId = 8 },
        new Product { ProductId = 219, ProductName = "Nguồn máy tính MSI MAG A500N-H - Active PFC (500W)", Price = 790000, ProductDetail = "Nguồn MSI MAG A500N-H, công suất 500W, trang bị Active PFC.", StockQuantity = 26, ImageUrl = "/Assets/img/Products/8/219.jpg", CategoryId = 8 },
        new Product { ProductId = 220, ProductName = "Nguồn máy tính Cooler Master MWE 750", Price = 1690000, ProductDetail = "Nguồn Cooler Master MWE 750W, lựa chọn phổ thông đáng tin cậy.", StockQuantity = 14, ImageUrl = "/Assets/img/Products/8/220.jpg", CategoryId = 8 },
        new Product { ProductId = 221, ProductName = "Nguồn máy tính ASUS ROG Thor 1000P2", Price = 8990000, ProductDetail = "Nguồn ASUS ROG Thor P2, công suất 1000W, chuẩn Platinum, hiệu suất cao.", StockQuantity = 9, ImageUrl = "/Assets/img/Products/8/221.jpg", CategoryId = 8 },
        new Product { ProductId = 222, ProductName = "Nguồn máy tính ASUS ROG Thor 1600T GAMING", Price = 16990000, ProductDetail = "Nguồn ASUS ROG Thor 1600T, 1600W, chuẩn Titanium, tối ưu cho gaming.", StockQuantity = 6, ImageUrl = "/Assets/img/Products/8/222.jpg", CategoryId = 8 },
        new Product { ProductId = 223, ProductName = "Nguồn máy tính Lian Li EDGE 1000W Black L-Shape", Price = 4690000, ProductDetail = "Nguồn Lian Li EDGE 1000W màu đen, thiết kế đầu cắm L-Shape độc đáo.", StockQuantity = 17, ImageUrl = "/Assets/img/Products/8/223.jpg", CategoryId = 8 },
        new Product { ProductId = 224, ProductName = "Nguồn máy tính Lian Li EDGE 1000W White L-Shape", Price = 4990000, ProductDetail = "Nguồn Lian Li EDGE 1000W màu trắng, thiết kế đầu cắm L-Shape tiện lợi.", StockQuantity = 23, ImageUrl = "/Assets/img/Products/8/224.jpg", CategoryId = 8 },
        new Product { ProductId = 225, ProductName = "Nguồn máy tính Thermaltake TOUGHPOWER GT 850W SNOW ", Price = 2790000, ProductDetail = "Nguồn Thermaltake TOUGHPOWER GT 850W, phiên bản Snow màu trắng.", StockQuantity = 20, ImageUrl = "/Assets/img/Products/8/225.jpg", CategoryId = 8 },
        new Product { ProductId = 226, ProductName = "Nguồn máy tính Lian Li EDGE 1300W White L-Shape", Price = 5990000, ProductDetail = "Nguồn Lian Li EDGE 1300W màu trắng, công suất lớn, thiết kế L-Shape.", StockQuantity = 12, ImageUrl = "/Assets/img/Products/8/226.jpg", CategoryId = 8 },
        new Product { ProductId = 227, ProductName = "Nguồn máy tính Thermaltake TOUGHPOWER GT 850W", Price = 2690000, ProductDetail = "Nguồn Thermaltake TOUGHPOWER GT, công suất 850W, hiệu suất cao.", StockQuantity = 29, ImageUrl = "/Assets/img/Products/8/227.jpg", CategoryId = 8 },
        new Product { ProductId = 228, ProductName = "guồn máy tính Lian Li EDGE 1300W Black L-Shape", Price = 5490000, ProductDetail = "Nguồn Lian Li EDGE 1300W màu đen, công suất lớn, thiết kế L-Shape.", StockQuantity = 8, ImageUrl = "/Assets/img/Products/8/228.jpg", CategoryId = 8 },
        new Product { ProductId = 229, ProductName = "Nguồn máy tính GIGABYTE AORUS ELITE P1000W PCIe 5.0", Price = 6490000, ProductDetail = "Nguồn GIGABYTE AORUS ELITE, 1000W, hỗ trợ chuẩn PCIe 5.0.", StockQuantity = 16, ImageUrl = "/Assets/img/Products/8/229.jpg", CategoryId = 8 },
        new Product { ProductId = 230, ProductName = "Nguồn máy tính GIGABYTE P650SS ICE ", Price = 1290000, ProductDetail = "Nguồn GIGABYTE P650SS, 650W, phiên bản ICE màu trắng.", StockQuantity = 24, ImageUrl = "/Assets/img/Products/8/230.jpg", CategoryId = 8 },
        new Product { ProductId = 231, ProductName = "Nguồn máy tính GIGABYTE P550SS", Price = 990000, ProductDetail = "Nguồn GIGABYTE P550SS, công suất 550W, phù hợp cho PC cơ bản.", StockQuantity = 27, ImageUrl = "/Assets/img/Products/8/231.jpg", CategoryId = 8 },
        new Product { ProductId = 232, ProductName = "Nguồn máy tính GIGABYTE AORUS ELITE P1000W PCIe 5.0 ICE", Price = 6590000, ProductDetail = "Nguồn GIGABYTE AORUS ELITE 1000W, bản ICE màu trắng, hỗ trợ PCIe 5.0.", StockQuantity = 13, ImageUrl = "/Assets/img/Products/8/232.jpg", CategoryId = 8 },
        new Product { ProductId = 233, ProductName = "Nguồn máy tính MSI MAG A1250GL PCIE5", Price = 5490000, ProductDetail = "Nguồn MSI MAG A1250GL, 1250W, chuẩn 80 Plus Gold, hỗ trợ PCIe 5.0.", StockQuantity = 10, ImageUrl = "/Assets/img/Products/8/233.jpg", CategoryId = 8 },
        new Product { ProductId = 234, ProductName = "Nguồn máy tính Deepcool PN850M - 80 Plus Gold - ATX 3.1", Price = 2890000, ProductDetail = "Nguồn Deepcool PN850M, 850W, chuẩn 80 Plus Gold, tương thích ATX 3.1.", StockQuantity = 21, ImageUrl = "/Assets/img/Products/8/234.jpg", CategoryId = 8 },
        new Product { ProductId = 235, ProductName = "Nguồn máy tính ASUS Prime 850W", Price = 3490000, ProductDetail = "Nguồn ASUS Prime 850W, chuẩn Gold, thiết kế đồng bộ với bo mạch chủ Prime.", StockQuantity = 18, ImageUrl = "/Assets/img/Products/8/235.jpg", CategoryId = 8 },
        new Product { ProductId = 236, ProductName = "Nguồn máy tính Corsair HX1200i - 80 Plus Platinum", Price = 7590000, ProductDetail = "Nguồn Corsair HX1200i, 1200W, chuẩn 80 Plus Platinum, có giám sát phần mềm.", StockQuantity = 5, ImageUrl = "/Assets/img/Products/8/236.jpg", CategoryId = 8 },
        new Product { ProductId = 237, ProductName = "Nguồn máy tính GIGABYTE UD850GM PG5", Price = 2990000, ProductDetail = "Nguồn GIGABYTE UD850GM, 850W, chuẩn Gold, hỗ trợ PCIe 5.0 (PG5).", StockQuantity = 25, ImageUrl = "/Assets/img/Products/8/237.jpg", CategoryId = 8 },
        new Product { ProductId = 238, ProductName = "Nguồn máy tính ASUS ROG Strix 1000W AURA White Edition", Price = 6990000, ProductDetail = "Nguồn ASUS ROG Strix 1000W, phiên bản màu trắng, hỗ trợ AURA Sync RGB.", StockQuantity = 12, ImageUrl = "/Assets/img/Products/8/238.jpg", CategoryId = 8 },
        new Product { ProductId = 239, ProductName = "Nguồn máy tính Thermaltake TOUGHPOWER GF A3 750W", Price = 2690000, ProductDetail = "Nguồn Thermaltake TOUGHPOWER GF A3, 750W, chuẩn Gold, tương thích ATX 3.0.", StockQuantity = 23, ImageUrl = "/Assets/img/Products/8/239.jpg", CategoryId = 8 }
        );  
        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
