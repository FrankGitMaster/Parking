using Parking.Atributos;
using System.ComponentModel.DataAnnotations;

namespace Parking.ViewModels.CuentaVM
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El {0} es requerido")]
        [Display(Name = "Nombre Completo")]
        [StringLength(maximumLength: 100, ErrorMessage = "El {0} excede el máximo de 100 caracteres")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "El {0} ingresado no es válido")]
        public string Nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "El {0} es requerido")]
        [EmailAddress(ErrorMessage = "El {0} ingresado no es válido")]
        public string Correo { get; set; } = string.Empty;
        [Required(ErrorMessage = "La {0} es requerida")]
        [Display(Name = "Contraseña")]
        [StringLength(maximumLength: 16, MinimumLength = 8, ErrorMessage = "La {0} debe ser un valor entre 8 y 16 caracteres")]
        [Compare("ConfirmarContrasena", ErrorMessage = "Las {0}s no coinciden")]
        [RequiereMinuscula]
        [RequiereMayuscula]
        [RequiereNumero]
        [RequiereCaracterEspecial]
        public string Contrasena { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} es requerido")]
        [Display(Name = "Confirmar la Contraseña")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
