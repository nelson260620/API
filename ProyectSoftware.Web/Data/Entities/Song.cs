using System.ComponentModel.DataAnnotations;

namespace ProyectSoftware.Web.Data.Entities
{
    public class Song
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        public string Title { get; set; }

        [Required(ErrorMessage = "El artista es requerido")]
        public string Artist { get; set; }

        [Required(ErrorMessage = "La URL de audio es requerida")]
        public string AudioUrl { get; set; }

        [Required(ErrorMessage = "La URL de imagen es requerida")]
        public string ImageUrl { get; set; }



        // Relaciones con otras entidades
        public ICollection<HasSongPlaylist> HasSongPlaylists { get; set; }
        public ICollection<HasSongGender> HasSongGenders { get; set; }
        public ICollection<UserSong> UserSongs { get; set; }
    }
}
