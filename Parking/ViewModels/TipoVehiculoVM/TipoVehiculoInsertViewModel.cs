using System.ComponentModel.DataAnnotations;

namespace Parking.ViewModels.TipoVehiculo
{
    public class TipoVehiculoInsertViewModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El tipo del vehículo es requerido")]
        [MaxLength(50, ErrorMessage = "El tipo de vehículo excede la cantidad máxima de 50 caracteres")]
        public string Tipo { get; set; } = string.Empty;
        public string Estado { get; set; } = "A";
    }
}
