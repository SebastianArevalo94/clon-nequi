using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ClonNequi2.Models;

public partial class ClonNequiContext : DbContext
{
    public ClonNequiContext()
    {
    }

    public ClonNequiContext(DbContextOptions<ClonNequiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Cuenta> Cuenta { get; set; }

    public virtual DbSet<Movimiento> Movimientos { get; set; }

    public virtual DbSet<TipoMovimiento> TipoMovimientos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cliente__3214EC07E71938AD");

            entity.ToTable("Cliente");

            entity.Property(e => e.Apellidos)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Contrasenia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Documento)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FechaNacimiento).HasColumnName("Fecha_Nacimiento");
            entity.Property(e => e.Nombres)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Numero)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cuenta__3214EC0753A31D64");

            //entity.HasOne(d => d.Cliente).WithMany(p => p.Cuenta)
            //    .HasForeignKey(d => d.ClienteId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK__Cuenta__ClienteI__38996AB5");
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Movimien__3214EC07A641B0C9");

            entity.Property(e => e.Fecha).HasColumnType("datetime");

            //entity.HasOne(d => d.Cliente).WithMany(p => p.Movimientos)
            //    .HasForeignKey(d => d.ClienteId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK__Movimient__Clien__3D5E1FD2");

            entity.HasOne(d => d.TipoMovimientoNavigation).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.TipoMovimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Movimient__TipoM__3E52440B");
        });

        modelBuilder.Entity<TipoMovimiento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoMovi__3214EC07DD19510E");

            entity.ToTable("TipoMovimiento");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
