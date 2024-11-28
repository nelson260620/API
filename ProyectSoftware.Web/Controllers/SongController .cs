using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ProjectSoftware.Web.DTOs;
using ProyectSoftware.Web.Data;
using ProyectSoftware.Web.Data.Entities;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

namespace ProyectSoftware.Web.Controllers
{
    public class SongController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public SongController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var songs = await _context.Songs
                .Select(s => new SongDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    Artist = s.Artist,
                    AudioUrl = s.AudioUrl,
                    ImageUrl = s.ImageUrl
                }).ToListAsync();

            return View(songs);
        }

        [HttpGet]
        // GET: Song/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Song/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SongDTO dto)
        {
            if (ModelState.IsValid)
            {
                string audioFileName = null;
                string imageFileName = null;

                if (dto.AudioFile != null)
                {
                    // procesa la audio le pone nombre distintivo y crea una ruta para ella
                    audioFileName = Path.GetFileNameWithoutExtension(dto.AudioFile.FileName);
                    string audioExtension = Path.GetExtension(dto.AudioFile.FileName);
                    audioFileName = audioFileName + "_" + Path.GetRandomFileName() + audioExtension;
                    string audioPath = Path.Combine(_hostEnvironment.WebRootPath, "audio", audioFileName);
                    using (FileStream fileStream = new FileStream(audioPath, FileMode.Create))
                    {
                        await dto.AudioFile.CopyToAsync(fileStream);
                    }
                }

                if (dto.ImageFile != null)
                {
                    //procesa la imagen le pone nombre distintivo y crea una ruta para ella 
                    imageFileName = Path.GetFileNameWithoutExtension(dto.ImageFile.FileName);
                    string imageExtension = Path.GetExtension(dto.ImageFile.FileName);
                    imageFileName = imageFileName + "_" + Path.GetRandomFileName() + imageExtension;
                    string imagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", imageFileName);
                    using (FileStream fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await dto.ImageFile.CopyToAsync(fileStream);
                    }
                }

                Song song = new Song
                {
                    Title = dto.Title,
                    Artist = dto.Artist,
                    AudioUrl = audioFileName != null ? "/audio/" + audioFileName : null,

                    ImageUrl = imageFileName != null ? "/images/" + imageFileName : null

                };

                await _context.Songs.AddAsync(song);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Song? song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            SongDTO songDTO = new SongDTO
            {
                Id = song.Id,
                Title = song.Title,
                Artist = song.Artist,
                AudioUrl = song.AudioUrl,
                ImageUrl = song.ImageUrl
            };

            return View(songDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SongDTO songDTO)
        {
            if (id != songDTO.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Song? song = await _context.Songs.FindAsync(id);

                    if (song == null)
                    {
                        return NotFound();
                    }

                    string audioPath = song.AudioUrl;
                    string imagePath = song.ImageUrl;

                    if (songDTO.AudioFile != null && songDTO.AudioFile.Length > 0)
                    {
                        string audioFileName = Path.GetFileName(songDTO.AudioFile.FileName);
                        string audioFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/audio", audioFileName);
                        using (var stream = new FileStream(audioFilePath, FileMode.Create))
                        {
                            await songDTO.AudioFile.CopyToAsync(stream);
                        }
                        audioPath = "/audio/" + audioFileName;
                    }

                    if (songDTO.ImageFile != null && songDTO.ImageFile.Length > 0)
                    {
                        string imageFileName = Path.GetFileName(songDTO.ImageFile.FileName);
                        string imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imageFileName);
                        using (var stream = new FileStream(imageFilePath, FileMode.Create))
                        {
                            await songDTO.ImageFile.CopyToAsync(stream);
                        }
                        imagePath = "/images/" + imageFileName;
                    }

                    song.Title = songDTO.Title;
                    song.Artist = songDTO.Artist;
                    song.AudioUrl = audioPath;
                    song.ImageUrl = imagePath;

                    _context.Update(song);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(songDTO.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(songDTO);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Song? song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }
    }
}
