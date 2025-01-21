using Microsoft.AspNetCore.Mvc;
using pizza_mama.Data;
using pizza_mama.Models;
using System.Linq;

namespace pizza_mama.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilisateurController : ControllerBase
    {
        private readonly DataContext _context;

        public UtilisateurController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetUtilisateurs()
        {
            return Ok(_context.Utilisateurs.ToList());
        }

        [HttpPost]
        public IActionResult CreateUtilisateur(Utilisateur utilisateur)
        {
            _context.Utilisateurs.Add(utilisateur);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetUtilisateurs), new { id = utilisateur.Id }, utilisateur);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUtilisateur(int id, Utilisateur utilisateur)
        {
            var user = _context.Utilisateurs.Find(id);
            if (user == null) return NotFound();

            user.Nom = utilisateur.Nom;
            user.Email = utilisateur.Email;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUtilisateur(int id)
        {
            var user = _context.Utilisateurs.Find(id);
            if (user == null) return NotFound();

            _context.Utilisateurs.Remove(user);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
