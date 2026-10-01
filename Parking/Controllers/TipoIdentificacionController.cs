using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Models;
using Parking.ViewModels.TipoIdentificacionVM;
using System.Threading.Tasks;

namespace Parking.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class TipoIdentificacionController : Controller
    {

        private ParkingDbContext _context;

        public TipoIdentificacionController(ParkingDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tiposIdentificacion = _context.TipoIdentificacion.Select(ti => new TipoIdentificacionViewModel
            {
                Id = ti.Id,
                Tipo = ti.Tipo,
                Sigla = ti.Sigla,
                Estado = ti.Estado
            })
            .OrderBy(ti => ti.Id).ToList();
            return View(tiposIdentificacion);
        }

        public async Task<IActionResult> TipoIdentificacionForm(int id)
        {
            ViewBag.Accion = id > 0 ? "Editar" : "Crear";
            var viewModel = new TipoIdentificacionInsertViewModel();
            if (id > 0)
            {
                var tipoDocumento = await _context.TipoIdentificacion.FindAsync(id);
                if (tipoDocumento != null)
                {
                    viewModel.Id = tipoDocumento.Id;
                    viewModel.Tipo = tipoDocumento.Tipo;
                    viewModel.Sigla = tipoDocumento.Sigla;
                    viewModel.Estado = tipoDocumento.Estado;
                }
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearEditarTipoIdentificacion(TipoIdentificacionInsertViewModel viewModel)
        {
            ViewBag.Accion = viewModel.Id > 0 ? "Editar" : "Crear";
            if (ModelState.IsValid && viewModel != null)
            {
                try
                {
                    var model = await _context.TipoIdentificacion.FindAsync(viewModel.Id);
                    if (model == null)
                        model = new TipoIdentificacion();
                    model.Tipo = viewModel.Tipo;
                    model.Sigla = viewModel.Sigla;
                    model.Estado = viewModel.Estado;
                    if (model.Id > 0)
                        model.FechaActualizacion = DateTime.Now;
                    else
                        _context.TipoIdentificacion.Add(model);
                    await _context.SaveChangesAsync();
                    TempData["SwalText"] = $"Tipo de Identificación {(viewModel.Id > 0 ? "actualizado" : "creado")} exitosamente!";
                    TempData["SwalIcon"] = "success";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    TempData["SwalText"] = "Error de base de datos";
                    TempData["SwalIcon"] = "error";
                    return View("TipoIdentificacionForm", viewModel);
                }
                catch (Exception)
                {
                    TempData["SwalText"] = "Error inesperado";
                    TempData["SwalIcon"] = "error";
                    return View("TipoIdentificacionForm", viewModel);
                }
            }
            return View("TipoIdentificacionForm", viewModel);
        }

        public async Task<IActionResult> EliminarTipoIdentificacion(int id)
        {
            var tipoIdentificacion = await _context.TipoIdentificacion.FindAsync(id);
            if (tipoIdentificacion != null)
            {
                try
                {
                    _context.TipoIdentificacion.Remove(tipoIdentificacion);
                    await _context.SaveChangesAsync();
                    TempData["SwalText"] = "Tipo de Identificación eliminado exitosamente!";
                    TempData["SwalIcon"] = "success";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["SwalText"] = "Error al intentar eliminar el Tipo de Identificación";
                    TempData["SwalIcon"] = "error";
                    return RedirectToAction("Index");
                }
            }
            TempData["SwalText"] = "Tipo de Identificación no encontrado";
            TempData["SwalIcon"] = "error";
            return RedirectToAction("Index");
        }

    }
}
