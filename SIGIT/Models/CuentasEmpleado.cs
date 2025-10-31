using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIGIT.Models;

public partial class CuentasEmpleado
{
    [Display (Name = "Cuenta ID")]
    public int CuentaId { get; set; }

    [Display(Name = "Empleado")]
    public int EmpleadoId { get; set; }

    [Display(Name = "Aplicación")]
    public int AplicacionId { get; set; }

    [Display(Name = "Usuario")]
    public string Usuario { get; set; } = null!;

    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = null!;
    
    [Display(Name = "Fecha de creación")]
    public DateTime FechaCreacion { get; set; }

    [Display(Name = "Última modificación")]
    public DateTime UltimaModificacion { get; set; }

    public virtual Aplicacione Aplicacion { get; set; } = null!;

    public virtual Empleado Empleado { get; set; } = null!;
}
