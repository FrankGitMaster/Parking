using Microsoft.AspNetCore.Identity;

namespace Parking.ViewModels.CuentaVM
{
    public class UsuarioViewModel
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
