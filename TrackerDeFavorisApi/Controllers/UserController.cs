
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using TrackerDeFavorisApi.Models;

public class UserContext : DbContext
{
    public UserContext( DbContextOptions<UserContext> options )
        : base(options)
    {
    }

    protected override void OnConfiguring( DbContextOptionsBuilder options )
    {
        // Connexion a la base de donnees
        options.UseSqlite("Data Source=User.db");
    }
    public DbSet<User> Users { get; set; } = null!;
}


namespace TrackerDeFavorisApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;

         private readonly JwtService _jwtService;

        public UserController(UserContext ctx,JwtService jwtService)
        {
            _jwtService = jwtService;
            _context = ctx;
        }

        // GET /api/user
        [HttpGet]    
        [HttpGet("id")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            // on récupère la User correspondant a l'id
            var User = await _context.Users.FindAsync(id);

            if (User == null)
            {
                return NotFound();
            }
            return Ok(User);
        }
 
        //POST /api/user/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> PostUser(UserInfo userInfo)
        {
            // on créer un nouveau user avec les informations reçu
            User user = new User {
                Pseudo = userInfo.Pseudo, 
                MotDePasse = userInfo.MotDePasse,
                Role = Role.User
            };
            // on l'ajoute a notre contexte (BDD)
            _context.Users.Add(user);
            // on enregistre les modifications dans la BDD ce qui remplira le champ Id de notre objet
            await _context.SaveChangesAsync();
            // on retourne un code 201 pour indiquer que la création a bien eu lieu
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // POST /api/user/login
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserInfo userInfo) 
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Pseudo == userInfo.Pseudo && u.MotDePasse == userInfo.MotDePasse);
            if(user == null)
            {
                return Unauthorized("Identifiant ou mot de passe incorrect !!");
            }
            // Si la vérification réussie, générer un JWT
            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Pseudo, user.Role.ToString());

            // Retourner le token dans la réponse
            return Ok(new { message = "Login successful, welcome back!", userName = user.Pseudo, user.Role,token });        }  


       // PUT /api/user/{id} update la BDD et le user 
        [HttpPut("Update User as Admin")]
        public async Task<IActionResult> PutUser(int id,UserInfo userInfo)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) 
            {
                return NotFound();
            }

            user.Pseudo = userInfo.Pseudo;
            user.MotDePasse = userInfo.MotDePasse;
            user.Role = userInfo.Role;

            _context.Entry(user).State = EntityState.Modified;

            try{
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Erreur de concurrence");
            }

            return Ok(user);
        }

        // PUT /api/user/{id} update la BDD et le user 
        [HttpPut("Update User as user")]
        public async Task<IActionResult> PutUser(int id,string pseudo,string motDePasse)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) 
            {
                return NotFound();
            }

            user.Pseudo = pseudo;
            user.MotDePasse = motDePasse;

            _context.Entry(user).State = EntityState.Modified;

            try{
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Erreur de concurrence");
            }

            return Ok(user);
        }
        // DELETE /api/user/{id}
        [HttpDelete("Delete User")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET /api/User 
        [HttpGet("Get All Users Pseudos")]
        [Authorize(Roles = "User")] // Permet d'accéder à cette route si l'utilisateur a le role Admin
        public List<string> GetUsersPseudos()
        {
            List<string> PseudoL =  _context.Users
                                            .Select(u => u.Pseudo)
                                            .ToList();

            return PseudoL;
        }

        // GET /api/User
        [HttpGet("Get All Users")]
        [Authorize(Roles = "Admin")] // Permet d'accéder à cette route si l'utilisateur a le role Admin

        public async Task<ActionResult<IEnumerable<PublicUsers>>> GetUsers()
        {
            var users = await _context.Users
                                        .Select(u => new PublicUsers
                                        {
                                            Id = u.Id,
                                            Pseudo = u.Pseudo,
                                            Role = u.Role
                                        })
                                        .ToListAsync();

            return Ok(users); 
        }

        // GET /api/User Admin
        [HttpGet("Get Admins")]
        public async Task<ActionResult<IEnumerable<PublicUsers>>> GetUserAdmin() 
        {
            var users = await _context.Users
                                        .Where (u => u.Role == Role.Admin)
                                        .Select(u => new PublicUsers
                                        {
                                            Id = u.Id,
                                            Pseudo = u.Pseudo,
                                            Role = u.Role 
                                        })
                                        .ToListAsync();

            return Ok(users);
        }

        // GET /api/User users with "name" in their pseudo
        [HttpGet("Search 'name' in pseudo")]
        public async Task<ActionResult<IEnumerable<PublicUsers>>> SearchNameInPeudo()
        {
            var users = await _context.Users
                                        .Where (u => u.Pseudo.Contains("name"))
                                        .Select(u => new PublicUsers
                                        {
                                            Id = u.Id,
                                            Pseudo = u.Pseudo,
                                            Role = u.Role 
                                        })
                                        .ToListAsync();

            return Ok(users);
        }
        
    }
}