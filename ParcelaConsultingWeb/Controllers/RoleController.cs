using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParcelaConsultingWeb.Data;
using ParcelaConsultingWeb.Models;
using ParcelaConsultingWeb.Utility;
using ParcelaConsultingWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParcelaConsultingWeb.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext context;

        public RoleController(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<IActionResult> Index()
        {
            var Roles = await (from x in context.Roles
                               select x).ToListAsync();
            return View(Roles);
        }

        public IActionResult Entities()
        {
            var entities = context.Permissions.ToList();
            return View(entities);
        }

        //GET: Measure/Partial:5
        //GET: Measure/Edit:5
        public IActionResult GetPartialRoles(string id)
        {
            if (string.IsNullOrEmpty(id))
                return PartialView("_RolesAddOrEdit", new Role());
            else
                return PartialView("_RolesAddOrEdit", context.Roles.Find(id));
        }

        //POST: Measure
        public async Task<IActionResult> AddOrEdit(Role rol)
        {
            if (ModelState.IsValid)
            {
                var role = await context.Roles.FindAsync(rol.Id);
                if (role == null)
                {
                    var rolExist = context.Roles
                        .Where(x => x.Name.ToLower().Equals(rol.Name.ToLower()))
                        .ToList();
                    if (rolExist.Count != 0)
                    {
                        return Json(new
                        {
                            isValid = false,
                            html = Utils.RenderRazorViewToString(this, "Index", rol)
                        });
                    }
                    rol.NormalizedName = rol.Name.ToUpper();
                    context.Add(rol);

                }
                else
                {
                    context.Update(rol);
                }
                await context.SaveChangesAsync();
                return Json(new
                {
                    isValid = true,
                    html = Utils.RenderRazorViewToString(this, "Index", context.Roles.ToListAsync())
                });
            }
            return View(rol);
        }

        // GET: TypeOfVisitor/Delete/5
        public async Task<IActionResult> RoleDelete(string id)
        {
            var status = false;

            //Valida si hay Usuario asociados.
            var userRole = await (from x in context.Users
                                  join ur in context.UserRoles on x.Id equals ur.UserId
                                  join r in context.Roles on ur.RoleId equals r.Id
                                  where r.Id == id
                                  select x).ToListAsync();


            if (userRole.Count != 0)
            {
                status = false;
                return Json(status);
            }
            else
            {
                var rolDelete = await context.Roles.FindAsync(id);
                context.Roles.Remove(rolDelete);
                await context.SaveChangesAsync();
                status = true;
            }

            return Json(status);
        }

        public IActionResult CreateEntity(int id = 0)
        {
            if (id == 0)
                return PartialView("_EntityAddOrEdit", new Role());
            else
                return PartialView("_EntityAddOrEdit", context.Roles.Find(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEntity(int id, Permission permission)
        {
            if (permission == null)
            {
                return Json(new
                {
                    isValid = false,
                    html = Utils.RenderRazorViewToString(this, "CreateEntity", permission)
                });
            }

            if (id == 0)
            {
                context.Permissions.Add(permission);
            }
            else
            {
                context.Permissions.Update(permission);
            }

            await context.SaveChangesAsync();

            return Json(new
            {
                isValid = true,
                html = Utils.RenderRazorViewToString(this, "Index", context.Roles.ToListAsync())
            });
        }

        public async Task<IActionResult> AddEntitiesToRole(string roleId)
        {
            List<PermissionByRole> permissionByRoles = new List<PermissionByRole>();
            var entities = context.Permissions.Select(x => x.Id).ToList();

            foreach (var item in entities)
            {
                var permissionRoleExist = await PermissionRoleExist(roleId, item);
                if (!permissionRoleExist)
                {
                    var pr = new PermissionByRole();
                    pr.RoleId = roleId;
                    pr.PermissionId = item;
                    pr.LastModified = DateTime.Now;
                    pr.Id = Convert.ToString(Guid.NewGuid());
                    pr.Create = false;
                    pr.Read = false;
                    pr.Update = false;
                    pr.Delete = false;
                    permissionByRoles.Add(pr);
                }
            }

            if (permissionByRoles.Count != 0)
            {
                context.PermissionByRoles.AddRange(permissionByRoles);
            }
            await context.SaveChangesAsync();

            return RedirectToAction("GetEntitiesByRole", new { @roleId = roleId });
        }

        public async Task<bool> PermissionRoleExist(string roleid, int permissionId)
        {
            var existEntity = await context.PermissionByRoles
                .Where(x => x.RoleId.Equals(roleid) && x.PermissionId == permissionId)
                .FirstOrDefaultAsync();
            if (existEntity != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IActionResult> GetEntitiesByRole(string roleId)
        {
            var entities = await (from p in context.Permissions
                                  join pr in context.PermissionByRoles on p.Id equals pr.PermissionId
                                  join r in context.Roles on pr.RoleId equals r.Id
                                  where pr.RoleId == roleId
                                  select new EntitiesByRole
                                  {
                                      Id = pr.Id,
                                      RoleId = r.Id,
                                      RoleName = r.Name,
                                      EntityName = p.Entity,
                                      Create = pr.Create,
                                      Read = pr.Read,
                                      Update = pr.Update,
                                      Delete = pr.Delete
                                  }).ToListAsync();

            ViewBag.roleName = entities.Select(x => x.RoleName).FirstOrDefault();
            ViewBag.roleId = roleId;

            return View(entities);

        }

        [HttpPost]
        public async Task<IActionResult> EntityCrud(string id, string val, bool item)
        {
            var roleId = "";
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(val))
            {
                var entity = context.PermissionByRoles
                    .Where(x => x.Id.Equals(id))
                    .FirstOrDefault();
                roleId = entity.RoleId;

                if (val == "Create")
                {
                    if (item == false)
                    {
                        entity.Create = true;
                    }
                    else
                    {
                        entity.Create = false;
                    }

                }
                else if (val == "Read")
                {
                    if (item == false)
                    {
                        entity.Read = true;
                    }
                    else
                    {
                        entity.Read = false;
                    }
                }
                else if (val == "Update")
                {
                    if (item == false)
                    {
                        entity.Update = true;
                    }
                    else
                    {
                        entity.Update = false;
                    }
                }
                else if (val == "Delete")
                {
                    if (item == false)
                    {
                        entity.Delete = true;
                    }
                    else
                    {
                        entity.Delete = false;
                    }
                }

                context.PermissionByRoles.Update(entity);
                await context.SaveChangesAsync();

                return Json(new
                {
                    isValid = true,
                    html = Utils.RenderRazorViewToString(this, "GetEntitiesByRole", context.PermissionByRoles.Where(x => x.RoleId == entity.RoleId).ToListAsync())
                });
            }

            return Json(new
            {
                isValid = true,
                html = Utils.RenderRazorViewToString(this, "GetEntitiesByRole", context.PermissionByRoles.Where(x => x.RoleId == roleId).ToListAsync())
            });
        }
    }
}
