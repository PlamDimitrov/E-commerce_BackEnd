#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerce_API.Data;
using ecommerce_API.Entities;
using Microsoft.AspNetCore.Authorization;
using ecommerce_API.Services;

namespace ecommerce_API.Controllers
{
    public class Payload
    {
        public int Id { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly ecommerce_APIContext _context;
        private readonly ImageService _imageService;

        public BrandsController(ecommerce_APIContext context)
        {
            _context = context;
            _imageService = new ImageService(context);
        }

        // GET: api/Brands
        [HttpGet]
        [Route("getAll")]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrand()
        {
            return await _context.Brands.ToListAsync();
        }

        // GET: api/Brands/5
        [HttpPost]
        [Route("getOne/{id}")]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            return brand;
        }

        // PUT: api/Brands/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBrand(int id, Brand brand)
        {
            Brand brandFromDb = await _context.Brands
                .Where(c => c.Id == id)
                .FirstAsync();
            brandFromDb.Name = brand.Name;
            _context.Brands.Update(brandFromDb);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(brandFromDb);
        }

        // POST: api/Brands
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<ActionResult<Brand>> PostBrand(Brand brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBrand", new { id = brand.Id }, brand);
        }

        // DELETE: api/Brands/5
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [Route("uploadProfilePicture/{id}")]
        [Authorize]
        public async Task<ActionResult<byte[]>> UploadBrandPicture(int id)
        {
            if (Request.Form.Files.Count == 0)
            {
                var updatedBrand = await _imageService.RemoveFromBrand(id);
                if (updatedBrand == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedBrand);
                }
            }
            else
            {
                IFormFile file = Request.Form.Files[0];
                var updatedBrand = await _imageService.AddToBrand(id, file);
                if (updatedBrand == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedBrand);
                }
            }
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }
    }
}
