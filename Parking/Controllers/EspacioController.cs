using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation.Contracts;
using OfficeOpenXml.Style;
using Parking.DTOs;
using Parking.Models;
using Parking.Utilities.MetodosExtension;
using Parking.ViewModels.EspacioVM;
using Parking.ViewModels.TipoVehiculo;
using System.Security.Claims;

namespace Parking.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EspacioController : Controller
    {

        private readonly UserManager<Usuario> _userManager;
        private readonly ParkingDbContext _context;

        public EspacioController(UserManager<Usuario> userManager, ParkingDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var espacios = _context.Espacios.Select(e => new EspacioViewModel
            {
                Id = e.Id,
                Numero = e.Numero,
                TipoVehiculo = e.TipoVehiculoNavigation.Tipo,
                Estado = e.Estado
            })
            .OrderBy(e => e.Numero).ToList();
            ViewBag.TiposVehiculo = (await _context.TipoVehiculo
                .Select(tv => new TipoVehiculoViewModel
                {
                    Id = tv.Id,
                    Tipo = tv.Tipo
                })
                .OrderBy(tv => tv.Id).ToListAsync()).AsReadOnly();
            return View(espacios);
        }

        public async Task<IActionResult> FiltrarEspaciosPorTipoVehiculo(int id)
        {
            IQueryable<Espacio> query = _context.Espacios;
            if (id != 0)
                query = query.Where(e => e.IdTipoVehiculo == id);
            var espacios = await query.Select(e => new EspacioViewModel
            {
                Id = e.Id,
                Numero = e.Numero,
                TipoVehiculo = e.TipoVehiculoNavigation.Tipo,
                Estado = e.Estado
            }).ToListAsync();
            return Json(espacios);
        }

        public async Task<IActionResult> EspacioForm(int id)
        {
            var viewModel = new EspacioInsertViewModel();
            ViewBag.TiposVehiculo = CargarTiposVehiculo();
            ViewBag.Accion = id > 0 ? "Editar" : "Crear";
            if (id > 0)
            {
                var espacio = await _context.Espacios.FindAsync(id);
                if (espacio != null)
                {
                    viewModel.Id = espacio.Id;
                    viewModel.Numero = espacio.Numero;
                    viewModel.IdTipoVehiculo = espacio.IdTipoVehiculo;
                    viewModel.Estado = espacio.Estado;
                    viewModel.EstadoInactivo = espacio.Estado == "X" ? true : false;
                }
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearEditarEspacio(EspacioInsertViewModel viewModel)
        {
            ViewBag.TiposVehiculo = CargarTiposVehiculo();
            ViewBag.Accion = viewModel.Id > 0 ? "Editar" : "Crear";
            if (ModelState.IsValid && viewModel != null)
            {
                try
                {
                    var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var espacio = await _context.Espacios.FindAsync(viewModel.Id);
                    if (espacio == null)
                        espacio = new Espacio();
                    espacio.IdUsuarioActualizacion = idUsuario;
                    espacio.Numero = viewModel.Numero;
                    espacio.Estado = viewModel.EstadoInactivo ? "X" : viewModel.Estado;
                    espacio.IdTipoVehiculo = viewModel.IdTipoVehiculo;
                    if (espacio.Id > 0)
                    {
                        await _context.Database.ExecuteSqlRawAsync
                            ("""
                            UPDATE espacio
                            SET numero = {0},
                                id_tipo_vehiculo = {1},
                                estado = {2},
                                fecha_actualizacion = NOW(),
                                id_usuario_actualizacion = {3}
                            WHERE id = {4}
                            """,
                            espacio.Numero,
                            espacio.IdTipoVehiculo,
                            espacio.Estado,
                            espacio.IdUsuarioActualizacion,
                            espacio.Id
                            );
                    }
                    else
                    {
                        _context.Espacios.Add(espacio);
                        await _context.SaveChangesAsync();
                    }
                    TempData["SwalText"] = $"Espacio {(espacio.Id > 0 ? "actualizado" : "creado")} exitosamente!";
                    TempData["SwalIcon"] = "success";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    TempData["SwalText"] = "Error en base de datos";
                    TempData["SwalIcon"] = "error";
                    return RedirectToAction("EspacioForm", viewModel);
                }
                catch (Exception)
                {
                    TempData["SwalText"] = "Error inesperado";
                    TempData["SwalIcon"] = "error";
                    return RedirectToAction("EspacioForm", viewModel);
                }
            }
            return View("EspacioForm", viewModel);
        }

        private List<TipoVehiculoViewModel> CargarTiposVehiculo() => _context.TipoVehiculo.Where(tv => tv.Estado == "A").Select(tv => new TipoVehiculoViewModel { Id = tv.Id, Tipo = tv.Tipo }).OrderBy(tv => tv.Id).ToList();

        public async Task<IActionResult> EliminarEspacio(int id)
        {
            var espacio = await _context.Espacios.FindAsync(id);
            if (espacio != null)
            {
                try
                {
                    _context.Espacios.Remove(espacio);
                    await _context.SaveChangesAsync();
                    TempData["SwalText"] = "Espacio eliminado exitosamente!";
                    TempData["SwalIcon"] = "success";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["SwalText"] = "Error al intentar eliminar el Espacio";
                    TempData["SwalIcon"] = "error";
                    return RedirectToAction("Index");
                }
            }
            TempData["SwalText"] = "Espacio no encontrado";
            TempData["SwalIcon"] = "error";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GenerarReporteExcelServiciosEspacio(string espacioNumero)
        {
            var servicios = await _context.Servicios.Where(s => s.EspacioNumero == espacioNumero)
                .Select(s => new ServicioReporteDTO
                {
                    IdServicio = s.Id.ToString(),
                    TipoVehiculo = s.TipoVehiculoNavigation!.Tipo,
                    Placa = s.Placa,
                    Color = s.Color,
                    Observacion = s.Observacion ?? "",
                    Espacio = s.EspacioNumero,
                    FechaHoraIngreso = s.FechaHoraIngreso.ToString(),
                    FechaHoraSalida = s.FechaHoraSalida.ToString()!,
                    UsuarioFinalizacion = s.UsuarioFinalizacionNavigation!.UserName!,
                    TotalMinutos = s.TotalMinutos.ToString(),
                    ValorTotal = s.ValorTotal.ToString(),
                    TarifaPlena = s.TarifaPlena ? "X" : "",
                    FechaCreacion = s.FechaCreacion.ToString(),
                    UsuarioCreacion = s.UsuarioCreacionNavigation!.UserName!,
                    FechaActualizacion = s.FechaActualizacion.ToString() ?? "",
                    UsuarioActualizacion = s.UsuarioActualizacionNavigation!.UserName!
                })
                .OrderByDescending(s => s.FechaCreacion).ToListAsync();
            (var ExcelBytes, var nombreArchivo) = await GenerarReporteExcel(servicios, $"Reporte de Servicios en Espacio {espacioNumero}");
            return File(
                fileContents: ExcelBytes,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: nombreArchivo);
        }

        private async Task<(byte[] excelBytes, string nombreArchivo)> GenerarReporteExcel<T>(List<T> datos, string tituloReporte)
        {
            try
            {
                //CREACIÓN DE ARCHIVO Y CARGA DE DATOS
                var nombreArchivo = tituloReporte.FormatearNombreArchivo("xlsx");
                using var package = new ExcelPackage(nombreArchivo); //Crear una instancia de ExcelPackage a partir de un nuevo archivo
                var workSheet = package.Workbook.Worksheets.Add("Reporte"); //Crear una hoja de trabajo al archivo Excel
                var rangeBase = workSheet.Cells["A2"].LoadFromCollection(datos, PrintHeaders: true); //Cargar la lista de datos a partir de un rango de celdas especifico
                rangeBase.AutoFitColumns(); //Establecer el ancho automático de las celdas con datos del rango de datos

                //HEADER
                workSheet.Cells["A1"].Value = tituloReporte; //Asignar un contenido a una celda
                workSheet.Cells["A1:Z1"].Merge = true; //Combinar celdas
                workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Size = 24;

                //DATA(rangeBase)
                workSheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; //Centrar horizontalmente el contenido de las celdas de una columna
                workSheet.Row(2).Style.Font.Bold = true;

                var excelBytes = await package.GetAsByteArrayAsync();
                return (excelBytes, nombreArchivo);
            }
            catch (Exception)
            {
                throw new Exception("Error al intentar generar el reporte");
            }
        }

    }
}