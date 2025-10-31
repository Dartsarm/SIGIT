using System;
using System.Collections.Generic;

namespace SIGIT.Models;

public partial class Role
{
    public int RolId { get; set; }

    public string NombreRol { get; set; } = null!;

    public virtual ICollection<UsuariosSistema> UsuariosSistemas { get; set; } = new List<UsuariosSistema>();
}
