using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Models;
using Parking.ViewModels.EspacioVM;
using Parking.ViewModels.ServicioVM;
using Parking.ViewModels.TipoVehiculo;
using System.Security.Claims;

namespace Parking.Controllers
{
    [Authorize]
    public class ServicioController : Controller
    {

        private readonly UserManager<Usuario> _userManager;
        private ParkingDbContext _context;

        public ServicioController(UserManager<Usuario> userManager, ParkingDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var servicios = await _context.Servicios.Where(s => s.Estado == "A")
                .Include(s => s.UsuarioCreacionNavigation)
                .Select(s => new ServicioViewModel
                {
                    Id = s.Id,
                    Placa = s.Placa,
                    Color = s.Color,
                    TipoVehiculo = s.TipoVehiculoNavigation.Tipo,
                    Observacion = s.Observacion ?? "-",
                    Espacio = s.EspacioNumero,
                    UsuarioCreacion = s.UsuarioCreacionNavigation.UserName ?? "Sin especificar",
                    FechaHoraIngreso = s.FechaHoraIngreso.ToString(),
                    FechaHoraSalida = s.FechaHoraSalida.ToString() ?? "-",
                    TotalMinutos = s.TotalMinutos == 0 ? "-" : s.TotalMinutos.ToString(),
                    ValorTotal = s.ValorTotal == 0 ? "-" : s.ValorTotal.ToString(),
                    TarifaPlena = s.TarifaPlena,
                    Estado = s.Estado
                }).OrderByDescending(s => s.FechaHoraIngreso).ToListAsync();
            return View("Index", servicios.AsReadOnly());
        }

        public async Task<IActionResult> ObtenerTodo()
        {
            var servicios = await _context.Servicios.Include(s => s.TipoVehiculoNavigation)
                .Select(s => new ServicioViewModel
                {
                    Id = s.Id,
                    Placa = s.Placa,
                    Color = s.Color,
                    TipoVehiculo = s.TipoVehiculoNavigation.Tipo,
                    Observacion = s.Observacion ?? "-",
                    Espacio = s.EspacioNumero,
                    UsuarioCreacion = "admin",
                    FechaHoraIngreso = s.FechaHoraIngreso.ToString(),
                    FechaHoraSalida = s.FechaHoraSalida.ToString() ?? "-",
                    TotalMinutos = s.TotalMinutos == 0 ? "-" : s.TotalMinutos.ToString(),
                    ValorTotal = s.ValorTotal == 0 ? "-" : s.ValorTotal.ToString(),
                    TarifaPlena = s.TarifaPlena,
                    Estado = s.Estado
                })
                .OrderBy(s => s.Estado)
                .ThenByDescending(s => s.FechaHoraIngreso).ToListAsync();
            return View("Index", servicios.AsReadOnly());
        }

        public async Task<IActionResult> FiltrarFinalizados()
        {
            var servicios = await _context.Servicios.Where(s => s.Estado == "F")
                .Select(s => new ServicioViewModel
                {
                    Id = s.Id,
                    Placa = s.Placa,
                    Color = s.Color,
                    TipoVehiculo = s.TipoVehiculoNavigation.Tipo,
                    Observacion = s.Observacion ?? "-",
                    Espacio = s.EspacioNumero,
                    UsuarioCreacion = "admin",
                    FechaHoraIngreso = s.FechaHoraIngreso.ToString(),
                    FechaHoraSalida = s.FechaHoraSalida.ToString() ?? "-",
                    TotalMinutos = s.TotalMinutos == 0 ? "-" : s.TotalMinutos.ToString(),
                    ValorTotal = s.ValorTotal == 0 ? "-" : s.ValorTotal.ToString(),
                    TarifaPlena = s.TarifaPlena,
                    Estado = s.Estado
                }).OrderByDescending(s => s.FechaHoraIngreso).ToListAsync();
            return View("Index", servicios.AsReadOnly());
        }

        public async Task<IActionResult> ServicioForm(int id)
        {
            var viewModel = new ServicioInsertViewModel();
            var servicio = await _context.Servicios.FindAsync(id);
            int? idTipoVehiculo = null;
            if (servicio != null)
            {
                viewModel.Id = servicio.Id;
                viewModel.Placa = servicio.Placa;
                viewModel.Color = servicio.Color;
                viewModel.IdTipoVehiculo = servicio.IdTipoVehiculo;
                viewModel.Observacion = servicio.Observacion;
                viewModel.Espacio = servicio.EspacioNumero;
                idTipoVehiculo = servicio.IdTipoVehiculo;
            }
            (var listaTiposVehiculo, var listaEspacios) = await ObtenerSelectsList((id > 0 ? id : null), idTipoVehiculo);
            var espaciosDisponibles = listaEspacios.Count();
            ViewBag.TiposVehiculo = listaTiposVehiculo.AsReadOnly();
            ViewBag.Espacios = listaEspacios.AsReadOnly();
            ViewBag.Accion = id > 0 ? "Editar" : "Crear";
            return View(viewModel);
        }

        public async Task<IActionResult> CrearEditarServicio(ServicioInsertViewModel viewModel)
        {
            (var listaTiposVehiculo, var listaEspacios) = await ObtenerSelectsList((viewModel.Id > 0 ? viewModel.Id : null));
            ViewBag.TiposVehiculo = listaTiposVehiculo.AsReadOnly();
            ViewBag.Espacios = listaEspacios.AsReadOnly();
            ViewBag.Accion = viewModel.Id > 0 ? "Editar" : "Crear";
            if (ModelState.IsValid && viewModel != null)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                    var servicio = await _context.Servicios.FindAsync(viewModel.Id);
                    if (servicio == null)
                    {
                        servicio = new Servicio();
                        servicio.IdUsuarioCreacion = idUsuario;
                    }
                    string? idEspacioActual = viewModel.Id > 0 ? servicio.EspacioNumero : null;
                    var espacioSeleccionado = await _context.Espacios.FirstOrDefaultAsync(e => e.Numero == viewModel.Espacio) ?? throw new Exception();
                    servicio.Placa = viewModel.Placa;
                    servicio.Color = viewModel.Color;
                    servicio.IdTipoVehiculo = viewModel.IdTipoVehiculo;
                    servicio.Observacion = viewModel.Observacion;
                    servicio.EspacioNumero = espacioSeleccionado.Numero;
                    servicio.TarifaPlena = viewModel.TarifaPlena;
                    servicio.EspacioNumero = viewModel.Espacio;
                    if (viewModel.Id > 0)
                    {
                        servicio.IdUsuarioActualizacion = idUsuario;
                        servicio.FechaActualizacion = DateTime.Now;
                    }
                    else
                        _context.Servicios.Add(servicio);
                    await _context.SaveChangesAsync();
                    if (espacioSeleccionado != null)
                    {
                        if (espacioSeleccionado.Numero != "C")
                        {
                            espacioSeleccionado.Estado = "O";
                            espacioSeleccionado.IdServicioActivo = servicio.Id;
                            if (idEspacioActual != null)
                            {
                                var espacioActual = await _context.Espacios.Where(e => e.Numero == idEspacioActual).FirstOrDefaultAsync();
                                if (espacioActual != null && espacioActual.Numero != espacioSeleccionado.Numero)
                                {
                                    espacioActual.IdUsuarioActualizacion = idUsuario;
                                    espacioActual.FechaActualizacion = DateTime.Now;
                                    espacioActual.Estado = "L";
                                    espacioActual.IdServicioActivo = null;
                                }
                            }
                        }
                        espacioSeleccionado.IdUsuarioActualizacion = idUsuario;
                        espacioSeleccionado.FechaActualizacion = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    await transaction.CommitAsync();
                    TempData["SwalText"] = $"Servicio {(viewModel.Id > 0 ? "Editado" : "Creado")} exitosamente!";
                    TempData["SwalIcon"] = "success";
                    var espaciosDisponibles = _context.Espacios.Where(e => e.IdTipoVehiculo == servicio.IdTipoVehiculo && e.Estado == "L").Count();
                    if (espaciosDisponibles == 0)
                    {
                        var tipoVehiculo = await _context.TipoVehiculo.Where(tv => tv.Id == servicio.IdTipoVehiculo).Select(tv => tv.Tipo).FirstOrDefaultAsync();
                        TempData["SwalTextMessage"] = $"No quedan Espacios disponibles para {tipoVehiculo}";
                        TempData["SwalIconMessage"] = "info";
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    TempData["SwalText"] = "Error al intentar crear el Servicio";
                    TempData["SwalIcon"] = "error";
                    return View("ServicioForm", viewModel);
                }
            }
            return View("ServicioForm", viewModel);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EliminarServicio(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var servicio = await _context.Servicios.FindAsync(id);
                if (servicio != null)
                {
                    _context.Servicios.Remove(servicio);
                    var espacio = _context.Espacios.FirstOrDefault(e => e.IdServicioActivo == id);
                    if (espacio != null)
                    {
                        espacio.IdUsuarioActualizacion = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        espacio.FechaActualizacion = DateTime.Now;
                        espacio.Estado = "L";
                        espacio.IdServicioActivo = null;
                    }
                    else
                        throw new Exception();
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["SwalText"] = "Servicio eliminado exitosamente";
                    TempData["SwalIcon"] = "success";
                    return RedirectToAction("Index");
                }
                TempData["SwalText"] = "Servicio no encontrado";
                TempData["SwalIcon"] = "error";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["SwalText"] = "Error al intentar eliminar el servicio";
                TempData["SwalIcon"] = "error";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> FinalizarServicio(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var servicio = await _context.Servicios.FindAsync(id);
                if (servicio != null)
                {
                    var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    servicio.IdUsuarioFinalizacion = idUsuario;
                    servicio.IdUsuarioActualizacion = idUsuario;
                    servicio.FechaActualizacion = DateTime.Now;
                    servicio.Estado = "F";
                    servicio.FechaHoraSalida = DateTime.Now;
                    servicio.TotalMinutos = CalcularTotalMinutos(servicio.FechaHoraIngreso, servicio.FechaHoraSalida);
                    if (servicio.TarifaPlena)
                        servicio.ValorTotal = await CalcularValorTotal(servicio.FechaHoraIngreso, servicio.IdTipoVehiculo, totalMinutos: null, tarifaPlena: true);
                    else
                        servicio.ValorTotal = await CalcularValorTotal(servicio.FechaHoraIngreso, servicio.IdTipoVehiculo, totalMinutos: servicio.TotalMinutos, tarifaPlena: false);
                    var espacio = servicio.EspacioNumero == "C" ? await _context.Espacios.FirstOrDefaultAsync(e => e.Numero == "C") : await _context.Espacios.FirstOrDefaultAsync(e => e.IdServicioActivo == servicio.Id);
                    if (espacio != null)
                    {
                        espacio.IdUsuarioActualizacion = (await _userManager.GetUserAsync(User))!.Id;
                        espacio.FechaActualizacion = DateTime.Now;
                        if (espacio.Numero != "C")
                        {
                            espacio.Estado = "L";
                            espacio.IdServicioActivo = null;
                        }
                    }
                    else
                        throw new Exception();
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["SwalText"] = "Servicio finalizado exitosamente";
                    TempData["SwalIcon"] = "success";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["SwalText"] = "Servicio no encontrado";
                    TempData["SwalIcon"] = "error";
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message + "\nInner: " + ex.InnerException);
                throw;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["SwalText"] = "Error interno al finalizar el Servicio";
                TempData["SwalIcon"] = "error";
                return RedirectToAction("Index");
            }
        }

        private int CalcularTotalMinutos(DateTime fechaHoraInicial, DateTime? fechaHoraFinal)
        {
            try
            {
                if (fechaHoraFinal.HasValue)
                {
                    var totalMinutos = (int)(fechaHoraFinal - fechaHoraInicial).Value.TotalMinutes;
                    totalMinutos = totalMinutos == 0 ? 1 : totalMinutos;
                    return totalMinutos;
                }
                else
                    throw new Exception("La Fecha y Hora de salida del Servicio no ha sido establecida");
            }
            catch (Exception)
            {
                throw new Exception("Error al intentar calcular el total de minutos del Servicio");
            }
        }

        private async Task<decimal> CalcularValorTotal(DateTime fechaInicial, int idVehiculo, int? totalMinutos = null, bool? tarifaPlena = null)
        {
            try
            {
                decimal totalAPagar = 0;
                var valorTarifaPorMinuto = await _context.Tarifas.Where(t => t.IdTipoVehiculo == idVehiculo).Select(t => t.tarifaMinuto).FirstOrDefaultAsync();
                if (tarifaPlena.HasValue && tarifaPlena.Value)
                {
                    var valorTarifaPlena = await _context.Tarifas.Where(t => t.IdTipoVehiculo == idVehiculo).Select(t => t.tarifaPlena).FirstOrDefaultAsync();
                    var totalMinutosAdicionales = (int)(DateTime.Now - fechaInicial.AddHours(12)).TotalMinutes;
                    if (totalMinutosAdicionales > 0)
                        totalAPagar = totalMinutosAdicionales * valorTarifaPorMinuto;
                    return totalAPagar += valorTarifaPlena;
                }
                else
                {
                    if (totalMinutos.HasValue)
                    {
                        totalMinutos = totalMinutos.Value == 0 ? 1 : totalMinutos;
                        return totalMinutos.Value * valorTarifaPorMinuto;
                    }
                    else
                        throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new Exception("Error al intentar calcular el valor del Servicio");
            }
        }

        private async Task<List<TipoVehiculoViewModel>> ObtenerTiposVehiculo(int? id = null)
        {
            try
            {
                var listaTiposVehiculo = await _context.TipoVehiculo.Where(tv => tv.Estado == "A")
                    .Select(tv => new TipoVehiculoViewModel
                    {
                        Id = tv.Id,
                        Tipo = tv.Tipo
                    })
                    .OrderBy(tv => tv.Id).ToListAsync();
                return listaTiposVehiculo;
            }
            catch (Exception)
            {
                throw new Exception("Error al obtener los Tipos de Vehículo");
            }
        }

        private async Task<List<EspacioViewModel>> ObtenerEspacios(int? id = null, int? idTipoVehiculo = null)
        {
            try
            {
                var listaEspacios = new List<EspacioViewModel>();
                idTipoVehiculo = idTipoVehiculo == null ? _context.TipoVehiculo.OrderBy(tv => tv.Id).First().Id : idTipoVehiculo;
                if (id.HasValue && id.Value > 0)
                {
                    listaEspacios = await _context.Espacios.Where(e => (e.Estado == "L" || e.IdServicioActivo == id.Value) && e.IdTipoVehiculo == idTipoVehiculo.Value)
                    .Select(e => new EspacioViewModel
                    {
                        Id = e.Id,
                        Numero = e.Numero,
                        Estado = e.Estado
                    }).OrderByDescending(e => e.Estado)
                    .ThenBy(e => e.Numero).ToListAsync();
                }
                else
                {
                    listaEspacios = await _context.Espacios.Where(e => e.Estado == "L" && e.IdTipoVehiculo == idTipoVehiculo.Value)
                    .Select(e => new EspacioViewModel
                    {
                        Id = e.Id,
                        Numero = e.Numero
                    })
                    .OrderBy(e => e.Numero).ToListAsync();
                }
                return listaEspacios;
            }
            catch (Exception)
            {
                throw new Exception("Error al obtener los Tipos de Vehículo");
            }
        }

        private async Task<(List<TipoVehiculoViewModel> listaTiposVehiculo, List<EspacioViewModel> listaEspacios)> ObtenerSelectsList(int? id = null, int? idTipoVehiculo = null)
        {
            var listaTiposVehiculo = await ObtenerTiposVehiculo(id);
            var listaEspacios = await ObtenerEspacios(id, idTipoVehiculo);
            return (listaTiposVehiculo, listaEspacios);
        }

        public async Task<IActionResult> ObtenerEspaciosJson(int? id = null, int? idTipoVehiculo = null) => Json(await ObtenerEspacios(id, idTipoVehiculo));
    }
}
