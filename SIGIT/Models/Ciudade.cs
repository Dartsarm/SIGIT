using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIGIT.Models;

public partial class Ciudade
{
    [Display(Name = "Ciudad ID")]
    public int CiudadId { get; set; }

    [Display(Name = "Ciudades")]
    public string NombreCiudad { get; set; } = null!;

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
