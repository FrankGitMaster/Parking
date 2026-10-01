using Parking.Models;

namespace Parking.ViewModels.TipoVehiculo
{
    public class TipoVehiculoViewModel
    {
        public int? Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Estado
        {
            get
            {
                return _estado switch
                {
                    "A" => "Activo",
                    "I" => "Inactivo"
                };
            }
            set => _estado = value;
        }
        private string _estado = "A";
    }
}
