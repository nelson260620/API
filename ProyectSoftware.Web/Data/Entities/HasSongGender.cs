namespace ProyectSoftware.Web.Data.Entities
{
    public class HasSongGender
    {
        public int IdSong { get; set; }
        public Song Song { get; set; }

        public int IdGender { get; set; }
        public GenderType GenderType { get; set; }
    }
}
