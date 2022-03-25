using Microsoft.AspNetCore.Mvc;
using ParcelaConsultingWeb.Data;
using ParcelaConsultingWeb.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace ParcelaConsultingWeb.Controllers
{
    public class BaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<MunicipioViewModel> GetMunicipios(string term = null)
        {
            var municipio = (from m in _context.Municipios
                             join p in _context.Provincias on m.ProvinciaId equals p.ProvinciaId
                             select new MunicipioViewModel
                             {
                                 Id = m.MunicipioId,
                                 Name = p.Name + " - " + m.Name
                             }).ToList();

            if (!string.IsNullOrEmpty(term))
            {
                municipio = municipio.Where(x => x.Name.ToLower().Contains(term.ToLower())).ToList();
            }
            return municipio;
        }

        public List<DepartamentoViewModel> GetDepartamentos(string term = null)
        {
            var municipio = (from m in _context.Departamentos
                             select new DepartamentoViewModel
                             {
                                 Id = m.id,
                                 Name = m.Name
                               
                             }).ToList();

            if (!string.IsNullOrEmpty(term))
            {
                municipio = municipio.Where(x => x.Name.ToLower().Contains(term.ToLower())).ToList();
            }
            return municipio;
        }

        public List<UsuarioSelectViewModel> GetUsuarioLogger(string term = null)
        {
            var municipio = (from m in _context.Users
                             select new UsuarioSelectViewModel
                             {
                                 Id = m.Id,
                                 Name = m.FullName
                             }).ToList();

            if (!string.IsNullOrEmpty(term))
            {
                municipio = municipio.Where(x => x.Name.ToLower().Contains(term.ToLower())).ToList();
            }
            return municipio;
        }


        //- PIE CHART1//
        public IActionResult TipoGeneradorPie()
        {
            TipoGeneradorViewModel model = new TipoGeneradorViewModel();
            model.Micro = _context.Solicitudes.Count();
            model.Peq = _context.Concesiones.Count();
            model.Gran = _context.Opiniones.Count();

            return Ok(model);

        }


        //- PIE CHART2//
        public IActionResult Data()
        {
            TipoGeneradorViewModel model = new TipoGeneradorViewModel();
            model.Micro = _context.Solicitudes.Count();
            model.Peq = _context.Concesiones.Count();
            model.Gran = _context.Solicitudes.Count();

            return Ok(model);
        }


        //- PIE CHART3//
        public IActionResult Data3()
        {
            TipoGeneradorViewModel model = new TipoGeneradorViewModel();
            model.Micro = _context.Provincias.Count();
            model.Peq = _context.Municipios.Count();
            model.Gran = _context.Users.Count();

            return Ok(model);
        }


    }
}
