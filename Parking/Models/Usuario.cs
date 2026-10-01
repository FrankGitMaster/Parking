using Microsoft.AspNetCore.Identity;

namespace Parking.Models
{
    public class Usuario : IdentityUser
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public ICollection<Espacio> Espacios { get; set; } = new List<Espacio>();
        public ICollection<Servicio> ServiciosCreados { get; set; } = new List<Servicio>();
        public ICollection<Servicio> ServiciosActualizados { get; set; } = new List<Servicio>();
        public ICollection<Servicio> ServiciosFinalizados { get; set; } = new List<Servicio>();
    }
}
