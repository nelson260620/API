using System.ComponentModel.DataAnnotations;

namespace ProyectSoftware.Web.Data.Entities
{
    public class GenderType
    {
        [Key]
        [Display(Name = "Id")]
        [Required(ErrorMessage = "El campo '{0}' es requerido.")]
        public int Id { get; set; }

        [Display(Name ="GenderType")]
        [Required(ErrorMessage = "El campo '{0}' es requerido.")]
        [MaxLength(64, ErrorMessage = "El campo '{0}' debe terner máximo {1} caractéres")]
        public string GenderName { get; set; }

      
        public ICollection<HasSongGender> HasSongGenders { get; set; }
    }
}
