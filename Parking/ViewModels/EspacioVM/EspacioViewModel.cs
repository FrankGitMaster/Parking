using Parking.ViewModels.TipoVehiculo;

namespace Parking.ViewModels.EspacioVM
{
    public class EspacioViewModel
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string TipoVehiculo { get; set; } = string.Empty;
        public string Estado
        {
            get => _estado switch
            {
                "L" => "Libre",
                "O" => "Ocupado",
                "X" => "Inactivo",
                _ => "Sin especificar"
            }; set => _estado = value;
        }
        private string _estado = string.Empty;
    }
}
