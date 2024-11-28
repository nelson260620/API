using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ProyectSoftware.Web.Data.Entities;
using ProyectSoftware.Web.Services;
using ProyectSoftware.Web.Core;
using Constants = ProyectSoftware.Web.Core.Constants;


namespace ProyectSoftware.Web.Data.Seeders
{
    public class UserRoleSeeder
    {
        private readonly IUsersService _usersService;
        private readonly DataContext _context;

        public UserRoleSeeder(IUsersService usersService, DataContext context)
        {
            _usersService = usersService;
            _context = context;
        }

        public async Task SeedAsync()
        {
            await CheckRolesAsync();
            await CheckUsers();
        }
        private async Task AdministradorRoleAsync()
        {
            ProyectSoftwareRole? tmp = await _context.ProyectSoftwareRoles.Where(ir => ir.Name == Constants.SUPER_ADMIN_ROLE_NAME).FirstOrDefaultAsync();

            if (tmp == null)
            {
                ProyectSoftwareRole role = new ProyectSoftwareRole { Name = Constants.SUPER_ADMIN_ROLE_NAME };
                _context.ProyectSoftwareRoles.Add(role);
                await _context.SaveChangesAsync();
            }
        }
        
         private async Task CreadorDeContenidoRoleAsync()
        {

            ProyectSoftwareRole? tmp = await _context.ProyectSoftwareRoles.Where(pbr => pbr.Name == "Creador de contenido")
                                                                  .FirstOrDefaultAsync();

            if (tmp == null)
            {
                ProyectSoftwareRole role = new ProyectSoftwareRole { Name = "Creador de contenido" };

                _context.ProyectSoftwareRoles.Add(role);

                //posible solucion?
                List<Permission> permissions = await _context.Permissions.Where(p => p.Module == "Roles" ||
                                                                           p.Module == "GenderTypes").ToListAsync();


                foreach (Permission permission in permissions)
                {
                    _context.RolePermissions.Add(new RolePermission { Role = role, Permission = permission });
                }
            }

            await _context.SaveChangesAsync();
        }
        private async Task GestorDeUsuariosRoleAsync()
        {

            ProyectSoftwareRole? tmp = await _context.ProyectSoftwareRoles.Where(pbr => pbr.Name == "Gestor de usuarios")
                                                                  .FirstOrDefaultAsync();

            if (tmp == null)
            {
                ProyectSoftwareRole role = new ProyectSoftwareRole { Name = "Gestor de usuarios" };

                _context.ProyectSoftwareRoles.Add(role);

                ////posible solucion?
                //List<Permission> permissions = await _context.Permissions.Where(p => p.Module == "Roles" ||
                //                                                           p.Module == "Authors" ||
                //                                                           p.Module == "GenderTypes").ToListAsync();

                List<Permission> permissions = await _context.Permissions.Where(p => p.Module == "Secciones").ToListAsync();

                foreach (Permission permission in permissions)
                {
                    _context.RolePermissions.Add(new RolePermission { Role = role, Permission = permission });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task CheckUsers()
        {
            // Administrador
            User? user = await _usersService.GetUserAsync("manueld@yopmail.com");

            ProyectSoftwareRole adminRole = _context.ProyectSoftwareRoles.Where(r => r.Name == "Administrador")
                                                                 .First();

            if (user is null)
            {
                user = new User
                {
                    Email = "manueld@yopmail.com",
                    FirstName = "Manuel Alejandro",
                    LastName = "Domínguez Guerrero",
                    PhoneNumber = "3000000000",
                    UserName = "manueld@yopmail.com",
                    Document = "1111",
                    ProyectSoftwareRole = adminRole,
                };

                await _usersService.AddUserAsync(user, "1234");

                string token = await _usersService.GenerateEmailConfirmationTokenAsync(user);
                await _usersService.ConfirmEmailAsync(user, token);
            }

            // Creador de contenido
            user = await _usersService.GetUserAsync("anad@yopmail.com");


            ProyectSoftwareRole creadorDeContenidoRole = await _context.ProyectSoftwareRoles.Where(pbr => pbr.Name == "Creador de contenido")
                                                                                    .FirstAsync();

            if (user == null)
            {
                user = new User
                {
                    Email = "anad@yopmail.com",
                    FirstName = "Ana",
                    LastName = "Doe",
                    PhoneNumber = "30000000",
                    UserName = "anad@yopmail.com",
                    Document = "2222",
                    ProyectSoftwareRole = creadorDeContenidoRole
                };

                await _usersService.AddUserAsync(user, "1234");

                string token = await _usersService.GenerateEmailConfirmationTokenAsync(user);
                await _usersService.ConfirmEmailAsync(user, token);
            }

            // Gestor de usuarios
            user = await _usersService.GetUserAsync("jhond@yopmail.com");

            ProyectSoftwareRole gestorDeUsuarios = await _context.ProyectSoftwareRoles.Where(pbr => pbr.Name == "Gestor de usuarios")
                                                                              .FirstAsync();

            if (user == null)
            {
                user = new User
                {
                    Email = "jhond@yopmail.com",
                    FirstName = "Jhon",
                    LastName = "Doe",
                    PhoneNumber = "30000000",
                    UserName = "jhond@yopmail.com",
                    Document = "3333",
                    ProyectSoftwareRole = gestorDeUsuarios
                };

                var result = await _usersService.AddUserAsync(user, "1234");

                string token = await _usersService.GenerateEmailConfirmationTokenAsync(user);
                await _usersService.ConfirmEmailAsync(user, token);
            }
        }

        private async Task CheckRolesAsync()
        {
            await AdministradorRoleAsync();
            await CreadorDeContenidoRoleAsync();
            await GestorDeUsuariosRoleAsync();
        }
    }
}
