using System.ComponentModel.DataAnnotations;

namespace Parking.ViewModels.EspacioVM
{
    public class EspacioInsertViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El Número del Espacio es requerido")]
        [MaxLength(5, ErrorMessage = "El Espacio ingresado excede la cantidad máxima de caracteres")]
        public string Numero { get => _numero; set => _numero = value.ToUpper(); }
        [Required(ErrorMessage = "El Tipo de Vehículo es requerido")]
        public int IdTipoVehiculo { get; set; }
        public string Estado { get; set; } = "L";
        public bool EstadoInactivo { get; set; } = false;
        public string IdUsuarioActualizacion { get; set; } = string.Empty;
        private string _numero = string.Empty;
    }
}
