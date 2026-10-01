using Parking.Models;

namespace Parking.DTOs
{
    public class ServicioReporteDTO
    {
        public string IdServicio { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string TipoVehiculo { get; set; } = string.Empty;
        public string Observacion { get; set; } = string.Empty;
        public string Espacio { get; set; } = string.Empty;
        public string FechaHoraIngreso { get; set; } = string.Empty;
        public string FechaHoraSalida { get; set; } = string.Empty;
        public string TotalMinutos { get; set; } = string.Empty;
        public string ValorTotal { get; set; } = string.Empty;
        public string TarifaPlena { get; set; } = string.Empty;
        public string FechaCreacion { get; set; } = string.Empty;
        public string FechaActualizacion { get; set; } = string.Empty;
        public string UsuarioCreacion { get; set; } = string.Empty;
        public string UsuarioActualizacion { get; set; } = string.Empty;
        public string UsuarioFinalizacion { get; set; } = string.Empty;
    }
}