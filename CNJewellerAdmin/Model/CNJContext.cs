using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CNJewellerAdmin.Model
{
    public partial class CNJContext : DbContext
    {
        public CNJContext()
        {
        }

        public CNJContext(DbContextOptions<CNJContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DriveFile> DriveFiles { get; set; } = null!;
        public virtual DbSet<ShareDatum> ShareData { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-T8V3JVM\\SQLEXPRESS;Database=CNJ;User Id=sa;Password=ikart@123;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DriveFile>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExpiryTime).HasColumnType("datetime");

                entity.Property(e => e.Mobile).HasMaxLength(10);

                entity.Property(e => e.SharedGuid).HasColumnName("SharedGUID");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);
            });

            modelBuilder.Entity<ShareDatum>(entity =>
            {
                entity.Property(e => e.LocalFilePath).HasMaxLength(1000);

                entity.Property(e => e.Mimetype)
                    .HasMaxLength(1000)
                    .HasColumnName("MIMEType");

                entity.Property(e => e.Name).HasMaxLength(1000);

                entity.Property(e => e.SharedData).HasMaxLength(1000);

                entity.Property(e => e.SharedGuid).HasColumnName("SharedGUID");

                entity.Property(e => e.ThumbnailLink).HasMaxLength(1000);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
