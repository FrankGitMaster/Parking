using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Parking.Models;
using Parking.ViewModels.CuentaVM;

namespace Parking.Controllers
{
    public class UsuarioController : Controller
    {

        private readonly UserManager<Usuario> _userManager;

        public UsuarioController(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = _userManager.Users.Select(u => new UsuarioViewModel
            {
                NombreCompleto = u.NombreCompleto,
                UserName = u.UserName
            }).ToList();
            foreach(var usuarioVM in usuarios)
            {
                var usuario = await _userManager.FindByEmailAsync(usuarioVM.UserName);
                usuarioVM.Rol = (await _userManager.GetRolesAsync(usuario)).ToList().FirstOrDefault() ?? "Sin Rol";
            }
            return View(usuarios);
        }
    }
}
