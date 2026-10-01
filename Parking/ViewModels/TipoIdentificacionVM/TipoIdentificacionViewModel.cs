namespace Parking.ViewModels.TipoIdentificacionVM
{
    public class TipoIdentificacionViewModel
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
        public string Estado
        {
            get => _estado switch
            {
                "A" => "Activo",
                "I" => "Inactivo",
                _ => "Estado no reconocido"
            }; set => _estado = value;
        }
        private string _estado = string.Empty;
    }
}
