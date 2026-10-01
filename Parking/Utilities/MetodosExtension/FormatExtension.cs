namespace Parking.Utilities.MetodosExtension
{
    public static class FormatExtension
    {

        public static string FormatearNombreArchivo(this string tituloReporte, string extension)
        {
            string nombreBase = tituloReporte.ToLower()
                .Replace(" ", "_")
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u")
                .Replace("ñ", "n")
                .Replace(":", "")
                .Replace(";", "")
                .Replace(",", "")
                .Replace(".", "");
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"{nombreBase}_{timestamp}.{extension}";
        }

    }
}
