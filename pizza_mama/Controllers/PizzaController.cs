using Microsoft.AspNetCore.Mvc;
using pizza_mama.Data;
using pizza_mama.Models;
using System.Linq;

namespace pizza_mama.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzaController : ControllerBase
    {
        private readonly DataContext _context;

        public PizzaController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetPizzas()
        {
            return Ok(_context.Pizzas.ToList());
        }

        [HttpPost]
        public IActionResult CreatePizza(Pizza pizza)
        {
            _context.Pizzas.Add(pizza);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetPizzas), new { id = pizza.PizzaID }, pizza);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePizza(int id, Pizza pizza)
        {
            var existingPizza = _context.Pizzas.Find(id);
            if (existingPizza == null) return NotFound();

            existingPizza.nom = pizza.nom;
            existingPizza.prix = pizza.prix;
            existingPizza.vegetarienne = pizza.vegetarienne;
            existingPizza.ingredients = pizza.ingredients;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePizza(int id)
        {
            var pizza = _context.Pizzas.Find(id);
            if (pizza == null) return NotFound();

            _context.Pizzas.Remove(pizza);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
