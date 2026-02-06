using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TherapuHubAPI.Models;

public partial class ContextDB : DbContext
{
    public ContextDB()
    {
    }

    public ContextDB(DbContextOptions<ContextDB> options)
        : base(options)
    {
    }

    public virtual DbSet<ClientStatuses> ClientStatuses { get; set; }

    public virtual DbSet<Clients> Clients { get; set; }

    public virtual DbSet<Companies> Companies { get; set; }

    public virtual DbSet<EventRecurrence> EventRecurrence { get; set; }

    public virtual DbSet<EventTypes> EventTypes { get; set; }

    public virtual DbSet<EventUsers> EventUsers { get; set; }

    public virtual DbSet<Events> Events { get; set; }

    public virtual DbSet<JobTitles> JobTitles { get; set; }

    public virtual DbSet<Menus> Menus { get; set; }

    public virtual DbSet<Programs> Programs { get; set; }

    public virtual DbSet<Reminders> Reminders { get; set; }

    public virtual DbSet<UserTypeMenus> UserTypeMenus { get; set; }

    public virtual DbSet<UserTypes> UserTypes { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:sql-therapyhub-dev-001.database.windows.net,1433;Database=THERAPYHUB;User ID=adminsql;Password=SqlDev#2026!;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClientStatuses>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClientSt__3214EC075CFFB672");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Clients>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clients__3214EC07F54175F4");

            entity.Property(e => e.ClientCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.GuardianName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Companies>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Companie__3214EC073FCE0C3C");

            entity.Property(e => e.CreatedAt).HasPrecision(0);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TaxId)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EventRecurrence>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EventRec__3214EC072261DBBA");

            entity.Property(e => e.EndDate).HasPrecision(0);
            entity.Property(e => e.Interval).HasDefaultValue(1);
            entity.Property(e => e.RecurrenceType)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EventTypes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EventTyp__3214EC07121FFA43");

            entity.Property(e => e.Color)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EventUsers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EventUse__3214EC076C6278E5");

            entity.HasIndex(e => new { e.EventId, e.UserId }, "UQ_EventUsers").IsUnique();
        });

        modelBuilder.Entity<Events>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Events__3214EC07F11D4114");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.EndDate).HasPrecision(0);
            entity.Property(e => e.StartDate).HasPrecision(0);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<JobTitles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobTitle__3214EC076C5254CF");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Menus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Menus__3214EC07846E3A0D");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Route).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Programs>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Programs__3214EC074FF65D75");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Reminders>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reminder__3214EC07D9338AD8");

            entity.Property(e => e.Channel)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<UserTypeMenus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserType__3214EC07707BCDD4");

            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<UserTypes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserType__3214EC07C9522850");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0777CCE6C5");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
