using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIGIT.Models;

public partial class UsuariosSistema
{
    [Display(Name = "ID Usuario")]
    public int UsuarioSistemaId { get; set; }
    [Required]
    public string Cedula { get; set; } = null!;

    [Required]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;
    
    [Display(Name = "Segundo Nombre")]
    public string? SegundoNombre { get; set; }

    [Required]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = null!;
    
    [Display(Name = "Segundo Apellido")]
    public string? SegundoApellido { get; set; }
        
    [Required]
    [Display(Name = "Correo")]
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "Celular")]
    public string Celular { get; set; } = null!;

    [Required]
    [Display(Name = "Usuario Login")]
    public string UsuarioLogin { get; set; } = null!;

    [Required]
    [Display(Name = "Contraseña")]
    public string PasswordHash { get; set; } = null!;
    
    [Display(Name = "ID Rol")]
    public int RolId { get; set; }
    
    [Display(Name = "Estatus")]
    public bool Activo { get; set; }
    
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; }
    
    [Display(Name = "Rol")]
    public virtual Role Rol { get; set; } = null!;
}
