using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Parking.Atributos
{
    public class RequiereMinuscula : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string contrasena && !string.IsNullOrEmpty(contrasena))
            {
                if (!Regex.IsMatch(contrasena, @"[a-z]"))
                    return new ValidationResult($"La {validationContext.DisplayName} debe contener al menos una minúscula");
            }
            return ValidationResult.Success;
        }
    }

    public class RequiereMayuscula : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string contrasena && !string.IsNullOrEmpty(contrasena))
            {
                if (!Regex.IsMatch(contrasena, @"[A-Z]"))
                    return new ValidationResult($"La {validationContext.DisplayName} debe contener al menos una mayúscula");
            }
            return ValidationResult.Success;
        }
    }

    public class RequiereNumero : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value is string contrasena && !string.IsNullOrEmpty(contrasena))
            {
                if(!Regex.IsMatch(contrasena, @"[\d]"))
                {
                    return new ValidationResult($"La {validationContext} debe contener al menos un número");
                }
            }
            return ValidationResult.Success;
        }
    }

    public class RequiereCaracterEspecial : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value is string contrasena && !string.IsNullOrEmpty(contrasena))
            {
                if (!Regex.IsMatch(contrasena, @"[-+*_?&#$!%]"))
                    return new ValidationResult($"La {validationContext.DisplayName} debe contener al menos un caracter especial (ej: -+*_?&#$!%)");
            }
            return ValidationResult.Success;
        }
    }
}
