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
        public virtual DbSet<OfficeMaster> OfficeMasters { get; set; } = null!;
        public virtual DbSet<ShareDatum> ShareData { get; set; } = null!;
        public virtual DbSet<UserDetail> UserDetails { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=LAPTOP-EKSUU5RT\\HASHTAGIKART;Database=CNJ;User Id=sa; password=ikart@3689");
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

            modelBuilder.Entity<OfficeMaster>(entity =>
            {
                entity.ToTable("OfficeMaster");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.BranchCode).HasMaxLength(100);

                entity.Property(e => e.City).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.OfficeName).HasMaxLength(100);

                entity.Property(e => e.PhoneNo).HasMaxLength(12);

                entity.Property(e => e.Pincode).HasMaxLength(10);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
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

            modelBuilder.Entity<UserDetail>(entity =>
            {
                entity.ToTable("UserDetail");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailId).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.JoiningDate).HasColumnType("datetime");

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.MiddleName).HasMaxLength(50);

                entity.Property(e => e.MobileNo).HasMaxLength(12);

                entity.Property(e => e.Pincode).HasMaxLength(10);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
