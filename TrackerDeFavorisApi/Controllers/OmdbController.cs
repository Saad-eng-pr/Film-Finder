
using Microsoft.AspNetCore.Mvc;


namespace TrackerDeFavorisApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OmdbController : ControllerBase
    {
        private readonly OmdbService _omdbService;

        public OmdbController(OmdbService omdbService)
        {
            _omdbService = omdbService;
        }
        
        
        // GET: api/<OmdbController>
        [HttpGet("Search")]
        public async Task<IActionResult> SearchByTitre(string titre)
        {
            if (String.IsNullOrWhiteSpace(titre))
                return BadRequest("Le titre est vide");

            try {
                var films = await _omdbService.SearchByTitleAsync(titre);
                return Ok(films);
            } catch (HttpRequestException ex) {
                return StatusCode(500, $"Erreur lors de l'appel de l'API : {ex.Message}");
            }
        }

        // Post /api/Film/IdSearch
        [HttpGet("IdSearch")]
        public async Task<IActionResult> GetByImdbId(string imdbID)
        {
            if (String.IsNullOrWhiteSpace(imdbID))
                return BadRequest("L'id Imdb ne peut pas etre vide!!");

            try {
                var film = await _omdbService.GetByImdbId(imdbID);
                
                if(film.Title == null && film.Year == null) 
                    return NotFound("Film non trouvé");

                return Ok(film);
            } catch(HttpRequestException ex) {
                return StatusCode(500, $"Erreur lors de l'appel de l'API : {ex.Message}");
            }
        }
    }
}
