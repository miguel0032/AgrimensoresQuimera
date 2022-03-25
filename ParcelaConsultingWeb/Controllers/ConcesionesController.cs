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
    public class ConcesionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConcesionesController(ApplicationDbContext context)
        {
            _context = context;
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
            var concesion = await _context.Concesiones.FindAsync(id);
            if (concesion == null)
            {
                return Json(new
                {
                    isValid = false,
                    message = "No se Puede asignar el Usuario.",
                    html = Utils.RenderRazorViewToString(this, "Index", null)
                });
            }

            concesion.UsuarioId = userId;
            concesion.FechaAsignacion = DateTime.Now;
            concesion.Status = (int)Status.asignado;
            _context.Update(concesion);
            await _context.SaveChangesAsync();

            return Json(new
            {
                isValid = true,
                message = "Usuario asignado correctamente."
            });
        }


        // GET: Concesiones
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Concesiones.Include(c => c.Departamento).Include(c => c.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Concesiones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var concesion = await _context.Concesiones
                .Include(c => c.Departamento)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (concesion == null)
            {
                return NotFound();
            }

            return Ok(concesion);
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

            var result = await (from x in _context.Concesiones
                                select new ConcesionesListViewModel
                                {
                                    Id = x.Id,
                                    NoExpediente = x.NoExpediente,
                                    Remitente = x.Remitente,
                                    Asunto = x.Asunto,
                                    Asignado = x.Asignado,
                                    FechaAsignacion = x.FechaAsignacion.ToShortDateString(),
                                    FechaSalida = x.FechaEnviado.ToShortDateString(),
                                    FechaEnviado = x.FechaEnviado.ToShortDateString(),
                                    DiarenaNo = x.DiarenaNo,
                                    Departamento = x.Departamento.Name
                                }).ToListAsync();


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(r => r.Asunto != null && r.Asunto.ToUpper().Contains(searchBy.ToUpper()) ||
                                           r.NoExpediente != null && r.NoExpediente.ToUpper().Contains(searchBy.ToUpper())
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


        [HttpPost]
        public async Task<IActionResult> AddOrEdit(int id, Concesion concesion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        concesion.FechaAsignacion = DateTime.Now;
                        concesion.UsuarioId = null;
                        concesion.FechaEnviado = DateTime.Now;
                        if (string.IsNullOrEmpty(concesion.Comentario))
                        {
                            concesion.Comentario = "N/A";
                        }
                        _context.Add(concesion);
                        await _context.SaveChangesAsync();
                        return Ok(new
                        {
                            isValid = true,
                            message = "Concesion agregada correctamente."
                        });
                    }
                    else
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(concesion.Comentario))
                            {
                                concesion.Comentario = "N/A";
                            }
                            _context.Update(concesion);
                            await _context.SaveChangesAsync();
                            return Ok(new
                            {
                                isValid = true,
                                message = "Solicitud actualizada correctamente."
                            });
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!ConcesionExists(id))
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
                html = Utils.RenderRazorViewToString(this, "AddOrEdit", concesion)
            });
        }


        // GET: Concesiones/Create
        public IActionResult Create()
        {
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "id", "id");
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Concesiones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoExpediente,Fecha,Remitente,Asunto,Asignado,FechaAsignacion,DepartamentoId,DiarenaNo,FechaEnviado,Comentario,Status,UsuarioId,Id,Active,Create,LastModifier")] Concesion concesion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(concesion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "id", "id", concesion.DepartamentoId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", concesion.UsuarioId);
            return View(concesion);
        }

        // GET: Concesiones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concesion = await _context.Concesiones.FindAsync(id);
            if (concesion == null)
            {
                return NotFound();
            }
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "id", "id", concesion.DepartamentoId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", concesion.UsuarioId);
            return View(concesion);
        }

        // POST: Concesiones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NoExpediente,Fecha,Remitente,Asunto,Asignado,FechaAsignacion,DepartamentoId,DiarenaNo,FechaEnviado,Comentario,Status,UsuarioId,Id,Active,Create,LastModifier")] Concesion concesion)
        {
            if (id != concesion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(concesion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConcesionExists(concesion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "id", "id", concesion.DepartamentoId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", concesion.UsuarioId);
            return View(concesion);
        }

        // GET: Concesiones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concesion = await _context.Concesiones
                .Include(c => c.Departamento)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (concesion == null)
            {
                return NotFound();
            }

            return View(concesion);
        }

        // POST: Concesiones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var concesion = await _context.Concesiones.FindAsync(id);
            _context.Concesiones.Remove(concesion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConcesionExists(int id)
        {
            return _context.Concesiones.Any(e => e.Id == id);
        }
    }
}
