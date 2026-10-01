namespace Parking.Models
{
    public class Tarifa
    {
        public int Id { get; set; }
        public int IdTipoVehiculo { get; set; }
        public decimal tarifaMinuto { get; set; }
        public decimal tarifaPlena { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int IdUsuarioActualizacion { get; set; }
    }
}
