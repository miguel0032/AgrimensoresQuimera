using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParcelaConsultingWeb.Data;
using ParcelaConsultingWeb.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParcelaConsultingWeb.ViewComponents
{
    [ViewComponent(Name = "Acciones")]
    public class AccionesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public AccionesViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var acciones = await (from x in _context.Acciones
                                  join t in _context.TipoAcciones on x.TipoAccionId equals t.Id
                                  where x.SolicitudId == id
                                  select new AccionViewModel
                                  {
                                      Id = x.Id,
                                      Nombre = x.Nombre,
                                      Descripcion = x.Descripcion,
                                      Create = x.Create.ToShortDateString(),
                                      TipoAccion = t.Name
                                  }).ToListAsync();
            return View("Index", acciones);
        }
    }
}
