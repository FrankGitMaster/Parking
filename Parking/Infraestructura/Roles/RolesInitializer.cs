using Microsoft.AspNetCore.Identity;
using Parking.Models;

namespace Parking.Infraestructura.Roles
{
    public class RolesInitializer : IRolesInitializer
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly IConfiguration _configuration;

        public RolesInitializer(RoleManager<IdentityRole> roleManager, UserManager<Usuario> userManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task RolesInitializeAsync()
        {
            await CrearRolesAsync();
            await CrearAdminAsync();
        }

        private async Task CrearRolesAsync()
        {
            var roles = _configuration.GetSection("Roles").Get<string[]>()!;
            foreach (var rol in roles)
            {
                    await _roleManager.CreateAsync(new IdentityRole(rol));
            }
        }

        private async Task CrearAdminAsync()
        {
            var adminSettings = _configuration.GetSection("AdminSettings").Get<AdminSettings>()!;
            var admin = await _userManager.FindByEmailAsync(adminSettings.Email);
            if (admin == null)
            {
                admin = new Usuario
                {
                    NombreCompleto = adminSettings.NombreCompleto,
                    UserName = adminSettings.UserName,
                    Email = adminSettings.Email,
                    EmailConfirmed = true
                };
                var resultado = await _userManager.CreateAsync(admin, adminSettings.Contrasena);
                if (resultado.Succeeded)
                    await _userManager.AddToRoleAsync(admin, "Administrador");
            }
        }

    }
}
