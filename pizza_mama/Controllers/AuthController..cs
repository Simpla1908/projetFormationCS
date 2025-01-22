using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using pizza_mama.Data;
using pizza_mama.Models;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace pizza_mama.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;


        public AuthController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        // Méthode pour enregistrer un nouvel utilisateur
        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (_context.Utilisateurs.Any(u => u.Email == model.Email))
                return BadRequest("L'utilisateur existe déjà.");

            // Hachage du mot de passe avant de l'enregistrer
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var utilisateur = new Utilisateur
            {
                Nom = model.Nom,
                Email = model.Email,
                PasswordHash = passwordHash
            };

            _context.Utilisateurs.Add(utilisateur);
            _context.SaveChanges();

            return Ok("Utilisateur enregistré avec succès.");
        }

        // Méthode pour se connecter et obtenir un JWT
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = _context.Utilisateurs.SingleOrDefault(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                return Unauthorized(); // Si l'utilisateur est introuvable ou que le mot de passe est incorrect

            // Création du JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Email) }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
    }
}
