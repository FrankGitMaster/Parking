namespace Parking.Models
{
    public class Servicio
    {
        public int Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int IdTipoVehiculo { get; set; }
        public string? Observacion { get; set; }
        public string EspacioNumero { get; set; } = string.Empty;
        public DateTime FechaHoraIngreso { get; set; }
        public DateTime? FechaHoraSalida { get; set; }
        public int TotalMinutos { get; set; }
        public decimal ValorTotal { get; set; }
        public bool TarifaPlena { get; set; }
        public string? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string IdUsuarioCreacion { get; set; } = string.Empty;
        public string? IdUsuarioActualizacion { get; set; }
        public string? IdUsuarioFinalizacion { get; set; }
        public TipoVehiculo? TipoVehiculoNavigation { get; set; }
        public Espacio? EspacioNavigation { get; set; }
        public Usuario? UsuarioCreacionNavigation { get; set; }
        public Usuario? UsuarioActualizacionNavigation { get; set; }
        public Usuario? UsuarioFinalizacionNavigation { get; set; }
    }
}
