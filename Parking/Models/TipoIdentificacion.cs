namespace Parking.Models
{
    public class TipoIdentificacion
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int Usuario { get; set; }
    }
}
