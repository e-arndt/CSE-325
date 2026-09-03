using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("[controller]")]
public class PizzaController : ControllerBase
{
    public PizzaController()
    {
    }

    // GET /pizza
    [HttpGet]
    public ActionResult<List<Pizza>> GetAll() =>
        PizzaService.GetAll();

    // GET /pizza/{id}
    [HttpGet("{id}")]
    public ActionResult<Pizza> Get(int id)
    {
        var pizza = PizzaService.Get(id);

        if (pizza == null)
            return NotFound();

        return pizza;
    }

    // POST /pizza
    [HttpPost]
    public IActionResult Create(Pizza pizza)
    {
        PizzaService.Add(pizza);

        return CreatedAtAction(
            nameof(Get),
            new { id = pizza.Id },
            pizza);
    }

    // PUT /pizza/{id}
    [HttpPut("{id}")]
    public IActionResult Update(int id, Pizza pizza)
    {
        if (id != pizza.Id)
            return BadRequest();

        var existingPizza = PizzaService.Get(id);

        if (existingPizza is null)
            return NotFound();

        PizzaService.Update(pizza);

        return NoContent();
    }

    // DELETE /pizza/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var pizza = PizzaService.Get(id);

        if (pizza is null)
            return NotFound();

        PizzaService.Delete(id);

        return NoContent();
    }
}