using System;
using System.Collections.Generic;
using Hair_saloon.Models;
using Microsoft.EntityFrameworkCore;

namespace Hair_saloon.Data;

public partial class HairSaloonContext : DbContext
{
    public HairSaloonContext()
    {
    }

    public HairSaloonContext(DbContextOptions<HairSaloonContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<TypeOfUser> TypeOfUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=LAPTOP-GGB50FF3\\SQLEXPRESS; Initial Catalog=HairSaloon;Integrated Security=True;Connect Timeout=30; Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite; Multi Subnet Failover=False;Database=HairSaloon;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__31384C2905B918F0");

            entity.ToTable("Reservation");

            entity.HasIndex(e => e.ReservationDate, "UQ__Reservat__C5D482FED6526375").IsUnique();

            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.Payed).HasColumnName("payed");
            entity.Property(e => e.ReservationDate).HasColumnName("reservation_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reservati__user___3E52440B");
        });

        modelBuilder.Entity<TypeOfUser>(entity =>
        {
            entity.HasKey(e => e.IdTypeOfUser).HasName("PK__Type_Of___3213E83F7FBD87CA");

            entity.ToTable("Type_Of_User");

            entity.Property(e => e.IdTypeOfUser).HasColumnName("id_type_of_user");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.RoleOfUser)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("role_of_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__B9BE370FB67F6234");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ__User__F3DBC572D0E8F579").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.TypeOfUserId).HasColumnName("type_of_user_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.TypeOfUser).WithMany(p => p.Users)
                .HasForeignKey(d => d.TypeOfUserId)
                .HasConstraintName("FK__User__type_of_us__3A81B327");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
