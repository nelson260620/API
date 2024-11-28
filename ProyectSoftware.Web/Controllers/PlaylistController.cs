using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSoftware.Web.Data;
using ProjectSoftware.Web.Data.Entities;
using ProjectSoftware.Web.DTOs;
using ProyectSoftware.Web.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSoftware.Web.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly DataContext _context;

        public PlaylistController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var playlists = await _context.Playlists
                .Include(p => p.Songs)
                .Select(p => new PlaylistDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Songs = p.Songs.Select(s => new SongDTO { Title = s.Title, Artist = s.Artist }).ToList()
                })
                .ToListAsync();

            return View(playlists);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PlaylistDTO playlistDTO)
        {
            if (ModelState.IsValid)
            {
                var playlist = new Playlist(playlistDTO.Name);
                _context.Playlists.Add(playlist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(playlistDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist != null)
            {
                _context.Playlists.Remove(playlist);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
