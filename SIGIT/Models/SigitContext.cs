using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SIGIT.ViewModels;

namespace SIGIT.Models;

public partial class SigitContext : DbContext
{
    public SigitContext()
    {
    }

    public SigitContext(DbContextOptions<SigitContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aplicacione> Aplicaciones { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<Cargo> Cargos { get; set; }

    public virtual DbSet<Ciudade> Ciudades { get; set; }

    public virtual DbSet<Compania> Companias { get; set; }

    public virtual DbSet<CuentasEmpleado> CuentasEmpleados { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<Estatus> Estatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<UsuariosSistema> UsuariosSistemas { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=11D-MngSystDS;Database=SIGIT;Trusted_Connection=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aplicacione>(entity =>
        {
            entity.HasKey(e => e.AplicacionId).HasName("PK__Aplicaci__5A5CD10980B91640");

            entity.HasIndex(e => e.NombreAplicacion, "UQ__Aplicaci__031682C3097DAE1D").IsUnique();

            entity.Property(e => e.AplicacionId).HasColumnName("AplicacionID");
            entity.Property(e => e.NombreAplicacion).HasMaxLength(100);
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.AreaId).HasName("PK__Areas__70B8202885F85A32");

            entity.HasIndex(e => e.NombreArea, "UQ__Areas__D5E8EEB5E1D85042").IsUnique();

            entity.Property(e => e.AreaId).HasColumnName("AreaID");
            entity.Property(e => e.NombreArea).HasMaxLength(100);
        });

        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.HasKey(e => e.CargoId).HasName("PK__Cargos__B4E665EDB4D1B86B");

            entity.HasIndex(e => e.NombreCargo, "UQ__Cargos__B281D7B5ADDC12C9").IsUnique();

            entity.Property(e => e.CargoId).HasColumnName("CargoID");
            entity.Property(e => e.NombreCargo).HasMaxLength(100);
        });

        modelBuilder.Entity<Ciudade>(entity =>
        {
            entity.HasKey(e => e.CiudadId).HasName("PK__Ciudades__E826E790D34864E2");

            entity.HasIndex(e => e.NombreCiudad, "UQ__Ciudades__FCF2550273D2E35B").IsUnique();

            entity.Property(e => e.CiudadId).HasColumnName("CiudadID");
            entity.Property(e => e.NombreCiudad).HasMaxLength(100);
        });

        modelBuilder.Entity<Compania>(entity =>
        {
            entity.HasKey(e => e.CompaniaId).HasName("PK__Compania__DE6CF4D3B3CC345B");

            entity.HasIndex(e => e.NombreCompania, "UQ__Compania__53ED17FE238B4825").IsUnique();

            entity.Property(e => e.CompaniaId).HasColumnName("CompaniaID");
            entity.Property(e => e.NombreCompania).HasMaxLength(100);
        });

        modelBuilder.Entity<CuentasEmpleado>(entity =>
        {
            entity.HasKey(e => e.CuentaId).HasName("PK__CuentasE__40072EA1EC0FB756");

            entity.ToTable("CuentasEmpleado");

            entity.HasIndex(e => new { e.EmpleadoId, e.AplicacionId }, "UQ_Empleado_Aplicacion").IsUnique();

            entity.Property(e => e.CuentaId).HasColumnName("CuentaID");
            entity.Property(e => e.AplicacionId).HasColumnName("AplicacionID");
            entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UltimaModificacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Usuario).HasMaxLength(100);

            entity.HasOne(d => d.Aplicacion).WithMany(p => p.CuentasEmpleados)
                .HasForeignKey(d => d.AplicacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CuentasEmpleado_Aplicaciones");

            entity.HasOne(d => d.Empleado).WithMany(p => p.CuentasEmpleados)
                .HasForeignKey(d => d.EmpleadoId)
                .HasConstraintName("FK_CuentasEmpleado_Empleados");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.EmpleadoId).HasName("PK__Empleado__958BE6F0A688B9F9");

            entity.HasIndex(e => e.Celular, "UQ__Empleado__0E9B6C3BE2433A42").IsUnique();

            entity.HasIndex(e => e.CorreoPersonal, "UQ__Empleado__8E93C53C6A14290C").IsUnique();

            entity.HasIndex(e => e.Cedula, "UQ__Empleado__B4ADFE38F9E846AE").IsUnique();

            entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
            entity.Property(e => e.Apellido).HasMaxLength(50);
            entity.Property(e => e.AreaId).HasColumnName("AreaID");
            entity.Property(e => e.CargoId).HasColumnName("CargoID");
            entity.Property(e => e.Cedula).HasMaxLength(10);
            entity.Property(e => e.Celular).HasMaxLength(20);
            entity.Property(e => e.CiudadId).HasColumnName("CiudadID");
            entity.Property(e => e.CompaniaId).HasColumnName("CompaniaID");
            entity.Property(e => e.CorreoPersonal).HasMaxLength(100);
            entity.Property(e => e.EstatusId).HasColumnName("EstatusID");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.SegundoApellido).HasMaxLength(50);
            entity.Property(e => e.SegundoNombre).HasMaxLength(50);

            entity.HasOne(d => d.Area).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleados_Areas");

            entity.HasOne(d => d.Cargo).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.CargoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleados_Cargos");

            entity.HasOne(d => d.Ciudad).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.CiudadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleados_Ciudades");

            entity.HasOne(d => d.Compania).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.CompaniaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleados_Companias");

            entity.HasOne(d => d.Estatus).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.EstatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleados_Estatus");
        });

        modelBuilder.Entity<Estatus>(entity =>
        {
            entity.HasKey(e => e.EstatusId).HasName("PK__Estatus__DE10F26D6C47296E");

            entity.ToTable("Estatus");

            entity.HasIndex(e => e.NombreEstatus, "UQ__Estatus__12CF232C14BC02B0").IsUnique();

            entity.Property(e => e.EstatusId).HasColumnName("EstatusID");
            entity.Property(e => e.NombreEstatus).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__Roles__F92302D11BF51CDB");

            entity.HasIndex(e => e.NombreRol, "UQ__Roles__4F0B537F19312C52").IsUnique();

            entity.Property(e => e.RolId).HasColumnName("RolID");
            entity.Property(e => e.NombreRol).HasMaxLength(50);
        });

        modelBuilder.Entity<UsuariosSistema>(entity =>
        {
            entity.HasKey(e => e.UsuarioSistemaId).HasName("PK__Usuarios__AC322F41C9428A20");

            entity.ToTable("UsuariosSistema");

            entity.HasIndex(e => e.Celular, "UQ__Usuarios__0E9B6C3B0E991D42").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D10534AC22EDC3").IsUnique();

            entity.HasIndex(e => e.Cedula, "UQ__Usuarios__B4ADFE3886406809").IsUnique();

            entity.HasIndex(e => e.UsuarioLogin, "UQ__Usuarios__F96234F30F616A93").IsUnique();

            entity.Property(e => e.UsuarioSistemaId).HasColumnName("UsuarioSistemaID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Apellido).HasMaxLength(50);
            entity.Property(e => e.Cedula).HasMaxLength(10);
            entity.Property(e => e.Celular).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.RolId).HasColumnName("RolID");
            entity.Property(e => e.SegundoApellido).HasMaxLength(50);
            entity.Property(e => e.SegundoNombre).HasMaxLength(50);
            entity.Property(e => e.UsuarioLogin).HasMaxLength(100);

            entity.HasOne(d => d.Rol).WithMany(p => p.UsuariosSistemas)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsuariosSistema_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<SIGIT.ViewModels.EmpleadoListadoViewModel> EmpleadoListadoViewModel { get; set; } = default!;
}
