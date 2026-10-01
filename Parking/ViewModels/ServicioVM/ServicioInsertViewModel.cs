using Parking.Models;
using System.ComponentModel.DataAnnotations;

namespace Parking.ViewModels.ServicioVM
{
    public class ServicioInsertViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "La placa es requerida")]
        [MaxLength(10, ErrorMessage = "La placa ingresada excede el máximo de 10 caracteres")]
        public string Placa { get => _placa; set => _placa = value.ToUpper(); }
        [Required(ErrorMessage = "El color es requerido")]
        [MaxLength(20, ErrorMessage = "El color ingresado excede el máximo de 20 caracteres")]
        public string Color { get; set; } = string.Empty;
        public int IdTipoVehiculo { get; set; }
        [MaxLength(50, ErrorMessage = "La observación ingresada excede el máximo de 50 caracteres")]
        public string? Observacion { get; set; }
        public string Espacio { get; set; } = string.Empty;
        public bool TarifaPlena { get; set; }
        private string _placa = string.Empty;
    }
}
