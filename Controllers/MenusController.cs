#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerce_API.Data;
using ecommerce_API.Entities;
using Microsoft.AspNetCore.Authorization;
using ecommerce_API.Entities.MainMenu;
using System.Collections.Generic;

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
        public async Task<IActionResult> PutMenu(Menu menu)
        {

            var checkLinkForDeletion = async (ICollection<SubMenuLinks> linksClient, ICollection<SubMenuLinks> linksDb) =>
            {
                foreach (SubMenuLinks linkDb in linksDb)
                {
                    var boolianFlagLink = false;
                    foreach (SubMenuLinks linkClient in linksClient)
                    {
                        if (linkDb.Id == linkClient.Id)
                        {
                            boolianFlagLink = true;
                        }
                    }
                    if (boolianFlagLink == false)
                    {
                        _context.Links.Remove(linkDb);
                        await _context.SaveChangesAsync();
                    }
                }
            };

            var checkLinkForCreation = async (SubMenu subMenuClient) =>
            {
                List<SubMenuLinks> subMenuLinksFromDb = await _context.Links
                                        .AsNoTracking()
                                        .Where(l => l.subMenuId == subMenuClient.Id)
                                        .ToListAsync();
                ICollection<SubMenuLinks> subMenuLinksFromClient = subMenuClient.Links;
                foreach (SubMenuLinks linkClient in subMenuLinksFromClient)
                {
                    var linkFromDb = await _context.Links
                    .AsNoTracking()
                    .Where(l => l.Id == linkClient.Id)
                    .FirstOrDefaultAsync();
                    if (linkFromDb == null)
                    {
                        SubMenu subMenu = _context.subMenu
                        .AsNoTracking()
                        .Where(s => s.Id == subMenuClient.Id)
                        .Include(l => l.Links)
                        .First(); ;
                        subMenu.Links.Add(linkClient);
                        await _context.SaveChangesAsync();
                    }
                }
            };

            var checkSubMenuForCreation = async (Menu menuClient, SubMenu subMenuClient) =>
            {
                SubMenu menuExist = await _context.subMenu
                .AsNoTracking()
                .Where(s => s.Id == subMenuClient.Id)
                .FirstOrDefaultAsync();
                if (menuExist == null)
                {
                    Menu menuDb = _context.Menu
                    .AsNoTracking()
                    .Where(m => m.Id == menuClient.Id)
                    .Include(l => l.subMenus)
                    .First(); ;
                    menuDb.subMenus.Add(subMenuClient);
                    await _context.SaveChangesAsync();
                }
            };

            List<SubMenu> sebMenusFromDb = await _context.subMenu
                .AsNoTracking()
                .Where(s => s.MenuId == menu.Id)
                .Include(s => s.Links)
                .ToListAsync();

            List<SubMenu> subMenusToDelete = new List<SubMenu>();
            ICollection<SubMenu> subMenusFromClient = menu.subMenus;

            foreach (SubMenu subMenuDb in sebMenusFromDb)
            {
                var boolianFlag = false;
                foreach (SubMenu subMenuClient in subMenusFromClient)
                {
                    if (subMenuDb.Id == subMenuClient.Id)
                    {
                        boolianFlag = true;
                        await checkLinkForDeletion(subMenuClient.Links, subMenuDb.Links);
                        await checkLinkForCreation(subMenuClient);
                    }
                    else
                    {
                        await checkSubMenuForCreation(menu,subMenuClient);
                    }
                }
                if (boolianFlag == false)
                {
                    _context.subMenu.Remove(subMenuDb);
                    await _context.SaveChangesAsync();
                }
            }

            _context.Menu.Update(menu);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuExists(menu.Id))
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
        public async Task<ActionResult<Menu>> PostMenu(Menu menu)
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
