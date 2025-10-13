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
        //Acer
        new Product { ProductId = 1, ProductName = "Laptop Acer Nitro V ANV15 51 500A", Price = 18390000,ProductDetail= "Laptop gaming mạnh mẽ, CPU Intel Gen 13, card RTX 4050, tản nhiệt kép.", StockQuantity = 10, ImageUrl = "/assets/img/Products/1/Acer/1.jpg", CategoryId = 1 },
        new Product { ProductId = 2, ProductName = "Laptop Acer Swift 3 SF314 511 55QE", Price = 11990000, ProductDetail = "Thiết kế mỏng nhẹ, vỏ nhôm cao cấp, hiệu năng mạnh mẽ với Intel Core i5 Gen 11, pin bền, phù hợp cho sinh viên và dân văn phòng.", StockQuantity = 8, ImageUrl = "/assets/img/Products/1/Acer/2.jpg", CategoryId = 1 },
        new Product { ProductId = 3, ProductName = "Laptop Acer Aspire Lite 14 AL14 71P 55P9", Price = 14990000, ProductDetail = "Laptop giá tốt, hiệu năng ổn định, màn hình 14 inch Full HD, thiết kế thanh lịch – lựa chọn lý tưởng cho nhu cầu học tập và làm việc cơ bản.", StockQuantity = 6, ImageUrl = "/assets/img/Products/1/Acer/3.jpg", CategoryId = 1 },
        new Product { ProductId = 4, ProductName = "Laptop Acer Swift X14 SFX14 72G 77F9", Price = 33990000, ProductDetail = "Hiệu năng cao với chip Intel Gen 12 và GPU RTX, đáp ứng tốt nhu cầu đồ họa, lập trình và sáng tạo nội dung trong thiết kế gọn nhẹ.", StockQuantity = 5, ImageUrl = "/assets/img/Products/1/Acer/4.jpg", CategoryId = 1 },
        new Product { ProductId = 5, ProductName = "Laptop ACER Swift Lite 14 SFL14 51M 56HS", Price = 18390000, ProductDetail = "Mỏng nhẹ chỉ khoảng 1,2kg, pin lâu, cấu hình mạnh mẽ, phù hợp người dùng thường xuyên di chuyển và làm việc ngoài văn phòng.", StockQuantity = 10, ImageUrl = "/assets/img/Products/1/Acer/5.jpg", CategoryId = 1 },
        new Product { ProductId = 6, ProductName = "Laptop Acer Swift Go SFG14 74 58FJ", Price = 28990000, ProductDetail = "Trang bị CPU Intel Gen 14 mới nhất, màn OLED sắc nét, hiệu năng mạnh mẽ và thiết kế sang trọng – tối ưu cho công việc lẫn giải trí.", StockQuantity = 8, ImageUrl = "/assets/img/Products/1/Acer/6.jpg", CategoryId = 1 },
        //Asus
        new Product { ProductId = 7, ProductName = "Laptop ASUS Vivobook K3605VC RP431W", Price = 18290000, ProductDetail = "Thiết kế mỏng nhẹ, hiệu năng cao, phù hợp cho học tập và làm việc.", StockQuantity = 8, ImageUrl = "/assets/img/Products/1/Asus/7.jpg", CategoryId = 1 },
        new Product { ProductId = 8, ProductName = "Laptop ASUS Vivobook S14 S3407CA LY095WS", Price = 20490000, ProductDetail = "Thiết kế mỏng nhẹ, màn hình OLED 14 inch, CPU Intel Core Ultra 5, pin bền và hiệu năng ổn định cho học tập và làm việc.", StockQuantity = 6, ImageUrl = "/assets/img/Products/1/Asus/8.jpg", CategoryId = 1 },
        new Product { ProductId = 9, ProductName = "Laptop ASUS Vivobook S14 S3407CA LY096WS", Price = 22990000, ProductDetail = "Trang bị chip Intel Core Ultra 7, màn hình OLED sắc nét, thiết kế hiện đại, hiệu năng mạnh cho người dùng văn phòng và sáng tạo nội dung.", StockQuantity = 9, ImageUrl = "/assets/img/Products/1/Asus/9.jpg", CategoryId = 1 },
        new Product { ProductId = 10, ProductName = "Laptop ASUS Zenbook 14 UX3405CA PZ204WS", Price = 34990000, ProductDetail = "Ultrabook cao cấp với màn OLED 2.8K, chip Intel Core Ultra 7, siêu mỏng nhẹ, pin lâu – lựa chọn hoàn hảo cho doanh nhân di chuyển nhiều.", StockQuantity = 7, ImageUrl = "/assets/img/Products/1/Asus/10.jpg", CategoryId = 1 },
        new Product { ProductId = 11, ProductName = "Laptop ASUS ExpertBook B1 BM1403CDA S60974W", Price = 12190000, ProductDetail = "Laptop doanh nhân bền bỉ, CPU AMD Ryzen 5, RAM 16GB, hiệu năng ổn định, phù hợp làm việc văn phòng và học tập chuyên nghiệp.", StockQuantity = 5, ImageUrl = "/assets/img/Products/1/Asus/11.jpg", CategoryId = 1 },
        new Product { ProductId = 12, ProductName = "Laptop ASUS Expertbook P1403CVA-i716-50W", Price = 17990000, ProductDetail = "Hiệu năng mạnh mẽ với Intel Core i7 Gen 13, thiết kế chắc chắn, bảo mật cao – tối ưu cho môi trường doanh nghiệp.", StockQuantity = 4, ImageUrl = "/assets/img/Products/1/Asus/12.jpg", CategoryId = 1 },
        //Dell
        new Product { ProductId = 13, ProductName = "Laptop Dell 15 DC15250 i7U161W11SLU", Price = 20990000, ProductDetail = "Hiệu năng mạnh với Intel Core i7 Gen 13, RAM 16GB, SSD 512GB, thiết kế hiện đại – phù hợp làm việc và giải trí.", StockQuantity = 12, ImageUrl = "/assets/img/Products/1/Dell/13.jpg", CategoryId = 1 },
        new Product { ProductId = 14, ProductName = "Laptop Dell Inspiron 5440-PUS i5-1334U/512GB/16GB DDR5/14/FHD/W11/Carbon Black", Price = 16990000, ProductDetail = "Laptop mỏng nhẹ, CPU Intel Core i5 Gen 13, RAM 16GB, SSD 512GB, màn 14 FHD – lý tưởng cho sinh viên và dân văn phòng.", StockQuantity = 10, ImageUrl = "/assets/img/Products/1/Dell/14.jpg", CategoryId = 1 },
        new Product { ProductId = 15, ProductName = "Laptop Dell Inspirion N3530 i5U165W11SLU (1334U)", Price = 15990000, ProductDetail = "Thiết kế tinh tế, chip i5 Gen 13, hiệu năng ổn định, pin tốt – đáp ứng nhu cầu học tập và làm việc hằng ngày.", StockQuantity = 8, ImageUrl = "/assets/img/Products/1/Dell/15.jpg", CategoryId = 1 },
        new Product { ProductId = 16, ProductName = "Laptop Dell XPS 9350 XPS9350-U5IA165W11GR-FP", Price = 54990000, ProductDetail = "Ultrabook cao cấp, vỏ nhôm nguyên khối, màn hình sắc nét, chip i7, RAM 16GB – dành cho người dùng chuyên nghiệp và sáng tạo.", StockQuantity = 7, ImageUrl = "/assets/img/Products/1/Dell/16.jpg", CategoryId = 1 },
        //HP
        new Product { ProductId = 17, ProductName = "Laptop gaming HP Victus 16-r0369TX AY8Y2PA", Price = 29490000, ProductDetail = "Laptop gaming hiệu năng cao, CPU Intel Core i7 Gen 13, GPU RTX 4050, màn 16.1 – chơi game và làm đồ họa mượt mà.", StockQuantity = 13, ImageUrl = "/assets/img/Products/1/HP/17.jpg", CategoryId = 1 },
        new Product { ProductId = 18, ProductName = "Laptop gaming HP Victus 16-s1149AX AZ0D4PA", Price = 19990000, ProductDetail = "Trang bị Ryzen 7 7840HS và RTX 4060, hiệu năng mạnh, tản nhiệt tốt – phù hợp game thủ và nhà sáng tạo nội dung.", StockQuantity = 11, ImageUrl = "/assets/img/Products/1/HP/18.jpg", CategoryId = 1 },
        new Product { ProductId = 19, ProductName = "Laptop gaming HP VICTUS 15 fb3116AX BX8U4PA", Price = 20990000, ProductDetail = "Thiết kế trẻ trung, chip Ryzen 5 7535HS, GPU RTX 2050, hiệu năng ổn định, chơi game và học tập tốt.", StockQuantity = 9, ImageUrl = "/assets/img/Products/1/HP/19.jpg", CategoryId = 1 },
        new Product { ProductId = 20, ProductName = "Laptop gaming HP OMEN 16-am0180TX BX8Y6PA", Price = 31990000, ProductDetail = "Dòng cao cấp, CPU i7 Gen 13, GPU RTX 4070, màn QHD 165Hz, bàn phím RGB – trải nghiệm gaming đỉnh cao.", StockQuantity = 8, ImageUrl = "/assets/img/Products/1/HP/20.jpg", CategoryId = 1 },
        //Lenovo
        new Product { ProductId = 21, ProductName = "Laptop gaming Lenovo Legion 5 15IRX10 83LY00A7VN", Price = 37490000, ProductDetail = "Hiệu năng mạnh với Intel Core i7 Gen 13, GPU RTX 4060, tản nhiệt hiệu quả – chiến mượt mọi tựa game nặng.", StockQuantity = 14, ImageUrl = "/assets/img/Products/1/Lenovo/21.jpg", CategoryId = 1 },
        new Product { ProductId = 22, ProductName = "Laptop gaming Lenovo LOQ 15ARP9 83JC00M3VN", Price = 22290000, ProductDetail = "Trang bị Ryzen 7 7840HS, RTX 4050, thiết kế gaming hiện đại, bàn phím RGB – hiệu suất cao cho game và đồ họa.", StockQuantity = 12, ImageUrl = "/assets/img/Products/1/Lenovo/22.jpg", CategoryId = 1 },
        new Product { ProductId = 23, ProductName = "Laptop gaming Lenovo Legion 5 15AHP10 83M0002YVN", Price = 36990000, ProductDetail = "Sức mạnh từ Ryzen 7 8845HS và RTX 4070, màn 165Hz mượt mà – tối ưu cho game thủ chuyên nghiệp.", StockQuantity = 10, ImageUrl = "/assets/img/Products/1/Lenovo/23.jpg", CategoryId = 1 },
        new Product { ProductId = 24, ProductName = "Laptop gaming Lenovo LOQ 15AHP10 83JG0047VN", Price = 30790000, ProductDetail = "Laptop gaming tầm trung, CPU Ryzen 5 8645HS, GPU RTX 3050, hiệu năng ổn định – cân tốt các game phổ biến.", StockQuantity = 9, ImageUrl = "/assets/img/Products/1/Lenovo/24.jpg", CategoryId = 1 },
        //MSI
        new Product { ProductId = 25, ProductName = "Laptop MSI Prestige 14 Evo B13M 401VN", Price = 22490000, ProductDetail = "Ultrabook mỏng nhẹ, CPU Intel Core i7 Gen 13, RAM 16GB, pin lâu, tối ưu cho sáng tạo nội dung và văn phòng.", StockQuantity = 7, ImageUrl = "/assets/img/Products/1/MSI/25.jpg", CategoryId = 1 },
        new Product { ProductId = 26, ProductName = "Laptop MSI Prestige 14 AI+ Evo C2VMG 020VN", Price = 18290000, ProductDetail = "Laptop mỏng nhẹ, chip Intel Core i5, RAM 8GB, SSD 512GB – hiệu năng ổn định cho học tập và làm việc cơ bản.", StockQuantity = 8, ImageUrl = "/assets/img/Products/1/MSI/26.jpg", CategoryId = 1 },
        new Product { ProductId = 27, ProductName = "Laptop MSI Modern 14 F13MG 466VN", Price = 25490000, ProductDetail = "Ultrabook cao cấp, CPU Intel Core i7 Gen 13, GPU RTX 4060, màn 16 sắc nét – dành cho thiết kế đồ họa và multimedia.", StockQuantity = 7, ImageUrl = "/assets/img/Products/1/MSI/27.jpg", CategoryId = 1 },
        new Product { ProductId = 28, ProductName = "Laptop MSI Prestige 16 AI+ Mercedes AMG B2VMG 088VN", Price = 38990000, ProductDetail = "Hiệu năng đỉnh cao với Core i9 và RTX 4080, chuyên game và đồ họa.", StockQuantity = 3, ImageUrl = "/assets/img/Products/1/MSI/28.jpg", CategoryId = 1 },
        new Product { ProductId = 29, ProductName = "Laptop MSI Modern 15 H C13M 216VN", Price = 17390000, ProductDetail = "Laptop gaming mạnh mẽ, CPU Intel Gen 13, card RTX 4050, tản nhiệt kép.", StockQuantity = 10, ImageUrl = "/assets/img/Products/1/MSI/29.jpg", CategoryId = 1 },

        // PC Gaming
        //i3
        new Product { ProductId = 30, ProductName = "PC GVN Intel i3-12100F/ VGA RX 6500XT (Powered by ASUS)", Price = 10490000,ProductDetail= "Cấu hình khủng với i3-12100F VGA RX 6500XT, chơi mọi tựa game ở 4K.", StockQuantity = 3, ImageUrl = "/assets/img/Products/2/i3/30.jpg", CategoryId = 2 },
        new Product { ProductId = 31, ProductName = "PC GVN Intel i3-12700F/ VGA RX 6500XT (Powered by ASUS)", Price = 12390000,ProductDetail= "Máy tính chơi game tầm trung, hiệu năng ổn định, tiết kiệm điện.", StockQuantity = 10, ImageUrl = "/assets/img/Products/2/i3/31.jpg", CategoryId = 2 },
        new Product { ProductId = 32, ProductName = "PC GVN Homework i3 12100", Price = 31690000, ProductDetail = "Cấu hình mạnh mẽ, chiến tốt mọi game Esport, hỗ trợ làm đồ họa.", StockQuantity = 7, ImageUrl = "/assets/img/Products/2/i3/32.jpg", CategoryId = 2 },
        new Product { ProductId = 33, ProductName = "PC GVN Homework I3 14100", Price = 135990000, ProductDetail = "Cấu hình siêu cấp dành cho streamer và gamer chuyên nghiệp.", StockQuantity = 12, ImageUrl = "/assets/img/Products/2/i3/33.jpg", CategoryId = 2 },
        new Product { ProductId = 34, ProductName = "PC GVN Homework i3 - GT", Price = 162990000, ProductDetail = "Cấu hình khủng với i3 - GT, chơi mọi tựa game ở 4K.", StockQuantity = 3, ImageUrl = "/assets/img/Products/2/i3/34.jpg", CategoryId = 2 },
        //i5
        new Product { ProductId = 35, ProductName = "PC GVN INTEL I5-12400F/VGA RTX 5050", Price = 17490000, ProductDetail = "Máy tính chơi game tầm trung, hiệu năng ổn định, tiết kiệm điện.", StockQuantity = 9, ImageUrl = "/assets/img/Products/2/i5/35.jpg", CategoryId = 2 },
        new Product { ProductId = 36, ProductName = "PC GVN x Corsair iCUE (Intel i5-14400F/ VGA RTX 5060)", Price = 28990000, ProductDetail = "Cấu hình mạnh mẽ, chiến tốt mọi game Esport, hỗ trợ làm đồ họa.", StockQuantity = 6, ImageUrl = "/assets/img/Products/2/i5/36.jpg", CategoryId = 2 },
        new Product { ProductId = 37, ProductName = "PC GVN Intel i5-12400F/ VGA RTX 5060 (Main H)", Price = 18490000, ProductDetail = "Cấu hình siêu cấp dành cho streamer và gamer chuyên nghiệp.", StockQuantity = 5, ImageUrl = "/assets/img/Products/2/i5/37.jpg", CategoryId = 2 },
        new Product { ProductId = 38, ProductName = "PC GVN Intel i5-12400F/ VGA RTX 4060", Price = 17990000, ProductDetail = "Cấu hình khủng với Ryzen 9 và RTX 4060, chơi mọi tựa game ở 4K.", StockQuantity = 2, ImageUrl = "/assets/img/Products/2/i5/38.jpg", CategoryId = 2 },
        new Product { ProductId = 39, ProductName = "PC GVN x MSI PROJECT ZERO WHITE (Intel i5-14400F/ VGA RTX 5060)", Price = 30990000, ProductDetail = "Máy tính chơi game tầm trung, hiệu năng ổn định, tiết kiệm điện.", StockQuantity = 8, ImageUrl = "/assets/img/Products/2/i5/39.jpg", CategoryId = 2 },
        new Product { ProductId = 40, ProductName = "PC GVN Intel i5-14400F/ VGA RTX 5060 Ti", Price = 30790000, ProductDetail = "Máy tính chơi game tầm trung, hiệu năng ổn định, tiết kiệm điện.", StockQuantity = 4, ImageUrl = "/assets/img/Products/2/i5/40.jpg", CategoryId = 2 },
        //i7
        new Product { ProductId = 41, ProductName = "PC GVN Intel i7-14700F/ VGA RTX 3050", Price = 31690000, ProductDetail = "Cấu hình mạnh mẽ, chiến tốt mọi game Esport, hỗ trợ làm đồ họa.", StockQuantity = 7, ImageUrl = "/assets/img/Products/2/i7/41.jpg", CategoryId = 2 },
        new Product { ProductId = 42, ProductName = "PC GVN Intel i7-14700F/ VGA RTX 3060", Price = 235990000, ProductDetail = "Cấu hình siêu cấp dành cho streamer và gamer chuyên nghiệp.", StockQuantity = 12, ImageUrl = "/assets/img/Products/2/i7/42.jpg", CategoryId = 2 },
        new Product { ProductId = 43, ProductName = "PC GVN Intel i7-14700F/ VGA RTX 4060", Price = 462990000, ProductDetail = "Cấu hình khủng với i7-14700F và VGA RTX 4060, chơi mọi tựa game ở 4K.", StockQuantity = 3, ImageUrl = "/assets/img/Products/2/i7/43.jpg", CategoryId = 2 },
        new Product { ProductId = 44, ProductName = "PC GVN Intel i7-14700F/ VGA RTX 5080", Price = 58390000, ProductDetail = "Máy tính chơi game tầm trung, hiệu năng ổn định, tiết kiệm điện.", StockQuantity = 10, ImageUrl = "/assets/img/Products/2/i7/44.jpg", CategoryId = 2 },
        new Product { ProductId = 45, ProductName = "PC GVN Intel i7-14700F/ VGA RTX 5060", Price = 51690000, ProductDetail = "Cấu hình mạnh mẽ, chiến tốt mọi game Esport, hỗ trợ làm đồ họa.", StockQuantity = 7, ImageUrl = "/assets/img/Products/2/i7/45.jpg", CategoryId = 2 },
        //i9
        new Product { ProductId = 46, ProductName = "PC GVN x AORUS XTREME ICE (Intel i9-14900K/ VGA RTX 4080 Super)", Price = 135000000, ProductDetail = "Cấu hình siêu cấp dành cho streamer và gamer chuyên nghiệp.", StockQuantity = 2, ImageUrl = "/assets/img/Products/2/i9/46.jpg", CategoryId = 2 },
        new Product { ProductId = 47, ProductName = "PC GVN Intel i9-14900K/ VGA RTX 4060 Ti", Price = 52990000, ProductDetail = "Cấu hình siêu cấp dành cho streamer và gamer chuyên nghiệp ko chuyên.", StockQuantity = 3, ImageUrl = "/assets/img/Products/2/i9/47.jpg", CategoryId = 2 },


        // Màn hình
        //Acer
        new Product { ProductId = 48, ProductName = "Màn hình Acer SA272U-E White 27\" IPS 2K 100Hz", Price = 3890000,ProductDetail= "Màn hình 27 inch, độ phân giải 2K, tấm nền IPS cho màu sắc trung thực, tần số quét 100Hz, thiết kế trắng hiện đại, phù hợp làm việc và giải trí.", StockQuantity = 15, ImageUrl = "/assets/img/Products/3/Acer/48.jpg", CategoryId = 3 },
        new Product { ProductId = 49, ProductName = "Màn hình Acer KA272 G0 27\" IPS 120Hz", Price = 2550000, ProductDetail = "Màn hình 27 inch IPS, tần số quét 120Hz mượt mà, hiển thị sắc nét, thích hợp chơi game và làm việc văn phòng.", StockQuantity = 20, ImageUrl = "/assets/img/Products/3/Acer/49.jpg", CategoryId = 3 },
        new Product { ProductId = 50, ProductName = "Màn hình Acer EK251Q G 25\" IPS 120Hz", Price = 2290000, ProductDetail = "Màn hình 25 inch, IPS 120Hz, thời gian phản hồi nhanh, lý tưởng cho game và giải trí hàng ngày.", StockQuantity = 8, ImageUrl = "/assets/img/Products/3/Acer/50.jpg", CategoryId = 3 },
        new Product { ProductId = 51, ProductName = "Màn hình Acer VG271U M3 27\" IPS 2K 180Hz chuyên game", Price = 41990000, ProductDetail = "Màn hình 27 inch, độ phân giải 2K, IPS, tần số quét siêu cao 180Hz, tối ưu cho game thủ, hình ảnh mượt mà và sắc nét.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/Acer/51.jpg", CategoryId = 3 },
        new Product { ProductId = 52, ProductName = "Màn hình ACER EK221Q E3 22\" IPS 100Hz", Price = 17900000, ProductDetail = "Màn hình 22 inch, IPS, tần số quét 100Hz, hiển thị màu sắc chính xác, gọn nhẹ, phù hợp học tập và làm việc văn phòng.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/Acer/52.jpg", CategoryId = 3 },
        //AOC
        new Product { ProductId = 53, ProductName = "Màn hình cong AOC C27G4Z 27\" 280Hz Adaptive Sync chuyên game", Price = 7290000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/assets/img/Products/3/AOC/53.jpg", CategoryId = 3 },
        new Product { ProductId = 54, ProductName = "Màn hình AOC Q27G40E 27\" Fast IPS 2K 180Hz chuyên game", Price = 4990000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/assets/img/Products/3/AOC/54.jpg", CategoryId = 3 },
        new Product { ProductId = 55, ProductName = "Màn hình AOC 25G4K 25\" Fast IPS 420Hz chuyên game", Price = 5290000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/assets/img/Products/3/AOC/55.jpg", CategoryId = 3 },
        new Product { ProductId = 56, ProductName = "Màn hình AOC 24G11ZE 24\" Fast IPS 240Hz chuyên game", Price = 33990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/AOC/56.jpg", CategoryId = 3 },
        new Product { ProductId = 57, ProductName = "Màn hình AOC Agon Pro AG276QSD 27\" OLED 2K 360Hz chuyên game", Price = 33990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/AOC/57.jpg", CategoryId = 3 },
        //Asus
        new Product { ProductId = 58, ProductName = "Màn hình di động Asus ZenScreen MB169CK 16\" IPS FHD USBC", Price = 2990000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/assets/img/Products/3/Asus/58.jpg", CategoryId = 3 },
        new Product { ProductId = 59, ProductName = "Màn hình ASUS ProArt PA278QV 27\" IPS 2K 75Hz chuyên đồ họa", Price = 7690000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/assets/img/Products/3/Asus/59.jpg", CategoryId = 3 },
        new Product { ProductId = 60, ProductName = "Màn hình ASUS ProArt PA27JCV 27\" IPS 5K USBC chuyên đồ họa", Price = 20500000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/assets/img/Products/3/Asus/60.jpg", CategoryId = 3 },
        new Product { ProductId = 61, ProductName = "Màn hình ASUS VZ249HG 24\" IPS 120Hz viền mỏng", Price = 2350000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/Asus/61.jpg", CategoryId = 3 },
        new Product { ProductId = 62, ProductName = "Màn hình cong Asus ROG Swift PG39WCDM 39\" WOLED 2K 240Hz USBC chuyên game", Price = 40990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/Asus/62.jpg", CategoryId = 3 },
        //Dell
        new Product { ProductId = 63, ProductName = "Màn hình Dell Pro Plus P2725D 27\" IPS 2K 100Hz", Price = 6890000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/assets/img/Products/3/Dell/63.jpg", CategoryId = 3 },
        new Product { ProductId = 64, ProductName = "Màn hình Dell UltraSharp U2725QE 27\" IPS 4K 120Hz USBC chuyên đồ họa", Price = 16490000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/assets/img/Products/3/Dell/64.jpg", CategoryId = 3 },
        new Product { ProductId = 65, ProductName = "Màn hình Dell UltraSharp U2424HE 24\" IPS 120Hz USBC", Price = 6490000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/assets/img/Products/3/Dell/65.jpg", CategoryId = 3 },
        new Product { ProductId = 66, ProductName = "Màn hình cong Dell Alienware AW3423DW 34\" QD-OLED 2K 175Hz G-Sync Ultimate", Price = 23790000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/Dell/66.jpg", CategoryId = 3 },
        new Product { ProductId = 67, ProductName = "Màn hình Dell UltraSharp U4323QE 43\" IPS 4K chuyên đồ họa", Price = 23990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/Dell/67.jpg", CategoryId = 3 },
        //Gigabyte
        new Product { ProductId = 68, ProductName = "Màn hình GIGABYTE G25F2 25\" IPS 200Hz chuyên game", Price = 2990000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/assets/img/Products/3/Gigabyte/68.jpg", CategoryId = 3 },
        new Product { ProductId = 69, ProductName = "Màn hình GIGABYTE MO27Q28G 27\" WOLED 2K 280Hz chuyên game", Price = 16490000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/assets/img/Products/3/Gigabyte/69.jpg", CategoryId = 3 },
        new Product { ProductId = 70, ProductName = "Màn hình GIGABYTE FO32U2P 32\" OLED 4K 240Hz chuyên game", Price = 33990000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/assets/img/Products/3/Gigabyte/70.jpg", CategoryId = 3 },
        new Product { ProductId = 71, ProductName = "Màn hình GIGABYTE FO27Q5P 27\" OLED 2K 500Hz chuyên game", Price = 33990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/Gigabyte/71.jpg", CategoryId = 3 },
        new Product { ProductId = 72, ProductName = "Màn hình cong GIGABYTE G34WQC2 34\" 2K 200Hz chuyên game", Price = 7590000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/Gigabyte/72.jpg", CategoryId = 3 },
        //HKC
        new Product { ProductId = 73, ProductName = "Màn hình cong HKC MG34H18Q 34\" 2K 165Hz USBC", Price = 6690000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/assets/img/Products/3/HKC/73.jpg", CategoryId = 3 },
        new Product { ProductId = 74, ProductName = "Màn hình HKC MB24V9-U 24\" IPS 100Hz", Price = 1790000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/assets/img/Products/3/HKC/74.jpg", CategoryId = 3 },
        new Product { ProductId = 75, ProductName = "Màn hình HKC MG27H7F 27\" Fast IPS 165Hz Gsync", Price = 3090000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/assets/img/Products/3/HKC/75.jpg", CategoryId = 3 },
        new Product { ProductId = 76, ProductName = "Màn hình HKC MG27S9QS 27\" Fast IPS 2K 155Hz Gsync", Price = 4890000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/HKC/76.jpg", CategoryId = 3 },
        new Product { ProductId = 77, ProductName = "Màn hình HKC M27A9X-W Black 75Hz", Price = 2190000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/HKC/77.jpg", CategoryId = 3 },
        //LG
        new Product { ProductId = 78, ProductName = "Màn hình LG 27US500-W Ultrafine 27\" IPS 4K HDR10", Price = 5390000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/assets/img/Products/3/LG/78.jpg", CategoryId = 3 },
        new Product { ProductId = 79, ProductName = "Màn hình LG 22U401A-B 22\" 100Hz HDR10", Price = 2090000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/assets/img/Products/3/LG/79.jpg", CategoryId = 3 },
        new Product { ProductId = 80, ProductName = "Màn hình LG 27GR75Q-B UltraGear 27\" IPS 2K 165Hz Gsync chuyên game", Price = 6090000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/assets/img/Products/3/LG/80.jpg", CategoryId = 3 },
        new Product { ProductId = 81, ProductName = "Màn hình LG 27U411A-B 27\" IPS 120Hz HDR10 siêu mỏng", Price = 2790000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/LG/81.jpg", CategoryId = 3 },
        new Product { ProductId = 82, ProductName = "Màn hình LG 27U631A-B 27\" IPS 2K 100Hz HDR10", Price = 4690000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/LG/82.jpg", CategoryId = 3 },
        //MSI
        new Product { ProductId = 83, ProductName = "Màn hình cong MSI MAG 275CF X24 27\" 240Hz chuyên game", Price = 3490000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/assets/img/Products/3/MSI/83.jpg", CategoryId = 3 },
        new Product { ProductId = 84, ProductName = "Màn hình MSI MAG 272F X24 27\" Rapid IPS 240Hz chuyên game", Price = 3990000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/assets/img/Products/3/MSI/84.jpg", CategoryId = 3 },
        new Product { ProductId = 85, ProductName = "Màn hình MSI MPG 321URXW QD-OLED 32\" QD-LED 4K 240Hz chuyên game", Price = 30590000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/assets/img/Products/3/MSI/85.jpg", CategoryId = 3 },
        new Product { ProductId = 86, ProductName = "Màn hình MSI MAG 255XF 25\" Rapid IPS 300Hz FreeSync Premium chuyên game", Price = 4290000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/MSI/86.jpg", CategoryId = 3 },
        new Product { ProductId = 87, ProductName = "Màn hình MSI PRO MP251 E2 25\" IPS 120Hz", Price = 2190000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/MSI/87.jpg", CategoryId = 3 },
        //SamSung
        new Product { ProductId = 88, ProductName = "Màn hình cong Samsung LC34G55 34\" 2K 165Hz", Price = 7450000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/assets/img/Products/3/SamSung/88.jpg", CategoryId = 3 },
        new Product { ProductId = 89, ProductName = "Màn hình cong Samsung Odyssey G9 LS49CG934 49\" OLED 2K 240Hz", Price = 28490000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/assets/img/Products/3/SamSung/89.jpg", CategoryId = 3 },
        new Product { ProductId = 90, ProductName = "Màn hình Samsung Odyssey G8 LS32FG812SEXXV 32\" OLED 4K 240Hz", Price = 30990000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/assets/img/Products/3/SamSung/90.jpg", CategoryId = 3 },
        new Product { ProductId = 91, ProductName = "Màn hình Samsung Odyssey 3D LS27FG900XEXXV 27\" IPS 4K 165Hz", Price = 36990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/SamSung/91.jpg", CategoryId = 3 },
        new Product { ProductId = 92, ProductName = "Màn hình cong Samsung Odyssey G9 LS49DG930 49\" OLED 2K 240Hz USBC", Price = 36890000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/SamSung/92.jpg", CategoryId = 3 },
        //ViewSonic
        new Product { ProductId = 93, ProductName = "Màn hình ViewSonic VX2480-2K-SHD-2 24\" IPS 2K 100Hz", Price = 3890000, ProductDetail = "Màn hình gaming tốc độ cao, 300Hz, độ phân giải 2K, hỗ trợ G-Sync.", StockQuantity = 15, ImageUrl = "/assets/img/Products/3/ViewSonic/93.jpg", CategoryId = 3 },
        new Product { ProductId = 94, ProductName = "Màn hình ViewSonic ColorPro VP2456A 24\" IPS 120Hz USBC chuyên đồ hoạ", Price = 5290000, ProductDetail = "Màn hình siêu rộng, hỗ trợ HDR10, phù hợp làm việc đa nhiệm.", StockQuantity = 20, ImageUrl = "/assets/img/Products/3/ViewSonic/94.jpg", CategoryId = 3 },
        new Product { ProductId = 95, ProductName = "Màn hình Viewsonic VA2432-H 24\" IPS 100Hz viền mỏng", Price = 1990000, ProductDetail = "Chuẩn màu cao, độ chính xác Delta E < 2, dành cho thiết kế đồ họa.", StockQuantity = 8, ImageUrl = "/assets/img/Products/3/ViewSonic/95.jpg", CategoryId = 3 },
        new Product { ProductId = 96, ProductName = "Màn hình Viewsonic VA2432-H-2 24\" IPS 100Hz viền mỏng", Price = 1990000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/ViewSonic/96.jpg", CategoryId = 3 },
        new Product { ProductId = 97, ProductName = "Màn hình ViewSonic VX2779-HD-PRO 27\" IPS 180Hz chuyên game", Price = 3290000, ProductDetail = "Màn hình OLED 4K 240Hz, màu sắc rực rỡ, độ tương phản tuyệt vời.", StockQuantity = 10, ImageUrl = "/assets/img/Products/3/ViewSonic/97.jpg", CategoryId = 3 },

        // Loa
        new Product { ProductId = 98, ProductName = "Loa Edifier Bluetooth ES20 Black", Price = 890000, ProductDetail = "Âm thanh cân bằng, phù hợp nghe nhạc và làm việc tại nhà.", StockQuantity = 18, ImageUrl = "/assets/img/Products/4/Edifier/98.jpg", CategoryId = 4 },
        new Product { ProductId = 99, ProductName = "Loa Edifier Bluetooth ES300 Ivory", Price = 41900000, ProductDetail = "Loa gaming RGB đồng bộ ánh sáng theo trò chơi.", StockQuantity = 25, ImageUrl = "/assets/img/Products/4/Edifier/99.jpg", CategoryId = 4 },
        new Product { ProductId = 100, ProductName = "Loa máy tính Edifier MR4 White", Price = 1790000, ProductDetail = "Thiết kế độc đáo, âm thanh vòm 3D chân thực.", StockQuantity = 7, ImageUrl = "/assets/img/Products/4/Edifier/100.jpg", CategoryId = 4 },
        new Product { ProductId = 101, ProductName = "Loa máy tính Edifier MR4", Price = 1790000, ProductDetail = "Loa nhỏ gọn, công suất ổn, giá rẻ phù hợp học sinh sinh viên.", StockQuantity = 30, ImageUrl = "/assets/img/Products/4/Edifier/101.jpg", CategoryId = 4 },
        new Product { ProductId = 102, ProductName = "Loa Edifier Bluetooth ES20 Ivory", Price = 940000, ProductDetail = "Âm thanh cân bằng, phù hợp nghe nhạc và làm việc tại nhà.", StockQuantity = 28, ImageUrl = "/assets/img/Products/4/Edifier/102.jpg", CategoryId = 4 },
        new Product { ProductId = 103, ProductName = "Loa Logitech G560", Price = 3890000, ProductDetail = "Thiết kế độc đáo, âm thanh vòm 3D chân thực.", StockQuantity = 26, ImageUrl = "/assets/img/Products/4/Logitech/103.jpg", CategoryId = 4 },
        new Product { ProductId = 104, ProductName = "Loa Razer Leviathan V2", Price = 5790000, ProductDetail = "Loa nhỏ gọn, công suất ổn, giá rẻ phù hợp học sinh sinh viên.", StockQuantity = 17, ImageUrl = "/assets/img/Products/4/Razer/104.jpg", CategoryId = 4 },
        new Product { ProductId = 105, ProductName = "Loa Razer Leviathan V2 X", Price = 2490000, ProductDetail = "Loa gaming RGB đồng bộ ánh sáng theo trò chơi.", StockQuantity = 14, ImageUrl = "/assets/img/Products/4/Razer/105.jpg", CategoryId = 4 },
        new Product { ProductId = 106, ProductName = "Loa Razer Nommo V2 X", Price = 3490000, ProductDetail = "Loa nhỏ gọn, công suất ổn, giá rẻ phù hợp học sinh sinh viên.", StockQuantity = 24, ImageUrl = "/assets/img/Products/4/Razer/106.jpg", CategoryId = 4 },
        new Product { ProductId = 107, ProductName = "Loa Razer Nommo V2", Price = 6100000, ProductDetail = "Thiết kế độc đáo, âm thanh vòm 3D chân thực.", StockQuantity = 23, ImageUrl = "/assets/img/Products/4/Razer/107.jpg", CategoryId = 4 },
        new Product { ProductId = 108, ProductName = "Loa SoundMax SB201 Grey", Price = 490000, ProductDetail = "Thiết kế độc đáo, âm thanh vòm 3D chân thực.", StockQuantity = 13, ImageUrl = "/assets/img/Products/4/SoundMax/108.jpg", CategoryId = 4 },

        // Chuột
        new Product { ProductId = 109, ProductName = "Chuột Razer DeathAdder Essential White", Price = 410000, ProductDetail = "Chuột gaming huyền thoại, cảm biến chính xác, thiết kế công thái học.", StockQuantity = 25, ImageUrl = "/assets/img/Products/5/109.jpg", CategoryId = 5 },
        new Product { ProductId = 110, ProductName = "Chuột Logitech G102 Lightsync White", Price = 415000, ProductDetail = "Chuột gaming phổ biến, đèn RGB, độ bền cao.", StockQuantity = 40, ImageUrl = "/assets/img/Products/5/110.jpg", CategoryId = 5 },
        new Product { ProductId = 111, ProductName = "Chuột không dây Logitech G304 Lightspeed", Price = 750000, ProductDetail = "Chuột không dây hiệu năng cao, pin lâu, trọng lượng nhẹ.", StockQuantity = 35, ImageUrl = "/assets/img/Products/5/111.jpg", CategoryId = 5 },
        new Product { ProductId = 112, ProductName = "Chuột gaming ASUS ROG Strix Impact III", Price = 990000, ProductDetail = "Chuột chuyên game, tốc độ phản hồi nhanh, cảm giác bấm tốt.", StockQuantity = 12, ImageUrl = "/assets/img/Products/5/112.jpg", CategoryId = 5 }
        );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
