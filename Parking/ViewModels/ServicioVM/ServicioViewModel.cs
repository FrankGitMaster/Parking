using Parking.Models;

namespace Parking.ViewModels.ServicioVM
{
    public class ServicioViewModel
    {
        public int Id { get; set; }
        public string Placa { get => _placa; set => _placa = value.ToUpper(); }
        public string Color { get; set; } = string.Empty;
        public string TipoVehiculo { get; set; } = string.Empty;
        public string Observacion { get; set; } = string.Empty;
        public string Espacio { get; set; } = string.Empty;
        public string UsuarioCreacion { get; set; } = string.Empty;
        public string FechaHoraIngreso { get; set; } = string.Empty;
        public string FechaHoraSalida { get; set; } = string.Empty;
        public string TotalMinutos { get; set; } = string.Empty;
        public string ValorTotal { get; set; } = string.Empty;
        public bool TarifaPlena { get; set; }
        public string Estado { get => _estado switch
        {
            "A" => "Activo",
            "F" => "Finalizado",
            _ => "Sin especificar"
        }; set => _estado = value; }
        private string _placa = string.Empty;
        private string _estado = string.Empty;
    }
}
