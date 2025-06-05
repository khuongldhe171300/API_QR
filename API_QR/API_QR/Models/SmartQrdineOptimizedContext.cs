using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API_QR.Models;

public partial class SmartQrdineOptimizedContext : DbContext
{
    public SmartQrdineOptimizedContext()
    {
    }

    public SmartQrdineOptimizedContext(DbContextOptions<SmartQrdineOptimizedContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BlogCategory> BlogCategories { get; set; }

    public virtual DbSet<BlogPost> BlogPosts { get; set; }

    public virtual DbSet<BlogTag> BlogTags { get; set; }

    public virtual DbSet<MenuCategory> MenuCategories { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<MenuItemImage> MenuItemImages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Restaurant> Restaurants { get; set; }

    public virtual DbSet<RestaurantStaff> RestaurantStaffs { get; set; }

    public virtual DbSet<RestaurantTable> RestaurantTables { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<SupportTicket> SupportTickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwActiveRestaurant> VwActiveRestaurants { get; set; }

    public virtual DbSet<VwRestaurantMenuWithReview> VwRestaurantMenuWithReviews { get; set; }

    public virtual DbSet<VwRestaurantPerformance> VwRestaurantPerformances { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server =DESKTOP-K0615KK\\KHUONGLDHE; database = SmartQRDineOptimized;uid=sa;pwd=123; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlogCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__BlogCate__19093A2BDEC1420B");

            entity.HasIndex(e => e.Name, "UQ__BlogCate__737584F65ECD8991").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__BlogCate__BC7B5FB6F2C927E2").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Slug).HasMaxLength(100);
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__BlogPost__AA1260384A9E9406");

            entity.HasIndex(e => e.AuthorId, "IX_BlogPosts_AuthorID");

            entity.HasIndex(e => e.IsPublished, "IX_BlogPosts_IsPublished");

            entity.HasIndex(e => e.PublishedAt, "IX_BlogPosts_PublishedAt");

            entity.HasIndex(e => e.Slug, "IX_BlogPosts_Slug");

            entity.HasIndex(e => e.Slug, "UQ__BlogPost__BC7B5FB6A35C4523").IsUnique();

            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.CategoryIds).HasColumnName("CategoryIDs");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Excerpt).HasMaxLength(500);
            entity.Property(e => e.FeaturedImageUrl)
                .HasMaxLength(255)
                .HasColumnName("FeaturedImageURL");
            entity.Property(e => e.IsPublished).HasDefaultValue(false);
            entity.Property(e => e.PublishedAt).HasColumnType("datetime");
            entity.Property(e => e.Slug).HasMaxLength(255);
            entity.Property(e => e.TagIds).HasColumnName("TagIDs");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.Author).WithMany(p => p.BlogPosts)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__BlogPosts__Autho__3587F3E0");
        });

        modelBuilder.Entity<BlogTag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__BlogTags__657CFA4C4336D824");

            entity.HasIndex(e => e.Name, "UQ__BlogTags__737584F65A0FA49D").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__BlogTags__BC7B5FB64B7BD975").IsUnique();

            entity.Property(e => e.TagId).HasColumnName("TagID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Slug).HasMaxLength(50);
        });

        modelBuilder.Entity<MenuCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__MenuCate__19093A2B2886434C");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.MenuCategories)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MenuCateg__Resta__656C112C");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__MenuItem__727E83EB898918EF");

            entity.HasIndex(e => e.CategoryId, "IX_MenuItems_CategoryID");

            entity.HasIndex(e => e.IsAvailable, "IX_MenuItems_IsAvailable");

            entity.HasIndex(e => e.IsFeatured, "IX_MenuItems_IsFeatured");

            entity.HasIndex(e => e.Price, "IX_MenuItems_Price");

            entity.HasIndex(e => e.RestaurantId, "IX_MenuItems_RestaurantID");

            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);
            entity.Property(e => e.IsGlutenFree).HasDefaultValue(false);
            entity.Property(e => e.IsSpicy).HasDefaultValue(false);
            entity.Property(e => e.IsVegan).HasDefaultValue(false);
            entity.Property(e => e.IsVegetarian).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__MenuItems__Categ__6D0D32F4");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MenuItems__Resta__6C190EBB");
        });

        modelBuilder.Entity<MenuItemImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__MenuItem__7516F4EC65587272");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");

            entity.HasOne(d => d.Item).WithMany(p => p.MenuItemImages)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MenuItemI__ItemI__787EE5A0");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E3258C99B67");

            entity.HasIndex(e => e.IsRead, "IX_Notifications_IsRead");

            entity.HasIndex(e => e.RestaurantId, "IX_Notifications_RestaurantID");

            entity.HasIndex(e => e.Type, "IX_Notifications_Type");

            entity.HasIndex(e => e.UserId, "IX_Notifications_UserID");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.IsTicketResponse).HasDefaultValue(false);
            entity.Property(e => e.RedirectUrl)
                .HasMaxLength(255)
                .HasColumnName("RedirectURL");
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.TicketId).HasColumnName("TicketID");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.RestaurantId)
                .HasConstraintName("FK__Notificat__Resta__45BE5BA9");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK__Notificat__Ticke__46B27FE2");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__44CA3770");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFA9E9E999");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_NotifyNewOrder");
                    tb.HasTrigger("trg_TrackOrderStatusChanges");
                });

            entity.HasIndex(e => e.CreatedAt, "IX_Orders_CreatedAt");

            entity.HasIndex(e => e.CustomerPhone, "IX_Orders_CustomerPhone");

            entity.HasIndex(e => e.PaymentStatus, "IX_Orders_PaymentStatus");

            entity.HasIndex(e => e.RestaurantId, "IX_Orders_RestaurantID");

            entity.HasIndex(e => e.Status, "IX_Orders_Status");

            entity.HasIndex(e => e.TableId, "IX_Orders_TableID");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerEmail).HasMaxLength(100);
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(20);
            entity.Property(e => e.Discount)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NetAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OrderType).HasMaxLength(50);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.ServiceCharge)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TableId).HasColumnName("TableID");
            entity.Property(e => e.Tax)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Orders)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__Restaura__03F0984C");

            entity.HasOne(d => d.Table).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TableId)
                .HasConstraintName("FK__Orders__TableID__04E4BC85");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06A179E6DFB1");

            entity.HasIndex(e => e.ItemId, "IX_OrderItems_ItemID");

            entity.HasIndex(e => e.OrderId, "IX_OrderItems_OrderID");

            entity.HasIndex(e => e.Status, "IX_OrderItems_Status");

            entity.Property(e => e.OrderItemId).HasColumnName("OrderItemID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Item).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__ItemI__0E6E26BF");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Order__0D7A0286");
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__OrderSta__4D7B4ADDF8BFE320");

            entity.ToTable("OrderStatusHistory");

            entity.Property(e => e.HistoryId).HasColumnName("HistoryID");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NewStatus).HasMaxLength(50);
            entity.Property(e => e.OldStatus).HasMaxLength(50);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.ChangedBy)
                .HasConstraintName("FK__OrderStat__Chang__1CBC4616");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderStat__Order__1BC821DD");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A58082265A0");

            entity.HasIndex(e => e.OrderId, "IX_Payments_OrderID");

            entity.HasIndex(e => e.PaymentDate, "IX_Payments_PaymentDate");

            entity.HasIndex(e => e.PaymentStatus, "IX_Payments_PaymentStatus");

            entity.HasIndex(e => e.PaymentType, "IX_Payments_PaymentType");

            entity.HasIndex(e => e.SubscriptionId, "IX_Payments_SubscriptionID");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValue("VND");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.PaymentType).HasMaxLength(20);
            entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(255)
                .HasColumnName("TransactionID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Payments__OrderI__14270015");

            entity.HasOne(d => d.Subscription).WithMany(p => p.Payments)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK__Payments__Subscr__151B244E");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.RestaurantId).HasName("PK__Restaura__87454CB5701306EA");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateRestaurantTimestamp"));

            entity.HasIndex(e => e.City, "IX_Restaurants_City");

            entity.HasIndex(e => e.IsActive, "IX_Restaurants_IsActive");

            entity.HasIndex(e => e.OwnerUserId, "IX_Restaurants_OwnerUserID");

            entity.HasIndex(e => e.PlanId, "IX_Restaurants_PlanID");

            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.AllowSpecialInstructions).HasDefaultValue(true);
            entity.Property(e => e.AutoAcceptOrders).HasDefaultValue(false);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CoverImageUrl)
                .HasMaxLength(255)
                .HasColumnName("CoverImageURL");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValue("VND");
            entity.Property(e => e.DisplayPricesWithTax).HasDefaultValue(true);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EstimatedPrepTime).HasDefaultValue(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Language)
                .HasMaxLength(10)
                .HasDefaultValue("vi");
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(255)
                .HasColumnName("LogoURL");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.OrderNumberPrefix)
                .HasMaxLength(10)
                .HasDefaultValue("ORD");
            entity.Property(e => e.OwnerUserId).HasColumnName("OwnerUserID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.RequireCustomerInfo).HasDefaultValue(false);
            entity.Property(e => e.ServiceChargeRate)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.TaxRate)
                .HasDefaultValue(10.00m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Website).HasMaxLength(255);

            entity.HasOne(d => d.OwnerUser).WithMany(p => p.Restaurants)
                .HasForeignKey(d => d.OwnerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Restauran__Owner__47DBAE45");
        });

        modelBuilder.Entity<RestaurantStaff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Restaura__96D4AAF7FF1BD6DB");

            entity.ToTable("RestaurantStaff");

            entity.HasIndex(e => new { e.RestaurantId, e.UserId }, "UQ_RestaurantStaff_RestaurantUser").IsUnique();

            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsManager).HasDefaultValue(false);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.RestaurantStaffs)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Restauran__Resta__5FB337D6");

            entity.HasOne(d => d.User).WithMany(p => p.RestaurantStaffs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Restauran__UserI__60A75C0F");
        });

        modelBuilder.Entity<RestaurantTable>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("PK__Restaura__7D5F018EB0375969");

            entity.HasIndex(e => e.RestaurantId, "IX_RestaurantTables_RestaurantID");

            entity.HasIndex(e => e.Status, "IX_RestaurantTables_Status");

            entity.HasIndex(e => new { e.RestaurantId, e.TableNumber }, "UQ_RestaurantTable_Number").IsUnique();

            entity.Property(e => e.TableId).HasColumnName("TableID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.QrcodeData)
                .HasMaxLength(255)
                .HasColumnName("QRCodeData");
            entity.Property(e => e.QrcodeUrl)
                .HasMaxLength(255)
                .HasColumnName("QRCodeURL");
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Available");
            entity.Property(e => e.TableNumber).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.RestaurantTables)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Restauran__Resta__7E37BEF6");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AE4774ED44");

            entity.HasIndex(e => e.ItemId, "IX_Reviews_ItemID");

            entity.HasIndex(e => e.Rating, "IX_Reviews_Rating");

            entity.HasIndex(e => e.RestaurantId, "IX_Reviews_RestaurantID");

            entity.HasIndex(e => e.ReviewType, "IX_Reviews_ReviewType");

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.ReviewType).HasMaxLength(20);

            entity.HasOne(d => d.Item).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK__Reviews__ItemID__2180FB33");

            entity.HasOne(d => d.Order).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Reviews__OrderID__22751F6C");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__Restaur__208CD6FA");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AA03A4114");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160B2A84C8C").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__Subscrip__9A2B24BDB15F2E03");

            entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");
            entity.Property(e => e.AutoRenew).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Plan).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__PlanI__59063A47");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__Resta__5812160E");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__Subscrip__755C22D7B63ABBEA");

            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.BillingCycle).HasMaxLength(20);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PlanName).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__SupportT__712CC627324A4EB9");

            entity.HasIndex(e => e.Priority, "IX_SupportTickets_Priority");

            entity.HasIndex(e => e.RestaurantId, "IX_SupportTickets_RestaurantID");

            entity.HasIndex(e => e.Status, "IX_SupportTickets_Status");

            entity.HasIndex(e => e.UserId, "IX_SupportTickets_UserID");

            entity.Property(e => e.TicketId).HasColumnName("TicketID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Priority)
                .HasMaxLength(50)
                .HasDefaultValue("Medium");
            entity.Property(e => e.ResolvedAt).HasColumnType("datetime");
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Open");
            entity.Property(e => e.Subject).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.SupportTicketAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__SupportTi__Assig__40058253");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.SupportTickets)
                .HasForeignKey(d => d.RestaurantId)
                .HasConstraintName("FK__SupportTi__Resta__3D2915A8");

            entity.HasOne(d => d.User).WithMany(p => p.SupportTicketUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__SupportTi__UserI__3C34F16F");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACCC73AC08");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.IsActive, "IX_Users_IsActive");

            entity.HasIndex(e => e.RoleId, "IX_Users_RoleID");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534D44EC2CF").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EmailVerified).HasDefaultValue(false);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.ResetPasswordExpires).HasColumnType("datetime");
            entity.Property(e => e.ResetPasswordToken).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.VerificationToken).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleID__3C69FB99");
        });

        modelBuilder.Entity<VwActiveRestaurant>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ActiveRestaurants");

            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(255)
                .HasColumnName("LogoURL");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.OwnerEmail).HasMaxLength(100);
            entity.Property(e => e.OwnerName).HasMaxLength(101);
            entity.Property(e => e.OwnerPhone).HasMaxLength(20);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.PlanName).HasMaxLength(100);
            entity.Property(e => e.PlanPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.SubscriptionStatus).HasMaxLength(50);
        });

        modelBuilder.Entity<VwRestaurantMenuWithReview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_RestaurantMenuWithReviews");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.DiscountPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ItemName).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.RestaurantName).HasMaxLength(100);
        });

        modelBuilder.Entity<VwRestaurantPerformance>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_RestaurantPerformance");

            entity.Property(e => e.AverageOrderValue).HasColumnType("decimal(38, 6)");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.JoinedDate).HasColumnType("datetime");
            entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
            entity.Property(e => e.RestaurantName).HasMaxLength(100);
            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(38, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
