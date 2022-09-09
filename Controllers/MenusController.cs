#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerce_API.Data;
using ecommerce_API.Entities;
using Microsoft.AspNetCore.Authorization;
using ecommerce_API.Entities.MainMenu;

namespace ecommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController : ControllerBase
    {
        private readonly ecommerce_APIContext _context;

        public MenusController(ecommerce_APIContext context)
        {
            _context = context;
        }

        // GET: api/Menus
        [HttpGet]
        [Route("getAll")]
        public async Task<ActionResult<IEnumerable<Menu>>> GetMenu()
        {
            return await _context.Menu
                .Include(m => m.subMenus)
                .ThenInclude(s => s.Links)
                .ToListAsync();
        }

        // GET: api/Menus/5
        [HttpPost]
        [Route("getOne")]
        public async Task<ActionResult<Menu>> GetMenu(Payload payload)
        {
            var Menu = await _context.Menu.FindAsync(payload.Id);
            Console.WriteLine(payload.Id);

            if (Menu == null)
            {
                return NotFound();
            }

            return Menu;
        }

        // PUT: api/Menus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> PutMenu(int id, Menu menu)
        {
            if (id != menu.Id)
            {
                return BadRequest();
            }

            _context.Entry(menu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Menus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Menu>> PostMenu( Menu menu )
        {
            _context.Menu.Add(menu);

            await _context.SaveChangesAsync();
            


            return CreatedAtAction("GetMenu", new { id = menu.Id }, menu);
        }

        // DELETE: api/Menus/5
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteMenu(Menu menu)
        {
            var MenuFromDatabase = await _context.Menu.FindAsync(menu.Id);
            if (MenuFromDatabase == null)
            {
                return NotFound();
            }

            _context.Menu.Remove(MenuFromDatabase);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MenuExists(int id)
        {
            return _context.Menu.Any(e => e.Id == id);
        }
    }
}
