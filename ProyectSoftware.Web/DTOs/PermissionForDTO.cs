using ProyectSoftware.Web.Data.Entities;

namespace ProyectSoftware.Web.DTOs
{
    public class PermissionForDTO : Permission
    {
        public bool Selected { get; set; } = false;
    }
}
