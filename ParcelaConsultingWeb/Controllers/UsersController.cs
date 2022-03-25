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
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ParcelaConsultingWeb.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;


        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UsersController
        public ActionResult Index()
        {
            return View();
        }

        //GetById
        public async Task<IActionResult> GetById(string id)
        {
            var res = await _context.Users
                .Where(x => x.Id == id)
                .ToListAsync();

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> GetAllUsers([FromBody] DtParameters dtParameters)
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

            var result = await (from x in _context.Users
                                join ur in _context.UserRoles on x.Id equals ur.UserId
                                join r in _context.Roles on ur.RoleId equals r.Id
                                select new UserViewModel
                                {
                                    Id = x.Id,
                                    UserName = x.UserName,
                                    FullName = x.FullName,
                                    PhoneNumber = x.PhoneNumber,
                                    Rol = r.Name,
                                    Email = x.Email,
                                    LockoutEnabled = x.LockoutEnabled,
                                }).ToListAsync();


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(r => r.UserName != null && r.UserName.ToUpper().Contains(searchBy.ToUpper()) ||
                                           r.FullName != null && r.FullName.ToUpper().Contains(searchBy.ToUpper()) ||
                                           r.Rol != null && r.Rol.ToUpper().Contains(searchBy.ToUpper()) ||
                                           r.Email != null && r.Email.ToUpper().Contains(searchBy.ToUpper())
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


        //AddOrEdit
        public IActionResult AddOrEdit(string id = "")
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            var user = _context.Users
                ;
            if (string.IsNullOrEmpty(id))
                return PartialView("AddOrEdit", new EUserViewModel());
            else
                return PartialView("AddOrEdit", user);
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(string id, EUserViewModel user, string confirmacion)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { isValid = true, html = Utils.RenderRazorViewToString(this, "AddOrEdit", user) });
            }
            else
            {
                try
                {
                    User iUser = new User();


                    if (id == null)
                    {
                        iUser.Id = Convert.ToString(Guid.NewGuid());
                        iUser.UserName = user.UserName;
                        iUser.FullName = user.FullName;
                        iUser.PhoneNumber = user.PhoneNumber;
                        iUser.PhoneNumberConfirmed = true;
                        iUser.EmailConfirmed = true;
                        iUser.Email = user.Email;
                        iUser.NormalizedEmail = user.Email.ToUpper();
                        iUser.LockoutEnabled = false;
                        iUser.PasswordHash = HashPassword(user.Password);
                        iUser.NormalizedUserName = user.UserName.ToUpper();
                        var confirmPass = VerifyHashedPassword(iUser.PasswordHash, confirmacion);
                        if (!confirmPass)
                        {
                            return Json(new
                            {
                                isValid = true,
                                message = "La Contraseña no coincide",
                                html = Utils.RenderRazorViewToString(this, "AddOrEdit", user)
                            });
                        }
                        var userRole = new UserRole()
                        {
                            UserId = iUser.Id,
                            RoleId = user.Rol
                        };

                        _context.Add(iUser);

                        if (userRole != null)
                        {
                            _context.UserRoles.Add(userRole);
                        }
                        else
                        {
                            return Json(new
                            {
                                isValid = false,
                                message = "No se pudo guardar el rol",
                                html = Utils.RenderRazorViewToString(this, "AddOrEdit", user)
                            });
                        }
                    }
                    else
                    {
                        _context.Update(user);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw new Exception($"{e}");
                }

                //ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.Id);
                //return Json(new { isValid = true,
                //                  message = "Usuario Agregado satisfactoriamente!",
                //                  html = Utils.RenderRazorViewToString(this, "Index", _context.Users.ToListAsync()) });

                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult GetRoles(string term = "")
        {
            List<SelectRoleViewModel> roles = new List<SelectRoleViewModel>();
            roles = (from x in _context.Roles
                     select new SelectRoleViewModel
                     {
                         Id = x.Id,
                         Name = x.Name
                     }).ToList();

            if (!string.IsNullOrEmpty(term))
            {
                roles = roles.Where(x => x.Name.ToLower().Contains(term.ToLower())).ToList();
            }

            return Ok(roles);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var municipioDelete = await _context.Users.FindAsync(id);
            var dt = await _context.Solicitudes.Where(x => x.UsuarioId == id).FirstOrDefaultAsync();
            if (dt != null)
            {
                return Json(new
                {
                    isValid = false,
                    message = "El Usuario ha creado registros, no se puede eliminar!",
                    html = Utils.RenderRazorViewToString(this, "Index", _context.Users.ToListAsync())
                });
            }

            if (municipioDelete != null)
            {
                _context.Users.Remove(municipioDelete);
                await _context.SaveChangesAsync();
            }

            return Json(new
            {
                isValid = true,
                message = "El Usuario ha sido eliminado!",
                html = Utils.RenderRazorViewToString(this, "Index", _context.Users.ToListAsync())
            });
        }

        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
        }

        public static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            var areSame = true;

            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }
    }
}
