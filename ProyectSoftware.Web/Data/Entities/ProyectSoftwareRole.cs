using System.ComponentModel.DataAnnotations;

namespace ProyectSoftware.Web.Data.Entities
{
    public class ProyectSoftwareRole
    {
        public int Id { get; set; }

        [Display(Name = "Rol")]
        [MaxLength(64, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; } = null!;

        public IEnumerable<User> Users { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
