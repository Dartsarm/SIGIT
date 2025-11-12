using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIGIT.Models;

public partial class UsuariosSistema
{
    [Display(Name = "ID Usuario")]
    public int UsuarioSistemaId { get; set; }
    [Display(Name = "Cédula")]
    [Required(ErrorMessage = "El campo Cédula es requerido.")]
    [StringLength(10, ErrorMessage = "La Cédula no puede tener más de 10 caracteres.")]
    public string Cedula { get; set; } = null!;

    
    [Required(ErrorMessage = "El campo Nombre es requerido.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;
    
    [Display(Name = "Segundo Nombre")]
    public string? SegundoNombre { get; set; }

    [Required(ErrorMessage = "El campo Apellido es requerido.")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = null!;
    
    [Display(Name = "Segundo Apellido")]
    public string? SegundoApellido { get; set; }

    
    [Required(ErrorMessage = "El campo Correo es requerido.")]
    [EmailAddress(ErrorMessage = "Debe ser un correo válido.")]
    [Display(Name = "Correo")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "El campo Celular es requerido.")]
    [Display(Name = "Celular")]
        public string Celular { get; set; } = null!;

    [Required(ErrorMessage = "El campo usuario login es requerido.")]
    [Display(Name = "Usuario Login")]
    public string UsuarioLogin { get; set; } = null!;

    [Required(ErrorMessage = "El campo contraseña es obligatorio.")]
    [Display(Name = "Contraseña")]
    public string PasswordHash { get; set; } = null!;
    
    [Display(Name = "ID Rol")]
    public int RolId { get; set; }

    [Display(Name = "Estatus")]
    [Required(ErrorMessage = "Debe seleccionar un Estatus.")]
    public bool Activo { get; set; }
    
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; }
    
    [Display(Name = "Rol")]
    public virtual Role Rol { get; set; } = null!;
}
