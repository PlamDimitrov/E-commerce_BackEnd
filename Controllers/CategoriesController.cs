#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerce_API.Data;
using ecommerce_API.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using ecommerce_API.Services;

namespace ecommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ecommerce_APIContext _context;
        private readonly ImageService _imageService;

        public CategoriesController(ecommerce_APIContext context)
        {
            _context = context;
            _imageService = new ImageService(context);
        }

        // GET: api/Categories
        [HttpGet]
        [Route("getAll")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategory()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("getOne/{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            Category categoryFromDb = await _context.Categories
                .Where(c => c.Id == id)
                .FirstAsync();
            categoryFromDb.Name = category.Name;
            _context.Categories.Update(categoryFromDb);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(categoryFromDb);
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [Route("uploadProfilePicture/{id}")]
        [Authorize]
        public async Task<ActionResult<byte[]>> UploadCategoryPicture(int id)
        {
            if (Request.Form.Files.Count == 0)
            {
                var updatedCategory = await _imageService.RemoveFromCategory(id);
                if (updatedCategory == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedCategory);
                }
            }
            else
            {
                IFormFile file = Request.Form.Files[0];
                var updatedCategory = await _imageService.AddToBrand(id, file);
                if (updatedCategory == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedCategory);
                }
            }
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
