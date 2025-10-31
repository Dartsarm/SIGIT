using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIGIT.Models;

public partial class Aplicacione
{
    public int AplicacionId { get; set; }

    [Display(Name = "Nombre Aplicación")]
    public string NombreAplicacion { get; set; } = null!;

    public virtual ICollection<CuentasEmpleado> CuentasEmpleados { get; set; } = new List<CuentasEmpleado>();
}


