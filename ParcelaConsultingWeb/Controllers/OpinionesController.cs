using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParcelaConsultingWeb.Data;
using ParcelaConsultingWeb.Extension;
using ParcelaConsultingWeb.Models;
using ParcelaConsultingWeb.Utility;
using ParcelaConsultingWeb.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace ParcelaConsultingWeb.Controllers
{
    public class OpinionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OpinionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Opiniones
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Opiniones.Include(o => o.Departamento).Include(o => o.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Opiniones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opiniones
                .Include(o => o.Departamento)
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (opinion == null)
            {
                return NotFound();
            }

            return View(opinion);
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

            var result = await (from x in _context.Opiniones
                                select new OpinionesListViewModel
                                {
                                    Id = x.Id,
                                    Expediente = x.Expediente,
                                    Solicitante = x.Solicitante,
                                    Asunto = x.Asunto,
                                    Asignados = x.Asignados,
                                    Departamento = x.Departamento.Name,
                                    Digitador = x.Digitador,
                                    Usuario = x.UsuarioId,
                                    fechaAsignado = x.fechaAsignado.ToShortDateString(),
                                    FechaRemision = x.FechaRemision.ToShortDateString(),
                                    FechaEntrada = x.FechaEntrada.ToShortDateString(),
                                    Status = x.Status,
                                }).ToListAsync();


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(r => r.Solicitante != null && r.Solicitante.ToUpper().Contains(searchBy.ToUpper()) ||
                                           r.Expediente != null && r.Expediente.ToUpper().Contains(searchBy.ToUpper())
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

        // GET: Opiniones/Create
        public IActionResult Create()
        {
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "id", "id");
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Opiniones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Expediente,FechaEntrada,Solicitante,Asunto,Asignados,fechaAsignado,Digitador,DepartamentoId,RemitidoDiarena,FechaRemision,Comentario,UsuarioId,Status,Id,Active,Create,LastModifier")] Opinion opinion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(opinion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "id", "id", opinion.DepartamentoId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", opinion.UsuarioId);
            return View(opinion);
        }

        // GET: Opiniones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opiniones.FindAsync(id);
            if (opinion == null)
            {
                return NotFound();
            }
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "id", "id", opinion.DepartamentoId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", opinion.UsuarioId);
            return View(opinion);
        }

        // POST: Opiniones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Expediente,FechaEntrada,Solicitante,Asunto,Asignados,fechaAsignado,Digitador,DepartamentoId,RemitidoDiarena,FechaRemision,Comentario,UsuarioId,Status,Id,Active,Create,LastModifier")] Opinion opinion)
        {
            if (id != opinion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(opinion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpinionExists(opinion.Id))
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
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "id", "id", opinion.DepartamentoId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", opinion.UsuarioId);
            return View(opinion);
        }

        // GET: Opiniones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opiniones
                .Include(o => o.Departamento)
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (opinion == null)
            {
                return NotFound();
            }

            return View(opinion);
        }

        // POST: Opiniones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opinion = await _context.Opiniones.FindAsync(id);
            _context.Opiniones.Remove(opinion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OpinionExists(int id)
        {
            return _context.Opiniones.Any(e => e.Id == id);
        }
    }
}
