using System.ComponentModel.DataAnnotations;

namespace SIGIT.ViewModels
{
    public class EmpleadoListadoViewModel
    {

        public int Id { get; set; }

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = null!;
        public string Celular { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string Cargo { get; set; } = null!;
        public string Ciudad { get; set; } = null!;
        public string Estatus { get; set; } = null!;


    }
}
