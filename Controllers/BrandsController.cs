#nullable disable
using Microsoft.AspNetCore.Mvc;
using ecommerce_API.Entities;
using Microsoft.AspNetCore.Authorization;
using ecommerce_API.Interfaces;

namespace ecommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandRepository _brandRepository;

        public BrandsController(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        // GET: api/Brands
        [HttpGet]
        [Route("getAll")]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrand()
        {
            return Ok(await _brandRepository.GetAll());
        }

        // GET: api/Brands/5
        [HttpPost]
        [Route("getOne/{id}")]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
            var brand = await _brandRepository.GetOne(id);

            if (brand == null)
            {
                return NotFound();
            }

            return Ok(brand);
        }

        // PUT: api/Brands/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBrand(int id, Brand brand)
        {
            if (!_brandRepository.CheckIfExists(id))
            {
                return NotFound();
            }
            try
            {
                Brand brandFromDb = await _brandRepository.Update(brand);
                return Ok(brandFromDb);
            }
            catch (Exception)
            {
                throw new Exception("Error: Update Brand failed! Failed uppon update.");
            }
        }

        // POST: api/Brands
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<ActionResult<Brand>> PostBrand(Brand brand)
        {
            if (!_brandRepository.CheckIfExists(brand.Name))
            {
                return BadRequest("Brand already exists!");
            }
            try
            {
                Brand brandFromDb = await _brandRepository.Create(brand);
                if (brandFromDb == null)
                {
                    return BadRequest("Brand not created!");
                }
                return Ok(brandFromDb);
            }
            catch (Exception)
            {
                throw new Exception("Error: Update Brand failed! Failed uppon update.");
            }
        }

        // DELETE: api/Brands/5
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            try
            {
            bool isDeleted = await _brandRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound();
            }

            return Ok();
            }
            catch (Exception)
            {

                throw new Exception("Error: Delete Brand failed! Failed uppon deletion.");
            }
        }

        [HttpPost]
        [Route("uploadProfilePicture/{id}")]
        [Authorize]
        public async Task<ActionResult<byte[]>> UploadBrandPicture(int id)
        {
            if (Request.Form.Files.Count == 0)
            {
                var updatedBrand = await _brandRepository.RemoveImage(id);
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
                var updatedBrand = await _brandRepository.AddImage(id, file);
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
    }
}
