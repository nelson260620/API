using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using ProyectSoftware.Web.Core;
using ProyectSoftware.Web.Core.Atributes;
using ProyectSoftware.Web.Data;
using ProyectSoftware.Web.Data.Entities;
using ProyectSoftware.Web.DTOs;
using ProyectSoftware.Web.Helpers;
using ProyectSoftware.Web.Services;

namespace ProyectSoftware.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        private readonly IUsersService _UsersService;
        private readonly ICombosHelper _combosHelper;
        private readonly INotyfService _notify;

        public UsersController(DataContext context, IUsersService UserService, INotyfService notify, ICombosHelper combosHelper)
        {
            _context = context;
            _UsersService = UserService;
            _notify = notify;
            _combosHelper = combosHelper;
        }

        [HttpGet]
        [CustomAuthorize(permission: "showUsers", module: "Usuarios")]
        public async Task<IActionResult> Index()
        {
            _notify.Success("Users");
            // Obtiene la lista de autores de forma asincrónica desde el servicio de autores.

            Response<List<User>> response = await _UsersService.GetListAsync();

            // Devuelve una vista pasando la lista de autores como modelo.
            return View(response.Result);
        }

        [HttpGet]
        [CustomAuthorize(permission: "createUsers", module: "Usuarios")]
        public async Task<IActionResult> Create()
        {
            return View(new UserDTO
            {
                IsNew = true,
                ProyectSoftwareRole = await _combosHelper.GetComboProyectSoftwareRolesAsync()
            });
        }

        [HttpPost]
        [CustomAuthorize(permission: "createUsers", module: "Usuarios")]
        public async Task<IActionResult> Create(UserDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notify.Error("Debe ajustar los errores de validación.");
                dto.ProyectSoftwareRole = await _combosHelper.GetComboProyectSoftwareRolesAsync();
                return View(dto);
            }

            Response<User> response = await _UsersService.CreateAsync(dto);

            if (response.IsSuccess)
            {
                _notify.Success(response.Message);
                return RedirectToAction(nameof(Index));
            }

            _notify.Error(response.Message);
            dto.ProyectSoftwareRole = await _combosHelper.GetComboProyectSoftwareRolesAsync();
            return View(dto);
        }

    }
}
