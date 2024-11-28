using ProjectSoftware.Web.Data.Entities;

namespace ProyectSoftware.Web.Data.Entities
{
    public class HasSongPlaylist
    {
        public int IdSong { get; set; }
        public Song Song { get; set; }

        public int IdPlaylist { get; set; }
        public Playlist Playlist { get; set; }
    }
}
