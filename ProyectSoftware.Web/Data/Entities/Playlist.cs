using ProyectSoftware.Web.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectSoftware.Web.Data.Entities
{
    public class Playlist
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Song> Songs { get; set; }

        public Playlist()
        {
            Songs = new List<Song>();
        }

        public Playlist(string name)
        {
            Name = name;
            Songs = new List<Song>();
        }
    }
}

