using Cukcuk.Core.Auth;
using Cukcuk.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cukcuk.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<CustomerGroup> CustomerGroups { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Import> Imports { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<UserPermission> UserPermissions { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<UserFile> Files { get; set; }
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

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DepartmentId).HasName("PRIMARY");
                entity.ToTable("department");
                entity.Property(e => e.DepartmentName).HasMaxLength(45);
                entity.Property(e => e.DepartmentCode).HasMaxLength(45);
                entity.Property(e => e.CreatedBy).HasMaxLength(45);
                entity.Property(e => e.ModifiedBy).HasMaxLength(45);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(e => e.PositionId).HasName("PRIMARY");
                entity.ToTable("position");
                entity.Property(e => e.PositionName).HasMaxLength(45);
                entity.Property(e => e.PositionCode).HasMaxLength(45);
                entity.Property(e => e.CreatedBy).HasMaxLength(45);
                entity.Property(e => e.ModifiedBy).HasMaxLength(45);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });
            modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("menu");
                entity.Property(e => e.MenuName).HasMaxLength(45);
                entity.Property(e => e.MenuPath).HasMaxLength(45);
                entity.Property(e => e.MenuIcon).HasMaxLength(45);
            });
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.PermissionId).HasName("PRIMARY");
                entity.ToTable("permission");
                entity.Property(e => e.PermissionName).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(255);
            });

            modelBuilder.Entity<UserPermission>(entity =>
            {
                entity.ToTable("user_permission");
                entity.HasKey(e => new { e.UserId, e.PermissionId });
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Permission).WithMany(g => g.UserPermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .HasConstraintName("FK_user_permission_PermissionId");

                entity.HasOne(d => d.User).WithMany(g => g.UserPermissions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_user_permission_UserId");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.MessageId).HasName("PRIMARY");
                entity.ToTable("message");

                entity.Property(e => e.Content).HasMaxLength(2500);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.HasIndex(e => e.SenderId, "FK_message_SenderId_idx");
                entity.HasIndex(e => e.ReceiverId, "FK_message_RecivertId_idx");

                entity.HasOne(e => e.Sender)
                    .WithMany(s => s.SenderMessages)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK_message_SenderId");

                entity.HasOne(e => e.Receiver)
                    .WithMany(s => s.ReceiverMessages)
                    .HasForeignKey(d => d.ReceiverId)
                    .HasConstraintName("FK_message_ReceiverId");

            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("folder");

                entity.Property(e => e.FolderName).HasMaxLength(255);
                entity.Property(e => e.FolderPath).HasMaxLength(255);

                entity.HasIndex(e => e.MenuId, "FK_Folder_MenuId_idx");

                entity.HasOne(e => e.Menu)
                    .WithMany(m => m.Folders)
                    .HasForeignKey(e => e.MenuId)
                    .HasConstraintName("FK_Folder_MenuId");

                entity.HasOne(e => e.Parent)
                    .WithMany(p => p.SubFolders)
                    .HasForeignKey(e => e.ParentId)
                    .HasConstraintName("FK_Folder_ParentId");
            });

            modelBuilder.Entity<UserFile>(entity =>
            {
                entity.HasKey(e => e.FileId).HasName("PRIMARY");
                entity.ToTable("user_file");

                entity.Property(e => e.FileName).HasMaxLength(255);
                entity.Property(e => e.FilePath).HasMaxLength(255);

                entity.HasIndex(e => e.FolderId, "FK_File_FolderId_idx");

                entity.HasOne(e => e.Folder)
                    .WithMany(m => m.Files)
                    .HasForeignKey(e => e.FolderId)
                    .HasConstraintName("FK_File_FolderId");
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
