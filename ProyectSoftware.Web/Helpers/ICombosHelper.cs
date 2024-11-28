using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectSoftware.Web.Data;

namespace ProyectSoftware.Web.Helpers
{
    public interface ICombosHelper
    {
        Task<IEnumerable<SelectListItem>> GetComboProyectSoftwareRolesAsync();
        public Task<IEnumerable<SelectListItem>> GetComboSections();
    }

    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _context;
        public CombosHelper(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboProyectSoftwareRolesAsync()
        {
            List<SelectListItem> list = await _context.ProyectSoftwareRoles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString(),
            }).ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un rol...]",
                Value = "0"
            });

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboSections()
        {
            throw new NotImplementedException();
        }
    }
}
