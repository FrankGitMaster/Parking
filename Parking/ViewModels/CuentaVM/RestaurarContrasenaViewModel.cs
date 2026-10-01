using Parking.Atributos;
using System.ComponentModel.DataAnnotations;

namespace Parking.ViewModels.CuentaVM
{
    public class RestaurarContrasenaViewModel
    {
        public string Usuario { get; set; }
        [Required(ErrorMessage = "La {0} es requerida")]
        [Display(Name = "Contraseña")]
        [StringLength(maximumLength: 16, MinimumLength = 8, ErrorMessage = "La {0} debe ser un valor entre 8 y 16 caracteres")]
        [Compare("ConfirmarContrasena", ErrorMessage = "Las {0}s no coinciden")]
        [RequiereMinuscula]
        [RequiereMayuscula]
        [RequiereNumero]
        [RequiereCaracterEspecial]
        public string NuevaContrasena { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} es requerido")]
        [Display(Name = "Confirmar la Contraseña")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}
