namespace Parking.Models
{
    public class TipoVehiculo
    {
        public int? Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int IdUsuarioActualizacion { get; set; }
        public ICollection<Espacio> Espacios { get; set; } = new List<Espacio>();
        public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
    }
}
