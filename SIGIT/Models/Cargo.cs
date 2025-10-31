using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIGIT.Models;

public partial class Cargo
{
    [Display(Name = "Cargo ID")]
    public int CargoId { get; set; }

    [Display(Name = "Nombre cargo")]
    public string NombreCargo { get; set; } = null!;

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
