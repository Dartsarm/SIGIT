using System;
using System.Collections.Generic;

namespace SIGIT.Models;

public partial class UsuariosSistema
{
    public int UsuarioSistemaId { get; set; }

    public string Cedula { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? SegundoNombre { get; set; }

    public string Apellido { get; set; } = null!;

    public string? SegundoApellido { get; set; }

    public string UsuarioLogin { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Celular { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RolId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual Role Rol { get; set; } = null!;
}
