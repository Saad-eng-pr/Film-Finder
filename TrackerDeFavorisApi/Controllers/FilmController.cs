
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using TrackerDeFavorisApi.Models;

public class FilmContext : DbContext
{
    public FilmContext( DbContextOptions<FilmContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring ( DbContextOptionsBuilder options )
    {
        options.UseSqlite("Data Source=Film.db");
    }
    public DbSet<Film> Films { get; set; } = null!;
}

namespace TrackerDeFavorisApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmController : ControllerBase
    {
        private readonly FilmContext _context;
        public FilmController(FilmContext ctx)
        {
            _context = ctx; 
        }

        // GET: api/<FilmController>
        [HttpGet("Get All Films")]
        public async Task<ActionResult<IEnumerable<Film>>> GetFilms()
        {
            var films =  await _context.Films.ToListAsync();
            if (films == null)
            {
                return NotFound("No films found");
            }
            return Ok(films);
        }

        // GET /api/Film/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Film>>> Search(string title)
        {
            string normalTitle = title.ToLower();
            if (String.IsNullOrEmpty(normalTitle))
                return NotFound("No title found");
            
            var films = await _context.Films.Where(f => f.Title != null && f.Title.ToLower().Contains(title)).ToListAsync();
            
            if (films == null)
                return NotFound("No films found");
            
            return Ok(films);
        }

         // GET /api/Film/search
        [HttpGet("searchID")]
        public async Task<ActionResult<IEnumerable<Film>>> SearchID(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film == null)
                return NotFound("No films found");
            
            return Ok(film);
        }

        // GET /api/Film/info
        [HttpGet("info")]
        public async Task<ActionResult<IEnumerable<Film>>> GetInfo([FromQuery] int[] ids)
        { 
            if (ids == null || ids.Count() == 0)
            {
                return NotFound("No ids found");
            }

            var films = await _context.Films.Where(f => ids.Contains(f.Id)).ToListAsync();
            
            if (films == null || films.Count() == 0)
            {
                return NotFound("No films found");
            }
            return Ok(films);
        }

        // POST /api/Film
        [HttpPost("Add movies")]
        public async Task<ActionResult<Film>> PostFilm(Film film)
        {
            Film film1 = new Film
            {
                Id = film.Id,
                Title = film.Title,
                Poster = film.Poster,
                imdbID = film.imdbID,
                Year = film.Year
            };
            _context.Films.Add(film1);
            await _context.SaveChangesAsync();
            return Ok(film1);
        }

        
    }
}
