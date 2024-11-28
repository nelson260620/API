using System.ComponentModel.DataAnnotations;

namespace ProyectSoftware.Web.DTOs
{
    public class DeleteAuthorDTO
    {
        [Required(ErrorMessage = "El campo {0} es obligatiorio")]
        public string StageName { get; set; }
    }
}
