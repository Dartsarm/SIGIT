using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIGIT.Models;

public partial class Empleado
{
    public int EmpleadoId { get; set; }

    [Display(Name = "Cédula")]
    [Required(ErrorMessage = "El campo Cédula es requerido.")]
    [StringLength(10, ErrorMessage = "La Cédula no puede tener más de 10 caracteres.")]
    public string Cedula { get; set; } = null!;

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El campo Nombre es requerido.")]
    public string Nombre { get; set; } = null!;

    [Display(Name = "Segundo Nombre")]
    public string? SegundoNombre { get; set; }

    [Display(Name = "Apellido")]
    [Required(ErrorMessage = "El campo Apellido es requerido.")]
    public string Apellido { get; set; } = null!;

    [Display(Name = "Segundo Apellido")]
    public string? SegundoApellido { get; set; }

    [Display(Name = "Celular")]
    [Required(ErrorMessage = "El campo Celular es requerido.")]
    public string Celular { get; set; } = null!;

    [Display(Name = "Correo Personal")]
    [Required(ErrorMessage = "El campo Correo Personal es requerido.")]
    [EmailAddress(ErrorMessage = "Debe ser un correo válido.")]
    public string CorreoPersonal { get; set; } = null!;

    [Display(Name = "Cargo")]
    [Required(ErrorMessage = "Debe seleccionar un Cargo.")]
    public int CargoId { get; set; }

    [Display(Name = "Área")]
    [Required(ErrorMessage = "Debe seleccionar un Área.")]
    public int AreaId { get; set; }

    [Display(Name = "Ciudad")]
    [Required(ErrorMessage = "Debe seleccionar una Ciudad.")]
    public int CiudadId { get; set; }

    [Display(Name = "Compañía")]
    [Required(ErrorMessage = "Debe seleccionar una Compañía.")]
    public int CompaniaId { get; set; }

    [Display(Name = "Estatus")]
    [Required(ErrorMessage = "Debe seleccionar un Estatus.")]
    public int EstatusId { get; set; }

    [Display(Name = "Fecha de Ingreso")]
    [Required(ErrorMessage = "El campo Fecha de Ingreso es requerido.")]
    public DateOnly FechaIngreso { get; set; }

    [Display(Name = "Fecha de Retiro")]
    public DateOnly? FechaRetiro { get; set; }

    [Display(Name = "Fecha de registro")]
    public DateTime FechaRegistro { get; set; }

    // --- RELACIONES (Estas no se tocan) ---
    public virtual Area Area { get; set; } = null!;
    public virtual Cargo Cargo { get; set; } = null!;
    public virtual Ciudade Ciudad { get; set; } = null!;
    public virtual Compania Compania { get; set; } = null!;
    public virtual ICollection<CuentasEmpleado> CuentasEmpleados { get; set; } = new List<CuentasEmpleado>();
    public virtual Estatus Estatus { get; set; } = null!;
}