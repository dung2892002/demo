using Cukcuk.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace jwtAuth.Auth
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<CustomerGroup> CustomerGroups { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Import> Imports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerGroup>(entity =>
            {
                entity.HasKey(e => e.GroupId).HasName("PRIMARY");
                entity.ToTable("customergroup");
                entity.Property(e => e.GroupName).HasMaxLength(255);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId).HasName("PRIMARY");
                entity.ToTable("customer");
                entity.HasIndex(e => e.GroupId, "fk_customer_GroupId_idx");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
                entity.Property(e => e.CustomerCode).HasMaxLength(45);
                entity.Property(e => e.Fullname).HasMaxLength(45);
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.MobileNumber).HasMaxLength(45);
                entity.Property(e => e.Email).HasMaxLength(45);
                entity.Property(e => e.Amount).HasPrecision(19, 2);

                entity.HasOne(d => d.Group).WithMany(g => g.Customers)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("fk_customer_GroupId");
            });

            modelBuilder.Entity<Import>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("import");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.ColumnName).HasMaxLength(255);
                entity.Property(e => e.PropertyName).HasMaxLength(255);
                entity.Property(e => e.TableName).HasMaxLength(45);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
