using Microsoft.AspNetCore.Mvc;
using ProyectSoftware.Web.Data.Entities;
using ProyectSoftware.Web.Data;
using ProyectSoftware.Web.Services;
using ProyectSoftware.Web.Core;
using AspNetCoreHero.ToastNotification.Abstractions;
using ProyectSoftware.Web.Core.Atributes;



namespace ProyectSoftware.Web.Controllers
{
    public class GenderTypesController : Controller
    {
        private readonly DataContext _context;
        private readonly IGenderTypesService _GenderTypeService;
        private readonly INotyfService _notify;

        public GenderTypesController(DataContext context, IGenderTypesService GenderTypeService, INotyfService notify)
        {
            _context = context;
            _GenderTypeService = GenderTypeService; // Asigna el servicio de autores recibido al campo privado.
            _notify = notify;
        }
        [HttpGet]
        // Acción para mostrar la lista de autores.
        //click derecho - añador vista (debe tener el mismo nombre)
        [CustomAuthorize(permission: "showSections", module: "GenderType")]
        public async Task<IActionResult> Index()
        {

            Response<List<GenderType>> response = await _GenderTypeService.GetListAsync();

            // Devuelve una vista pasando la lista de autores como modelo.
            return View(response.Result);
        }
        [HttpGet]
        [CustomAuthorize(permission: "createGenderTypes", module: "Authors")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [CustomAuthorize(permission: "createGenderTypes", module: "Authors")]
        public async Task<IActionResult> Create(GenderType model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Si hay errores de validación, vuelve a cargar la vista de creación con los datos y mensajes de error.
                    _notify.Error("Bebe ajustar los errores de validacion");
                    return View(model);
                }//esta sucediendo un error aqui y no se porque

                Response<GenderType> Response = await _GenderTypeService.CreateAsync(model);

                if (Response.IsSuccess)
                {
                    _notify.Success(Response.Message);
                    return RedirectToAction(nameof(Index));

                }

                _notify.Error(Response.Message);
                return View(model);
            }
            catch (Exception ex) 
            {
                _notify.Error(ex.Message);
            }
            return View();
        }
        [HttpGet]
        [CustomAuthorize(permission: "showGenderTypes", module: "Authors")]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            Response<GenderType> response = await _GenderTypeService.GetOneAsync(id);

            if (response.IsSuccess)
            {
                return View(response.Result);
            }

            _notify.Error(response.Errors.First());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [CustomAuthorize(permission: "updateGenderTypes", module: "Authors")]
        public async Task<IActionResult> Edit(GenderType model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _notify.Error("Debe ajustar los errores de validación.");
                    return View(model);
                }
                
                Response<GenderType> response = await _GenderTypeService.EditAsync(model);

                if (response.IsSuccess)
                {
                    _notify.Success(response.Message);
                    return RedirectToAction(nameof(Index));
                }

                _notify.Error(response.Errors.First());
                return View(model);
            }
            catch (Exception ex)
            {
                _notify.Error(ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [CustomAuthorize(permission: "deleteGenderTypes", module: "Authors")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            Response<GenderType> response = await _GenderTypeService.DeleteAsync(id);

            if (response.IsSuccess)
            {
                _notify.Success(response.Message);
                return RedirectToAction(nameof(Index));
            }

            _notify.Error(response.Errors.First());
            return RedirectToAction(nameof(Index));
        }

        //[HttpPost]
        //public async Task<IActionResult> Toggle(int Id, bool Hide)
        //{
        //    ToggleSectionRequest request = new ToggleSectionRequest { Id = Id, Hide = Hide };
        //    Response<Section> response = await _sectionsService.ToggleSectionAsync(request);

        //    _notify.Success("Sección actualizada con éxito");

        //    return RedirectToAction(nameof(Index));
        //}
    }
}
