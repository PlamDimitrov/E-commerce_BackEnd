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
            return await _context.Menus
                .Include(m => m.SubMenus)
                .ThenInclude(s => s.Links)
                .ToListAsync();
        }

        // PUT: api/Menus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
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
                        _context.SubMenuLinks.Remove(linkDb);
                        await _context.SaveChangesAsync();
                    }
                }
            };

            var checkLinkForCreation = async (SubMenu subMenuClient) =>
            {
                List<SubMenuLinks> subMenuLinksFromDb = await _context.SubMenuLinks
                                        .AsNoTracking()
                                        .Where(l => l.SubMenuId == subMenuClient.Id)
                                        .ToListAsync();
                ICollection<SubMenuLinks> subMenuLinksFromClient = subMenuClient.Links;
                foreach (SubMenuLinks linkClient in subMenuLinksFromClient)
                {
                    var linkFromDb = await _context.SubMenuLinks
                    .AsNoTracking()
                    .Where(l => l.Id == linkClient.Id)
                    .FirstOrDefaultAsync();
                    if (linkFromDb == null)
                    {
                        SubMenu subMenu = _context.SubMenu
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
                SubMenu menuExist = await _context.SubMenu
                .AsNoTracking()
                .Where(s => s.Id == subMenuClient.Id)
                .FirstOrDefaultAsync();
                if (menuExist == null)
                {
                    Menu menuDb = _context.Menus
                    .AsNoTracking()
                    .Where(m => m.Id == menuClient.Id)
                    .Include(l => l.SubMenus)
                    .First(); ;
                    menuDb.SubMenus.Add(subMenuClient);
                    await _context.SaveChangesAsync();
                }
            };

            List<SubMenu> sebMenusFromDb = await _context.SubMenu
                .AsNoTracking()
                .Where(s => s.MenuId == menu.Id)
                .Include(s => s.Links)
                .ToListAsync();

            List<SubMenu> subMenusToDelete = new List<SubMenu>();
            ICollection<SubMenu> subMenusFromClient = menu.SubMenus;

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
                    _context.SubMenu.Remove(subMenuDb);
                    await _context.SaveChangesAsync();
                }
            }

            _context.Menus.Update(menu);
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
        [Authorize]

        public async Task<ActionResult<Menu>> PostMenu(Menu menu)
        {
            _context.Menus.Add(menu);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMenu", new { id = menu.Id }, menu);
        }

        // DELETE: api/Menus/5
        [HttpDelete("delete")]
        [Authorize]

        public async Task<IActionResult> DeleteMenu(Menu menu)
        {
            var MenuFromDatabase = await _context.Menus.FindAsync(menu.Id);
            if (MenuFromDatabase == null)
            {
                return NotFound();
            }

            _context.Menus.Remove(MenuFromDatabase);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MenuExists(int id)
        {
            return _context.Menus.Any(e => e.Id == id);
        }
    }
}
