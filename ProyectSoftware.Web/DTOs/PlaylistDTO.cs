using ProyectSoftware.Web.Data.Entities;

namespace ProjectSoftware.Web.DTOs
{
    public class PlaylistDTO
    {
        public PlaylistDTO()
        {
            Songs = new List<SongDTO>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<SongDTO> Songs { get; set; }
    }
}

