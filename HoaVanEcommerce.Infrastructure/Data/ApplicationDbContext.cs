using HoaVanEcommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HoaVanEcommerce.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<OrderShippingInfo> OrderShippingInfos => Set<OrderShippingInfo>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ShopSetting> ShopSettings => Set<ShopSetting>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    public DbSet<Complaint> Complaints => Set<Complaint>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnName("id");

            entity.Property(u => u.Email)
                .HasColumnName("email")
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(512)
                .IsRequired();

            entity.Property(u => u.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(u => u.PhoneNumber)
                .HasColumnName("phone_number")
                .HasMaxLength(50);

            entity.Property(u => u.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            entity.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("sysdatetime()");

            entity.Property(u => u.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("sysdatetime()");

            entity.HasIndex(u => u.Email)
                .IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.HasKey(r => r.Id);

            entity.Property(r => r.Id).HasColumnName("id");

            entity.Property(r => r.Code)
                .HasColumnName("code")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(r => r.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(r => r.Code)
                .IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles");

            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.Property(ur => ur.UserId)
                .HasColumnName("user_id");

            entity.Property(ur => ur.RoleId)
                .HasColumnName("role_id");

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            entity.Property(c => c.Slug).HasColumnName("slug").HasMaxLength(200).IsRequired();
            entity.Property(c => c.Description).HasColumnName("description").HasMaxLength(1000);
            entity.Property(c => c.ThumbnailUrl).HasColumnName("thumbnail_url").HasMaxLength(500);
            entity.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("sysdatetime()");
            entity.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("sysdatetime()");

            entity.HasIndex(c => c.Slug)
                .IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.Name).HasColumnName("name").HasMaxLength(300).IsRequired();
            entity.Property(p => p.Code).HasColumnName("code").HasMaxLength(100).IsRequired();
            entity.Property(p => p.Price).HasColumnName("price").HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(p => p.CategoryId).HasColumnName("category_id").IsRequired();
            entity.Property(p => p.ImageUrl).HasColumnName("image_url").HasMaxLength(500).IsRequired();
            entity.Property(p => p.Description).HasColumnName("description");
            entity.Property(p => p.StockQuantity).HasColumnName("stock_quantity").HasDefaultValue(0);
            entity.Property(p => p.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(p => p.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("sysdatetime()");
            entity.Property(p => p.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("sysdatetime()");

            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            entity.HasIndex(p => p.Code)
                .IsUnique();
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.ToTable("carts");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.UserId).HasColumnName("user_id");
            entity.Property(c => c.Status).HasColumnName("status").HasMaxLength(50).IsRequired().HasDefaultValue("ACTIVE");
            entity.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("sysdatetime()");
            entity.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("sysdatetime()");

            entity.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("cart_items");

            entity.HasKey(ci => ci.Id);

            entity.Property(ci => ci.Id).HasColumnName("id");
            entity.Property(ci => ci.CartId).HasColumnName("cart_id").IsRequired();
            entity.Property(ci => ci.ProductId).HasColumnName("product_id").IsRequired();
            entity.Property(ci => ci.Quantity).HasColumnName("quantity").IsRequired();
            entity.Property(ci => ci.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(18,2)").IsRequired();

            entity.HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId);

            entity.HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");

            entity.HasKey(o => o.Id);

            entity.Property(o => o.Id).HasColumnName("id");
            entity.Property(o => o.OrderCode).HasColumnName("order_code").HasMaxLength(50).IsRequired();
            entity.Property(o => o.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(o => o.TotalAmount).HasColumnName("total_amount").HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(o => o.PaymentStatus).HasColumnName("payment_status").HasMaxLength(50).IsRequired().HasDefaultValue("PENDING");
            entity.Property(o => o.ShippingStatus).HasColumnName("shipping_status").HasMaxLength(50).IsRequired().HasDefaultValue("NOT_SHIPPED");
            entity.Property(o => o.OrderStatus).HasColumnName("order_status").HasMaxLength(50).IsRequired().HasDefaultValue("PENDING_PAYMENT");
            entity.Property(o => o.PaymentMethod).HasColumnName("payment_method").HasMaxLength(50).IsRequired().HasDefaultValue("BANK_TRANSFER_QR");
            entity.Property(o => o.CurrentStatusNote).HasColumnName("current_status_note").HasMaxLength(1000);
            entity.Property(o => o.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("sysdatetime()");
            entity.Property(o => o.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("sysdatetime()");

            entity.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);

            entity.HasIndex(o => o.OrderCode)
                .IsUnique();
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_items");

            entity.HasKey(oi => oi.Id);

            entity.Property(oi => oi.Id).HasColumnName("id");
            entity.Property(oi => oi.OrderId).HasColumnName("order_id").IsRequired();
            entity.Property(oi => oi.ProductId).HasColumnName("product_id").IsRequired();
            entity.Property(oi => oi.ProductName).HasColumnName("product_name").HasMaxLength(300).IsRequired();
            entity.Property(oi => oi.ProductCode).HasColumnName("product_code").HasMaxLength(100).IsRequired();
            entity.Property(oi => oi.Quantity).HasColumnName("quantity").IsRequired();
            entity.Property(oi => oi.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(oi => oi.LineTotal).HasColumnName("line_total").HasColumnType("decimal(18,2)").IsRequired();

            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            entity.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("addresses");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id).HasColumnName("id");
            entity.Property(a => a.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(a => a.FullName).HasColumnName("full_name").HasMaxLength(256).IsRequired();
            entity.Property(a => a.PhoneNumber).HasColumnName("phone_number").HasMaxLength(50).IsRequired();
            entity.Property(a => a.Province).HasColumnName("province").HasMaxLength(100).IsRequired();
            entity.Property(a => a.District).HasColumnName("district").HasMaxLength(100).IsRequired();
            entity.Property(a => a.Ward).HasColumnName("ward").HasMaxLength(100).IsRequired();
            entity.Property(a => a.StreetDetail).HasColumnName("street_detail").HasMaxLength(300).IsRequired();
            entity.Property(a => a.IsDefault).HasColumnName("is_default").HasDefaultValue(false);
            entity.Property(a => a.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("sysdatetime()");
            entity.Property(a => a.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("sysdatetime()");

            entity.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId);
        });

        modelBuilder.Entity<OrderShippingInfo>(entity =>
        {
            entity.ToTable("order_shipping_info");

            entity.HasKey(s => s.OrderId);

            entity.Property(s => s.OrderId).HasColumnName("order_id");
            entity.Property(s => s.ReceiverName).HasColumnName("receiver_name").HasMaxLength(256).IsRequired();
            entity.Property(s => s.ReceiverPhone).HasColumnName("receiver_phone").HasMaxLength(50).IsRequired();
            entity.Property(s => s.FullAddress).HasColumnName("full_address").HasMaxLength(500).IsRequired();
            entity.Property(s => s.Note).HasColumnName("note").HasMaxLength(1000);

            entity.HasOne(s => s.Order)
                .WithOne(o => o.ShippingInfo)
                .HasForeignKey<OrderShippingInfo>(s => s.OrderId);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.OrderId).HasColumnName("order_id").IsRequired();
            entity.Property(p => p.Amount).HasColumnName("amount").HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(p => p.Status).HasColumnName("status").HasMaxLength(50).IsRequired().HasDefaultValue("PENDING_CONFIRM");
            entity.Property(p => p.TransactionRef).HasColumnName("transaction_ref").HasMaxLength(100);
            entity.Property(p => p.PaidAt).HasColumnName("paid_at");
            entity.Property(p => p.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("sysdatetime()");
            entity.Property(p => p.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("sysdatetime()");

            entity.HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId);
        });

        modelBuilder.Entity<ShopSetting>(entity =>
        {
            entity.ToTable("shop_settings");

            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id).HasColumnName("id");
            entity.Property(s => s.BankName).HasColumnName("bank_name").HasMaxLength(200).IsRequired();
            entity.Property(s => s.AccountName).HasColumnName("account_name").HasMaxLength(200).IsRequired();
            entity.Property(s => s.AccountNumber).HasColumnName("account_number").HasMaxLength(100).IsRequired();
            entity.Property(s => s.QrImageUrl).HasColumnName("qr_image_url").HasMaxLength(500).IsRequired();
            entity.Property(s => s.Description).HasColumnName("description").HasMaxLength(1000);
            entity.Property(s => s.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("sysdatetime()");
            entity.Property(s => s.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("sysdatetime()");
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.ToTable("order_status_history");

            entity.HasKey(h => h.Id);

            entity.Property(h => h.Id).HasColumnName("id");
            entity.Property(h => h.OrderId).HasColumnName("order_id").IsRequired();
            entity.Property(h => h.ChangedByUserId).HasColumnName("changed_by_user_id");
            entity.Property(h => h.FromStatus).HasColumnName("from_status").HasMaxLength(50);
            entity.Property(h => h.ToStatus).HasColumnName("to_status").HasMaxLength(50).IsRequired();
            entity.Property(h => h.Note).HasColumnName("note").HasMaxLength(1000);
            entity.Property(h => h.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("sysdatetime()");

            entity.HasOne(h => h.Order)
                .WithMany(o => o.StatusHistory)
                .HasForeignKey(h => h.OrderId);

            entity.HasOne(h => h.ChangedByUser)
                .WithMany()
                .HasForeignKey(h => h.ChangedByUserId);
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.ToTable("complaints");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.OrderId).HasColumnName("order_id").IsRequired();
            entity.Property(c => c.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(c => c.Type).HasColumnName("type").HasMaxLength(50).IsRequired();
            entity.Property(c => c.Description).HasColumnName("description");
            entity.Property(c => c.Status).HasColumnName("status").HasMaxLength(50).IsRequired().HasDefaultValue("OPEN");
            entity.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("sysdatetime()");
            entity.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("sysdatetime()");

            entity.HasOne(c => c.Order)
                .WithMany(o => o.Complaints)
                .HasForeignKey(c => c.OrderId);

            entity.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);
        });
    }
}
