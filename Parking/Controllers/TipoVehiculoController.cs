using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Models;
using Parking.ViewModels.TipoVehiculo;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Parking.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class TipoVehiculoController : Controller
    {

        private readonly ILogger<TipoVehiculoController> _logger;
        private readonly ParkingDbContext _context;

        public TipoVehiculoController(ILogger<TipoVehiculoController> logger, ParkingDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var tiposVehiculo = _context.TipoVehiculo.Select(tv => new TipoVehiculoViewModel
            {
                Id = tv.Id,
                Tipo = tv.Tipo,
                Estado = tv.Estado,
            })
            .OrderBy(tv => tv.Id).ToList();
            return View(tiposVehiculo);
        }

        public async Task<IActionResult> TipoVehiculoForm(int id)
        {
            var viewModel = new TipoVehiculoInsertViewModel();
            ViewBag.Accion = id > 0 ? "Editar" : "Crear";
            var tipoVehiculo = await _context.TipoVehiculo.FindAsync(id);
            if (tipoVehiculo != null)
            {
                viewModel.Id = tipoVehiculo.Id;
                viewModel.Tipo = tipoVehiculo.Tipo;
                viewModel.Estado = tipoVehiculo.Estado;
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearEditarTipoVehiculo(TipoVehiculoInsertViewModel viewModel)
        {
            ViewBag.Accion = viewModel.Id > 0 ? "Editar" : "Crear";
            if (ModelState.IsValid && viewModel != null)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    var tipoVehiculo = await _context.TipoVehiculo.FindAsync(viewModel.Id);
                    if (tipoVehiculo == null)
                        tipoVehiculo = new TipoVehiculo();
                    tipoVehiculo.Tipo = viewModel.Tipo;
                    tipoVehiculo.Estado = viewModel.Estado;
                    tipoVehiculo.IdUsuarioActualizacion = 1;
                    if (viewModel.Id > 0)
                    {
                        tipoVehiculo.FechaActualizacion = DateTime.Now;
                        tipoVehiculo.IdUsuarioActualizacion = 1;
                    }
                    else
                        _context.TipoVehiculo.Add(tipoVehiculo);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["SwalText"] = $"Tipo de Vehículo {(viewModel.Id > 0 ? "editado" : "creado")} exitosamente";
                    TempData["SwalIcon"] = "success";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    await transaction.RollbackAsync();
                    TempData["SwalText"] = "Error en base de datos";
                    TempData["SwalIcon"] = "error";
                    return View("TipoVehiculoForm", viewModel);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    TempData["SwalText"] = "Error inesperado";
                    TempData["SwalIcon"] = "error";
                    return View("TipoVehiculoForm", viewModel);
                }
            }
            return View("TipoVehiculoForm", viewModel);
        }

        public async Task<IActionResult> EliminarTipoVehiculo(int id)
        {
            var tipoVehiculo = await _context.TipoVehiculo.FirstOrDefaultAsync(tv => tv.Id == id);
            if (tipoVehiculo != null)
            {
                try
                {
                    _context.TipoVehiculo.Remove(tipoVehiculo);
                    await _context.SaveChangesAsync();
                    TempData["SwalText"] = "Tipo de Vehículo eliminado exitosamente";
                    TempData["SwalIcon"] = "success";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["SwalText"] = "Error al intentar eliminar el Tipo de Vehículo";
                    TempData["SwalIcon"] = "error";
                    return RedirectToAction("Index");
                }
            }
            TempData["SwalText"] = "Tipo de Vehículo no encontrado";
            TempData["SwalIcon"] = "error";
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
