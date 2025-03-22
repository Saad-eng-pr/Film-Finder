
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using TrackerDeFavorisApi.Models;

public class FavorisContext : DbContext
{
    public FavorisContext( DbContextOptions<FavorisContext> options) 
        : base(options)
    {
    }

    protected override void OnConfiguring( DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=Favoris.db");
    }
    public DbSet<Favoris> Favoris { get; set; } = null!;
}

namespace TrackerDeFavorisApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavorisController : ControllerBase
    {
        public readonly FavorisContext _context;
        
        public readonly FilmContext _filmContext;

        public FavorisController(FavorisContext ctx, FilmContext filmContext)
        {
            _context = ctx;
            _filmContext = filmContext;
        }

        // POST /api/Favoris
       [HttpPost("Add")]
        public async Task<ActionResult<Favoris>> PostFavoris(int userId, int filmId)
        {
            // Vérifier si le film existe
            if (_filmContext.Films.Any(f => f.Id == filmId))
            {
                // Vérifier si le film est déjà dans les favoris de cet utilisateur
                if (_context.Favoris.Any(f => f.UserId == userId && f.FilmId == filmId))
                {
                    return BadRequest("Film already in favorites");
                }

                // Ajouter un favori pour l'utilisateur
                Favoris favoris = new Favoris
                {
                    UserId = userId,
                    FilmId = filmId
                };
                _context.Favoris.Add(favoris);
                await _context.SaveChangesAsync();

                return Ok(favoris);
            }
            else
            {
                return NotFound("Film not found");
            }
        }


        // DELETE /api/Favoris/{id}
        [HttpDelete("Remove")]
        public async Task<IActionResult> DeleteFavoris(int userId, int filmId)
        {
            // Trouver le favori de cet utilisateur et ce film
            var favoris = await _context.Favoris
                                        .FirstOrDefaultAsync(f => f.UserId == userId && f.FilmId == filmId);
            
            if (favoris == null)
            {
                return NotFound("Film not found in favorites for this user");
            }
            else
            {
                _context.Favoris.Remove(favoris);
                await _context.SaveChangesAsync();
                return Ok("Film removed from favorites");
            }
        }

        
          //GET /api/Favorite/list/{userId}
        [HttpGet("Get Favorite List")]
        public async Task<ActionResult<IEnumerable<Favoris>>> GetFavoriteList(int userId)
        {
            var favoris = await _context.Favoris.Where(f => f.UserId == userId) 
                                .ToListAsync();
            if (favoris == null)
            {
                
                return NotFound("No favorites found");
            }
            return Ok(favoris);
        }
    }
}
