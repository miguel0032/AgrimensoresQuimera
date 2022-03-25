using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParcelaConsultingWeb.Data;
using ParcelaConsultingWeb.Extension;
using ParcelaConsultingWeb.Models;
using ParcelaConsultingWeb.Utility;
using ParcelaConsultingWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParcelaConsultingWeb.Controllers
{
    public class SolicitudesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public SolicitudesController(ApplicationDbContext context,
            UserManager<User> userManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
          

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAllData([FromBody] DtParameters dtParameters)
        {
            if (dtParameters == null)
            {
                return View();
            }
            var searchBy = dtParameters.Search?.Value;
            var orderCriteria = string.Empty;
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }
            else
            {
                orderCriteria = "Id";
                orderAscendingDirection = true;
            }

            var result = await (from x in _context.Solicitudes
                                select new ParcelaListViewModel
                                {
                                    Id = x.Id,
                                    Parcela = x.Parcelas,
                                    Cantidad = x.Cantidad_parc,
                                    FechaEntrada = x.FechadeEntrada.ToShortDateString(),
                                    Beneficiario = x.Beneficiario,
                                    Manzana = x.Manzana,
                                    FechaEnviado = x.FechaEnviado.ToShortDateString(),
                                    Status = x.Status
                                }).ToListAsync();


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(r => r.Parcela != null && r.Parcela.ToUpper().Contains(searchBy.ToUpper()) ||
                                           r.Beneficiario != null && r.Beneficiario.ToUpper().Contains(searchBy.ToUpper())
                                           ).ToList();
            }


            result = orderAscendingDirection ? result.AsQueryable().OrderByDynamic(orderCriteria, DtOrderDir.Asc).ToList() : result.AsQueryable().OrderByDynamic(orderCriteria, DtOrderDir.Desc).ToList();
            var filteredResultsCount = result.Count();
            var totalResultsCount = await _context.Users.CountAsync();


            return Json(new
            {
                draw = dtParameters.Draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToList()
            });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitud = await _context.Solicitudes
                .Include(s => s.Departamento)
                .Include(s => s.Municipio)
                .ThenInclude(s => s.Provincia)
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (solicitud == null)
            {
                return NotFound();
            }

            return View(solicitud);
        }

        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Solicitud());
            else
            {
                var Solicitud = await _context.Solicitudes
                    .Include(x => x.Usuario)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (Solicitud == null)
                {
                    return NotFound();
                }
                return View(Solicitud);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEdit(int id, Solicitud solicitud)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        solicitud.FechadeEntrada = DateTime.Now;
                        solicitud.FechaEnviado = DateTime.Now;
                        solicitud.UsuarioId = null;
                        solicitud.Status = (int)Status.creado;
                        if (string.IsNullOrEmpty(solicitud.Comentario))
                        {
                            solicitud.Comentario = "N/A";
                        }
                        _context.Add(solicitud);
                        await _context.SaveChangesAsync();
                        return Ok(new
                        {
                            isValid = true,
                            message = "Solicitud agregada correctamente."
                        });
                    }
                    else
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(solicitud.Comentario))
                            {
                                solicitud.Comentario = "N/A";
                            }

                            var modelExit = _context.Solicitudes.Where(x => x.Parcelas == Solicitud.parcelas && x.Parcelas == Solicitud.parcelas).FirstOrDefault();
                            if (modelExit != null)
                            {
                                ViewBag.Mesaje = "Esta parcela ya existe!";
                                return View();
                            }


                            _context.Update(solicitud);
                            await _context.SaveChangesAsync();
                            return Ok(new

                            {
                                isValid = true,
                                message = "Solicitud actualizada correctamente."
                            });
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!SolicitudExists(id))
                            { return NotFound(); }
                            else
                            { throw; }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"{ex}");
                }
            }

            return Json(new
            {
                isValid = false,
                message = "No se pudo agregar la parcela, verifica la información suministrada.",
                html = Utils.RenderRazorViewToString(this, "AddOrEdit", solicitud)
            });
        }

        public async Task<IActionResult> Acciones(int id)
        {
            var resp = await (from x in _context.Acciones
                              join t in _context.TipoAcciones on x.TipoAccionId equals t.Id
                              where x.SolicitudId == id
                              select new
                              {
                                  x.Id,
                                  x.Nombre,
                                  x.Descripcion,
                                  x.Create,
                                  x.SolicitudId,
                                  t.Name
                              }).ToListAsync();
            return Ok(resp);
        }

        [HttpGet]
        public async Task<IActionResult> Accion(int id = 0, int solicitudId = 0)
        {
            if (id == 0)
            {
                ViewData["SolicitudId"] = new SelectList(_context.Solicitudes, "Id", "Id", solicitudId);
                ViewData["TipoAccionId"] = new SelectList(_context.TipoAcciones, "Id", "Name");

                return View(new Accion());
            }
            else
            {
                var accion = await _context.Acciones.FindAsync(id);
                if (accion == null)
                {
                    return Json(new
                    {
                        isValid = false,
                        message = "No se Encontro la accion, intentelo de nuevo por favor."
                    });
                }

                ViewBag.solicitudId = solicitudId;
                return View(accion);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Accion(int id, Accion accion)
        {
            if (accion == null)
            {
                return Json(new
                {
                    isValid = false,
                    message = "Error al intentar Agregar la accion, intentelo de nuevo por favor."
                });
            }

            try
            {
                if (id == 0)
                {
                    accion.Create = DateTime.Now;
                    accion.Active = true;

                    _context.Acciones.Add(accion);
                }
                else
                {
                    accion.LastModifier = DateTime.Now;

                    _context.Acciones.Update(accion);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("AddOrEdit", new { id = accion.SolicitudId });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isValid = false,
                    message = $"Error : {ex}"
                });
            }
        }

        [HttpGet]
        public IActionResult AsignarUsuario(int id)
        {
            AsignarUsuarioViewModel usuario = new AsignarUsuarioViewModel
            {
                Id = id,
                UserId = ""
            };
            return PartialView("~/Views/Solicitudes/_AsignarUsuario.cshtml", usuario);
        }


        [HttpPost]
        public async Task<IActionResult> AsignarUsuario(int id, string userId)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null)
            {
                return Json(new
                {
                    isValid = false,
                    message = "No se Puede asignar el Usuario.",
                    html = Utils.RenderRazorViewToString(this, "Index", null)
                });
            }

            solicitud.UsuarioId = userId;
            solicitud.FechaAsignacion = DateTime.Now;
            solicitud.Status = (int)Status.asignado;
            _context.Update(solicitud);
            await _context.SaveChangesAsync();

            return Json(new
            {
                isValid = true,
                message = "Usuario asignado correctamente."
            });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new
                {
                    isValid = false,
                    message = "No se encuentra la Solicitud de Parcela",
                    html = Utils.RenderRazorViewToString(this, "Index", null)
                });
            }

            var solicitud = await _context.Solicitudes
                .Include(s => s.Departamento)
                .Include(s => s.Municipio)
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (solicitud == null)
            {
                return Json(new
                {
                    isValid = false,
                    message = "No se pudo eliminar la Solicitud de Parcela",
                    html = Utils.RenderRazorViewToString(this, "Index", null)
                });
            }

            var solicitudDelete = await _context.Solicitudes.FindAsync(id);
            _context.Solicitudes.Remove(solicitud);
            await _context.SaveChangesAsync();

            return Json(new
            {
                isValid = true,
                message = "La Solicitud de parcela se eliminó correctamente!",
                html = Utils.RenderRazorViewToString(this, "Index", null)
            });
        }

        private bool SolicitudExists(int id)
        {
            return _context.Solicitudes.Any(e => e.Id == id);
        }

        public IActionResult GetSolicitudById(int id)
        {
            var result = (from x in _context.Solicitudes
                          join m in _context.Municipios on x.MunicipioId equals m.MunicipioId
                          join d in _context.Departamentos on x.DepartamentoId equals d.id
                          join p in _context.Provincias on m.ProvinciaId equals p.ProvinciaId
                          where x.Id == id
                          select new
                          {
                              Id = x.Id,
                              MunicipioId = m.MunicipioId,
                              Municipio = m.Name,
                              ProvinciaId = p.ProvinciaId,
                              Provincia = p.Name,
                              DepartamentoId = d.id,
                              Departamrnto = d.Name
                          }).FirstOrDefault();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Salida(int? id)
        {
            if (id == 0)
            {
                return Json(new
                {
                    isValid = false,
                    message = "Ha ocurrido un error al momento de dar salida, intente de nuevo por favor."
                });
            }
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null)
            {
                return Json(new
                {
                    isValid = false,
                    message = "Ha ocurrido un error al momento de dar salida, intente de nuevo por favor."
                });
            }

            solicitud.Status = (int)Status.enviado;
            solicitud.FechaEnviado = DateTime.Now;
            _context.Update(solicitud);
            await _context.SaveChangesAsync();

            return Json(new
            {
                isValid = true,
                message = "La salida ha sido sastifactoria!."
            });

        }
            //GET :Autocompletar
            public List<Solicitud> GetEntradas(string term = "")
            {

                List<Solicitud> solicitud = new List<Solicitud>();

            //get data
            solicitud = (from x in _context.Solicitudes
                            .Where(x => x.Parcelas.Contains(term))
                            select x).ToList();

                return solicitud;
            }
        }

    }

