using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pizza_mama.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Stockage sécurisé du mot de passe

        [JsonIgnore]
        public ICollection<Pizza> Pizzas { get; set; } = new List<Pizza>(); // Un utilisateur peut avoir plusieurs pizzas
    }
}
