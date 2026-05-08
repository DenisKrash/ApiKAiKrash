using apikaikrash;
using Microsoft.EntityFrameworkCore;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;

namespace apikaikrash;

public partial class ShoesContext : DbContext
{
    public ShoesContext()
    {
    }

    public ShoesContext(DbContextOptions<ShoesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<PickUpPoint> PickUpPoints { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductOrder> ProductOrders { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=shoes;Username=postgres;Password=root");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.IdManufacturer).HasName("manufacturer_pkey");

            entity.ToTable("manufacturers");

            entity.Property(e => e.IdManufacturer)
                .HasDefaultValueSql("nextval('manufacturer_id_manufacturer_seq'::regclass)")
                .HasColumnName("id_manufacturer");
            entity.Property(e => e.Manufacturer1)
                .HasMaxLength(50)
                .HasColumnName("manufacturer");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.ClientFk).HasColumnName("client_fk");
            entity.Property(e => e.DateDelivery).HasColumnName("date_delivery");
            entity.Property(e => e.DateOrder).HasColumnName("date_order");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(20)
                .HasColumnName("order_status");
            entity.Property(e => e.PickUpPointFk).HasColumnName("pick-up_point_fk");
            entity.Property(e => e.ReceiptCode).HasColumnName("receipt_code");

            entity.HasOne(d => d.ClientFkNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_client_fk_fkey");

            entity.HasOne(d => d.PickUpPointFkNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PickUpPointFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_pick-up_point_fk_fkey");
        });

        modelBuilder.Entity<PickUpPoint>(entity =>
        {
            entity.HasKey(e => e.IdPickUpPoints).HasName("pick-up_points_pkey");

            entity.ToTable("pick-up_points");

            entity.Property(e => e.IdPickUpPoints).HasColumnName("id_pick-up_points");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.HouseNumber).HasColumnName("house_number");
            entity.Property(e => e.Index).HasColumnName("index");
            entity.Property(e => e.Street)
                .HasMaxLength(50)
                .HasColumnName("street");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("products_pkey");

            entity.ToTable("products");

            entity.HasIndex(e => e.Article, "products_article_key").IsUnique();

            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.Article)
                .HasMaxLength(20)
                .HasColumnName("article");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.ManufacturerFk).HasColumnName("manufacturer_fk");
            entity.Property(e => e.NameProduct)
                .HasMaxLength(50)
                .HasColumnName("name_product");
            entity.Property(e => e.Photo)
                .HasColumnType("character varying")
                .HasColumnName("photo");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProductCategoryFk).HasColumnName("product_category_fk");
            entity.Property(e => e.QuantityStock).HasColumnName("quantity_stock");
            entity.Property(e => e.SupplierFk).HasColumnName("supplier_fk");
            entity.Property(e => e.UnitMeasurement)
                .HasMaxLength(20)
                .HasColumnName("unit_measurement");

            entity.HasOne(d => d.ManufacturerFkNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.ManufacturerFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("products_manufacturer_fk_fkey");

            entity.HasOne(d => d.ProductCategoryFkNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductCategoryFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("products_product_category_fk_fkey");

            entity.HasOne(d => d.SupplierFkNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("products_supplier_fk_fkey");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.IdProductCategory).HasName("product_category_pkey");

            entity.ToTable("product_categories");

            entity.Property(e => e.IdProductCategory)
                .HasDefaultValueSql("nextval('product_category_id_product_category_seq'::regclass)")
                .HasColumnName("id_product_category");
            entity.Property(e => e.ProductCategory1)
                .HasMaxLength(50)
                .HasColumnName("product_category");
        });

        modelBuilder.Entity<ProductOrder>(entity =>
        {
            entity.HasKey(e => e.IdProductOrder).HasName("product_orders_pkey");

            entity.ToTable("product_orders");

            entity.Property(e => e.IdProductOrder).HasColumnName("id_product_order");
            entity.Property(e => e.ArticleNumberFk)
                .HasMaxLength(20)
                .HasColumnName("article_number_fk");
            entity.Property(e => e.OrderNumberFk).HasColumnName("order_number_fk");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.ArticleNumberFkNavigation).WithMany(p => p.ProductOrders)
                .HasPrincipalKey(p => p.Article)
                .HasForeignKey(d => d.ArticleNumberFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("product_orders_article_number_fk_fkey");

            entity.HasOne(d => d.OrderNumberFkNavigation).WithMany(p => p.ProductOrders)
                .HasForeignKey(d => d.OrderNumberFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("product_orders_order_number_fk_fkey");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.IdSupplier).HasName("supplier_pkey");

            entity.ToTable("suppliers");

            entity.Property(e => e.IdSupplier)
                .HasDefaultValueSql("nextval('supplier_id_supplier_seq'::regclass)")
                .HasColumnName("id_supplier");
            entity.Property(e => e.Supplier1)
                .HasMaxLength(50)
                .HasColumnName("supplier");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUsers).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.IdUsers).HasColumnName("id_users");
            entity.Property(e => e.EmployeeRole)
                .HasMaxLength(50)
                .HasColumnName("employee_role");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .HasColumnName("surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
