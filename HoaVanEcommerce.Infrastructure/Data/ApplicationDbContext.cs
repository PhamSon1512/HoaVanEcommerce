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

            entity.Property(u => u.Id)
                .HasColumnName("id");

            entity.Property(u => u.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(512)
                .IsRequired();

            entity.Property(u => u.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(255);

            entity.Property(u => u.PhoneNumber)
                .HasColumnName("phone_number")
                .HasMaxLength(50);

            entity.Property(u => u.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            entity.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(u => u.Email)
                .IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.HasKey(r => r.Id);

            entity.Property(r => r.Id)
                .HasColumnName("id");

            entity.Property(r => r.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();
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
            entity.Property(c => c.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(c => c.Slug).HasColumnName("slug").HasMaxLength(255);
            entity.Property(c => c.Description).HasColumnName("description");
            entity.Property(c => c.ParentCategoryId).HasColumnName("parent_category_id");
            entity.Property(c => c.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(c => c.ParentCategory)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.CategoryId).HasColumnName("category_id");
            entity.Property(p => p.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(p => p.Code).HasColumnName("code").HasMaxLength(100);
            entity.Property(p => p.Description).HasColumnName("description");
            entity.Property(p => p.Price).HasColumnName("price").HasColumnType("decimal(18,2)");
            entity.Property(p => p.ImageUrl).HasColumnName("image_url").HasMaxLength(500).IsRequired();
            entity.Property(p => p.StockQuantity).HasColumnName("stock_quantity");
            entity.Property(p => p.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(p => p.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.ToTable("carts");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.UserId).HasColumnName("user_id");
            entity.Property(c => c.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");
            entity.Property(c => c.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("cart_items");

            entity.HasKey(ci => ci.Id);

            entity.Property(ci => ci.Id).HasColumnName("id");
            entity.Property(ci => ci.CartId).HasColumnName("cart_id");
            entity.Property(ci => ci.ProductId).HasColumnName("product_id");
            entity.Property(ci => ci.Quantity).HasColumnName("quantity");
            entity.Property(ci => ci.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(18,2)");

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
            entity.Property(o => o.UserId).HasColumnName("user_id");
            entity.Property(o => o.OrderCode).HasColumnName("order_code").HasMaxLength(50).IsRequired();
            entity.Property(o => o.TotalAmount).HasColumnName("total_amount").HasColumnType("decimal(18,2)");
            entity.Property(o => o.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
            entity.Property(o => o.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");
            entity.Property(o => o.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_items");

            entity.HasKey(oi => oi.Id);

            entity.Property(oi => oi.Id).HasColumnName("id");
            entity.Property(oi => oi.OrderId).HasColumnName("order_id");
            entity.Property(oi => oi.ProductId).HasColumnName("product_id");
            entity.Property(oi => oi.Quantity).HasColumnName("quantity");
            entity.Property(oi => oi.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(18,2)");
            entity.Property(oi => oi.TotalPrice).HasColumnName("total_price").HasColumnType("decimal(18,2)");

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
            entity.Property(a => a.UserId).HasColumnName("user_id");
            entity.Property(a => a.FullName).HasColumnName("full_name").HasMaxLength(255).IsRequired();
            entity.Property(a => a.PhoneNumber).HasColumnName("phone_number").HasMaxLength(50).IsRequired();
            entity.Property(a => a.AddressLine).HasColumnName("address_line").HasMaxLength(500).IsRequired();
            entity.Property(a => a.Ward).HasColumnName("ward").HasMaxLength(255);
            entity.Property(a => a.District).HasColumnName("district").HasMaxLength(255);
            entity.Property(a => a.Province).HasColumnName("province").HasMaxLength(255);
            entity.Property(a => a.IsDefault).HasColumnName("is_default");
            entity.Property(a => a.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId);
        });

        modelBuilder.Entity<OrderShippingInfo>(entity =>
        {
            entity.ToTable("order_shipping_info");

            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id).HasColumnName("id");
            entity.Property(s => s.OrderId).HasColumnName("order_id");
            entity.Property(s => s.ReceiverName).HasColumnName("receiver_name").HasMaxLength(255).IsRequired();
            entity.Property(s => s.ReceiverPhone).HasColumnName("receiver_phone").HasMaxLength(50).IsRequired();
            entity.Property(s => s.AddressLine).HasColumnName("address_line").HasMaxLength(500).IsRequired();
            entity.Property(s => s.Ward).HasColumnName("ward").HasMaxLength(255);
            entity.Property(s => s.District).HasColumnName("district").HasMaxLength(255);
            entity.Property(s => s.Province).HasColumnName("province").HasMaxLength(255);
            entity.Property(s => s.ShippingFee).HasColumnName("shipping_fee").HasColumnType("decimal(18,2)");
            entity.Property(s => s.ShipperName).HasColumnName("shipper_name").HasMaxLength(255);
            entity.Property(s => s.TrackingCode).HasColumnName("tracking_code").HasMaxLength(100);
            entity.Property(s => s.EstimatedDeliveryDate).HasColumnName("estimated_delivery_date");

            entity.HasOne(s => s.Order)
                .WithOne(o => o.ShippingInfo)
                .HasForeignKey<OrderShippingInfo>(s => s.OrderId);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.OrderId).HasColumnName("order_id");
            entity.Property(p => p.Amount).HasColumnName("amount").HasColumnType("decimal(18,2)");
            entity.Property(p => p.Method).HasColumnName("method").HasMaxLength(50).IsRequired();
            entity.Property(p => p.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
            entity.Property(p => p.TransactionCode).HasColumnName("transaction_code").HasMaxLength(100);
            entity.Property(p => p.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");
            entity.Property(p => p.PaidAt).HasColumnName("paid_at");

            entity.HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId);
        });

        modelBuilder.Entity<ShopSetting>(entity =>
        {
            entity.ToTable("shop_settings");

            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id).HasColumnName("id");
            entity.Property(s => s.Key).HasColumnName("key").HasMaxLength(100).IsRequired();
            entity.Property(s => s.Value).HasColumnName("value");
            entity.Property(s => s.Description).HasColumnName("description");
            entity.Property(s => s.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.ToTable("order_status_history");

            entity.HasKey(h => h.Id);

            entity.Property(h => h.Id).HasColumnName("id");
            entity.Property(h => h.OrderId).HasColumnName("order_id");
            entity.Property(h => h.OldStatus).HasColumnName("old_status").HasMaxLength(50).IsRequired();
            entity.Property(h => h.NewStatus).HasColumnName("new_status").HasMaxLength(50).IsRequired();
            entity.Property(h => h.Note).HasColumnName("note");
            entity.Property(h => h.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(h => h.Order)
                .WithMany(o => o.StatusHistory)
                .HasForeignKey(h => h.OrderId);
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.ToTable("complaints");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.OrderId).HasColumnName("order_id");
            entity.Property(c => c.UserId).HasColumnName("user_id");
            entity.Property(c => c.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
            entity.Property(c => c.Content).HasColumnName("content").IsRequired();
            entity.Property(c => c.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
            entity.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETUTCDATE()");
            entity.Property(c => c.ResolvedAt).HasColumnName("resolved_at");

            entity.HasOne(c => c.Order)
                .WithMany(o => o.Complaints)
                .HasForeignKey(c => c.OrderId);

            entity.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);
        });
    }
}
