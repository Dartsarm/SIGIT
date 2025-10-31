using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIGIT.Models;

public partial class Estatus
{
    [Display(Name = "Estatus ID")]
    public int EstatusId { get; set; }

    [Display(Name = "Estado")]
    public string NombreEstatus { get; set; } = null!;

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
