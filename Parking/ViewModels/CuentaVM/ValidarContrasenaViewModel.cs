using System.ComponentModel.DataAnnotations;

namespace Parking.ViewModels.CuentaVM
{
    public class ValidarContrasenaViewModel
    {
        [Required(ErrorMessage = "El {0} es requerido")]
        [EmailAddress(ErrorMessage = "El {0} ingresado no es válido")]
        public string Usuario { get; set; }
    }
}
