using Microsoft.AspNetCore.Identity;

namespace Parking.Models
{
    public class Espacio
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public int IdTipoVehiculo { get; set; }
        public int? IdServicioActivo { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? IdUsuarioActualizacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public TipoVehiculo? TipoVehiculoNavigation { get; set; }
        public Servicio? ServicioNavigation { get; set; }
        public Usuario? UsuarioNavigation { get; set; }
    }
}
