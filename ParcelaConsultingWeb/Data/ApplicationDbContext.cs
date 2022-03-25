using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParcelaConsultingWeb.Models;

namespace ParcelaConsultingWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string,
        IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public virtual DbSet<Concesion> Concesiones { get; set; }
        public virtual DbSet<Departamento> Departamentos { get; set; }
        public virtual DbSet<Municipio> Municipios { get; set; }
        public virtual DbSet<Opinion> Opiniones { get; set; }
        public virtual DbSet<Provincia> Provincias { get; set; }
        public virtual DbSet<Solicitud> Solicitudes { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionByRole> PermissionByRoles { get; set; }
        public virtual DbSet<Accion> Acciones { get; set; }
        public virtual DbSet<TipoAccion> TipoAcciones { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PermissionByRole>(entity =>
            {
                entity.HasKey(pr => new { pr.PermissionId, pr.RoleId });

                entity.HasOne(e => e.Role)
                      .WithMany(e => e.Permissions)
                      .HasForeignKey(eu => eu.RoleId);

                entity.HasOne(e => e.Permission)
                      .WithMany(e => e.Permissions)
                      .HasForeignKey(eu => eu.PermissionId);
            });

            modelBuilder.Entity<User>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne()
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(eu => eu.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<Role>(e =>
            {
                e.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(eu => eu.RoleId)
                .IsRequired();
            });

            modelBuilder.Entity<Solicitud>(s =>
            {
                s.HasKey(x => x.Id);
                s.Property(x => x.Id).IsRequired();

                s.Property(x => x.Parcelas).IsRequired();
                s.Property(x => x.Cantidad_parc).IsRequired();
                s.Property(x => x.Beneficiario).IsRequired();
                s.Property(x => x.NumeroDiarena).IsRequired();

                s.HasOne(x => x.Departamento)
                                 .WithMany(x => x.Solicitudes)
                                 .HasForeignKey(x => x.DepartamentoId);

                s.HasOne(x => x.Usuario)
                                 .WithMany(x => x.Solicitudes)
                                 .HasForeignKey(x => x.UsuarioId);

                s.HasOne(x => x.Municipio)
                                 .WithMany(x => x.Solicitudes)
                                 .HasForeignKey(x => x.MunicipioId)
                                 .IsRequired();

            });

            modelBuilder.Entity<Opinion>(s =>
            {
                s.HasKey(x => x.Id);
                s.Property(x => x.Asunto).IsRequired();
            });

            modelBuilder.Entity<Concesion>(s =>
            {
                s.HasKey(x => x.Id);
                s.Property(x => x.Asunto).IsRequired();
            });

            modelBuilder.Entity<Municipio>(m =>
            {
                m.HasKey(m => m.MunicipioId);
                m.Property(m => m.MunicipioId).IsRequired();

                m.HasOne(p => p.Provincia)
                 .WithMany(m => m.Municipios)
                 .HasForeignKey(p => p.ProvinciaId)
                 .IsRequired();
            });

            modelBuilder.Entity<Provincia>(m =>
            {
                m.HasKey(m => m.ProvinciaId);
                m.Property(p => p.ProvinciaId).IsRequired();

            });

        }

        private void Seed(ModelBuilder builder)
        {
            var usuarioAdmin = new User()
            {
                Email = "usuarioAdmin@hotmail.com",
                NormalizedEmail = "USUARIOADMIN@HOTMAIL.COM",
                UserName = "usuarioAdmin",
                NormalizedUserName = "USUARIOADMIN"
            };

            var usuarioVendedor = new User()
            {
                Email = "usuarioVendedor@hotmail.com",
                NormalizedEmail = "USUARIOVENDEDOR@HOTMAIL.COM",
                UserName = "usuarioVendedor",
                NormalizedUserName = "USUARIOVENDEDOR"
            };

            var usuarioSinRol = new User()
            {
                Email = "usuarioSinRol@hotmail.com",
                NormalizedEmail = "USUARIOSINROL@HOTMAIL.COM",
                UserName = "usuarioSinRol",
                NormalizedUserName = "USUARIOSINROL"
            };

            var rolAdmin = new Role()
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            };

            var rolVendedor = new Role()
            {
                Name = "Vendedor",
                NormalizedName = "VENDEDOR"
            };

            builder.Entity<Role>().HasData(rolAdmin, rolVendedor);
            builder.Entity<User>().HasData(usuarioAdmin, usuarioVendedor, usuarioSinRol);

            var userRoleAdmin = new UserRole()
            {
                RoleId = rolAdmin.Id,
                UserId = usuarioAdmin.Id
            };

            var userRoleVendedor = new UserRole()
            {
                RoleId = rolVendedor.Id,
                UserId = usuarioVendedor.Id
            };

            builder.Entity<UserRole>().HasData(userRoleAdmin, userRoleVendedor);
        }
    }
}
