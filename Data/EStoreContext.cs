using Microsoft.EntityFrameworkCore;
using prakt15_TRPO.Models;

namespace prakt15_TRPO.Data;

public partial class EStoreContext : DbContext
{
    public EStoreContext()
    {
    }

    public EStoreContext(DbContextOptions<EStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<ProductTag> ProductTags { get; set; } 

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=EStoreDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.BrandId, "IX_Products_BrandId");
            entity.HasIndex(e => e.CategoryId, "IX_Products_CategoryId");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 2)");
            entity.HasOne(d => d.Brand).WithMany(p => p.Products).HasForeignKey(d => d.BrandId);
            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasForeignKey(d => d.CategoryId);

            entity.HasMany(d => d.Tags)
                  .WithMany(p => p.Products)
                  .UsingEntity<ProductTag>(
                      j => j.HasOne(pt => pt.Tag).WithMany().HasForeignKey(pt => pt.TagId),
                      j => j.HasOne(pt => pt.Product).WithMany().HasForeignKey(pt => pt.ProductId),
                      j =>
                      {
                          j.HasKey(t => new { t.ProductId, t.TagId });
                          j.ToTable("ProductTags");
                      });
        });

        modelBuilder.Entity<ProductTag>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.TagId });
            entity.HasIndex(e => e.TagId, "IX_ProductTags_TagId");

            entity.HasOne(d => d.Product)
                  .WithMany()
                  .HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.Tag)
                  .WithMany()
                  .HasForeignKey(d => d.TagId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}