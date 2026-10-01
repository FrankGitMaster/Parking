using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Parking.Models;
using Parking.ViewModels.CuentaVM;
using Parking.ViewModels.Rol;
using System.Threading.Tasks;

namespace Parking.Controllers
{
    public class CuentaController : Controller
    {

        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ParkingDbContext _context;

        public CuentaController(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByNameAsync(viewModel.Usuario);
                if (usuario != null)
                {
                    var usuarioBloqueado = await _userManager.IsLockedOutAsync(usuario);
                    if (usuarioBloqueado)
                    {
                        dynamic tiempoRestante = (int)((await _userManager.GetLockoutEndDateAsync(usuario)) - DateTimeOffset.UtcNow).Value.TotalMinutes;
                        tiempoRestante = tiempoRestante switch
                        {
                            0 => "unos segundos...",
                            1 => $"{tiempoRestante} minuto",
                            _ => $"{tiempoRestante} minutos"
                        };
                        TempData["SwalText"] = $"Demasiados intentos fallidos,<br/><b>intente nuevamente en {tiempoRestante}</b>";
                        TempData["SwalIcon"] = "warning";
                        return View(viewModel);
                    }
                }
                else
                {
                    TempData["SwalText"] = "Usuario no encontrado";
                    TempData["SwalIcon"] = "warning";
                    return View(viewModel);
                }
                var resultado = await _signInManager.PasswordSignInAsync(
                    viewModel.Usuario,
                    viewModel.Contrasena,
                    isPersistent: viewModel.RecordarContrasena,
                    lockoutOnFailure: true
                    );
                if (resultado.Succeeded)
                    return RedirectToAction("Index", "Servicio");
                else
                {
                    TempData["SwalText"] = "Usuario o Contraseña incorrectos";
                    TempData["SwalIcon"] = "error";
                    return View(viewModel);
                }
            }
            else
            {
                TempData["SwalText"] = "Error interno al intentar ingresar al sistema";
                TempData["SwalIcon"] = "error";
                return View(viewModel);
            }
        }

        public IActionResult Registro()
        {
            var viewModel = new RegistroViewModel();
            ViewBag.Roles = ObtenerRoles();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = new Usuario
                {
                    NombreCompleto = viewModel.Nombre,
                    Email = viewModel.Correo,
                    UserName = viewModel.Correo
                };
                var resultado = await _userManager.CreateAsync(usuario, viewModel.Contrasena);
                if (resultado.Succeeded)
                {

                    var rolName = (await _roleManager.FindByIdAsync(viewModel.Rol))!.Name;
                    var resultadoAddRol = await _userManager.AddToRoleAsync(usuario, rolName);
                    if (resultadoAddRol.Succeeded)
                    {
                        TempData["SwalText"] = "Usuario creado exitosamente";
                        TempData["SwalIcon"] = "success";
                        return RedirectToAction("Index", "Usuario", viewModel);
                    }
                }
                else
                {
                    TempData["SwalIcon"] = "error";
                    foreach (var error in resultado.Errors)
                    {
                        if (error.Code == "DuplicateUserName" || error.Code == "DuplicateEmail")
                        {
                            TempData["SwalText"] = "El Correo ingresado ya está en uso";
                            break;
                        }
                        TempData["SwalText"] = "Error interno al crear el usuario";
                    }
                    return RedirectToAction("Index", "Usuario", viewModel);
                }
            }
            return RedirectToAction("Index", "Usuario", viewModel);
        }

        public IActionResult ValidarContrasena()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidarContrasena(ValidarContrasenaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByNameAsync(viewModel.Usuario);
                if (usuario != null)
                    return RedirectToAction("RestaurarContrasena", new { Usuario = viewModel.Usuario });
                TempData["SwalText"] = "Usuario no encontrado";
                TempData["SwalIcon"] = "warning";
                return View(viewModel);
            }
            return View(viewModel);
        }

        public IActionResult RestaurarContrasena(string? usuario)
        {
            if (string.IsNullOrEmpty(usuario))
                return RedirectToAction("ValidarContrasena");
            return View(new RestaurarContrasenaViewModel { Usuario = usuario });
        }

        [HttpPost]
        public async Task<IActionResult> RestaurarContrasena(RestaurarContrasenaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(viewModel.Usuario);
                if (usuario != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                    var resultado = await _userManager.ResetPasswordAsync(usuario, token, viewModel.NuevaContrasena);
                    if (resultado.Succeeded)
                    {
                        TempData["SwalText"] = "Contraseña actualizada exitosamente";
                        TempData["SwalIcon"] = "success";
                        return RedirectToAction("Login");
                    }
                    TempData["SwalText"] = "Error interno al intentar restaurar la Contraseña";
                    TempData["SwalIcon"] = "error";
                    return View(viewModel);
                }
                else
                {
                    TempData["SwalText"] = "Usuario no encontrado";
                    TempData["SwalIcon"] = "warning";
                    return View(viewModel);
                }
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        private IReadOnlyList<RolViewModel> ObtenerRoles()
        {
            var roles = _roleManager.Roles.Select(r => new RolViewModel
            {
                Id = r.Id,
                Name = r.Name
            })
            .OrderByDescending(r => r.Name).ToList().AsReadOnly();
            return roles;
        }

    }
}
