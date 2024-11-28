using System.ComponentModel.DataAnnotations;

namespace ProjectSoftware.Web.DTOs
{
    public class SongDTO
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        public string Title { get; set; }

        [Required(ErrorMessage = "El artista es requerido")]
        public string Artist { get; set; }

        public string AudioUrl { get; set; }
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Please select an audio file.")]
        [DataType(DataType.Upload)]
        public IFormFile AudioFile { get; set; }

        [Required(ErrorMessage = "Please select an image file.")]
        [DataType(DataType.Upload)]
        public IFormFile ImageFile { get; set; }

    }
}
