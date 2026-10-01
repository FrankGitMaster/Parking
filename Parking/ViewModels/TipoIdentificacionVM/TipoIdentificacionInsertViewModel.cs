using System.ComponentModel.DataAnnotations;

namespace Parking.ViewModels.TipoIdentificacionVM
{
    public class TipoIdentificacionInsertViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El tipo de identificación es requerido")]
        [MaxLength(50, ErrorMessage = "El tipo de identificación ingresado excede el máximo de 50 caracteres")]
        public string Tipo { get; set; } = string.Empty;
        [Required(ErrorMessage = "La sigla es requerida")]
        [MaxLength(3, ErrorMessage = "La sigla ingresada excede el máximo de 3 caracteres")]
        public string Sigla { get => _sigla; set => _sigla = value.ToUpper(); }
        public string Estado { get; set; } = "A";
        private string _sigla = string.Empty;
    }
}
