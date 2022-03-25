using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParcelaConsultingWeb.Data;
using ParcelaConsultingWeb.Models;
using ParcelaConsultingWeb.Utility;
using System.Linq;
using System.Threading.Tasks;

namespace ParcelaConsultingWeb.Controllers
{
    public class MunicipiosController : Controller
    {
        private ApplicationDbContext context;

        public MunicipiosController(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ProvinciaId"] = new SelectList(context.Provincias, "ProvinciaId", "Name");
            return View(await context.Municipios.ToListAsync());
        }

        public IActionResult GetPartialMunicipio(int id = 0)
        {
            ViewData["ProvinciaId"] = new SelectList(context.Provincias, "ProvinciaId", "Name");
            if (id == 0)
                return PartialView("_MunicipioAddOrEdit", new Municipio());
            else
                return PartialView("_MunicipioAddOrEdit", context.Municipios.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MunicipioAddOrEdit(int id, Municipio municipio)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var ivaExist = context.Municipios
                        .Where(x => x.Name.ToLower().Contains(municipio.Name.ToLower())).ToList();
                    if (ivaExist.Count != 0)
                    {
                        ViewData["ProvinciaId"] = new SelectList(context.Provincias, "ProvinciaId", "Name", municipio.ProvinciaId);
                        return Json(new
                        {
                            isValid = false,
                            message = "Este Municipio ya existe!",
                            html = Utils.RenderRazorViewToString(this, "Index", municipio)
                        });
                    }
                    context.Add(municipio);
                }
                else
                {
                    context.Update(municipio);
                }
                await context.SaveChangesAsync();
                //ViewData["ProvinciaId"] = new SelectList(context.Provincias, "ProvinciaId", "Name");
                //return Json(new
                //{
                //    isValid = true,
                //    message = "Municipio agregado correctamente!",
                //    html = Utils.RenderRazorViewToString(this, "Index", context.Municipios.ToListAsync())
                //});
                return RedirectToAction(nameof(Index));
            }
            return View(municipio);
        }

        public async Task<IActionResult> MunicipioDelete(int id)
        {
            var municipioDelete = await context.Municipios.FindAsync(id);
            var dt = await context.Solicitudes.Where(x => x.MunicipioId == id).FirstOrDefaultAsync();
            if (dt != null)
            {
                ViewData["ProvinciaId"] = new SelectList(context.Provincias, "ProvinciaId", "Name", municipioDelete.ProvinciaId);
                return Json(new
                {
                    isValid = false,
                    message = "El Municipio esta ligado, no se puede eliminar!",
                    html = Utils.RenderRazorViewToString(this, "Index", context.Municipios.ToListAsync())
                });
            }

            if (municipioDelete != null)
            {
                context.Municipios.Remove(municipioDelete);
                await context.SaveChangesAsync();
            }
            ViewData["ProvinciaId"] = new SelectList(context.Provincias, "ProvinciaId", "Name", municipioDelete.ProvinciaId);
            return Json(new
            {
                isValid = true,
                message = "El Municipio ha sido eliminado!",
                html = Utils.RenderRazorViewToString(this, "Index", context.Municipios.ToListAsync())
            });
        }
    }
}
