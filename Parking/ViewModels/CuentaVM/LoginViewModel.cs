using System.ComponentModel.DataAnnotations;

namespace Parking.ViewModels.CuentaVM
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El {0} es requerido")]
        [EmailAddress(ErrorMessage = "El {0} ingresado no es válido")]
        public string Usuario { get; set; }
        [Required(ErrorMessage = "La {0} es requerida")]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }
        public bool RecordarContrasena { get; set; }
    }
}
