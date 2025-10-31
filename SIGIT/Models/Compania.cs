using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIGIT.Models;

public partial class Compania
{
    [Display(Name = "Compañía ID")]
    public int CompaniaId { get; set; }

    [Display(Name = "Nombre Compañía")]
    public string NombreCompania { get; set; } = null!;

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
