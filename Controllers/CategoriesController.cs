#nullable disable
using Microsoft.AspNetCore.Mvc;
using ecommerce_API.Entities;
using Microsoft.AspNetCore.Authorization;
using ecommerce_API.Interfaces;

namespace ecommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: api/Categories
        [HttpGet]
        [Route("getAll")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategory()
        {
            return Ok(await _categoryRepository.GetAll());
        }

        // GET: api/Categories/5
        [HttpGet("getOne/{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _categoryRepository.GetOne(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (!_categoryRepository.CheckIfExists(id))
            {
                return NotFound();
            }
            try
            {
                Category categoryFromDb = await _categoryRepository.Update(category);
                return Ok(categoryFromDb);
            }
            catch (Exception)
            {
                throw new Exception("Error: Update Category failed! Failed uppon update.");
            }
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            if (_categoryRepository.CheckIfExists(category.Name))
            {
                return BadRequest("Category already exists!");
            }
            else
            {
                try
                {
                    Category categoryFromDb = await _categoryRepository.Create(category);
                    if (categoryFromDb == null)
                    {
                        return BadRequest("Category not created!");
                    }
                    return Ok(categoryFromDb);
                }
                catch (Exception)
                {
                    throw new Exception("Error: Create Category failed! Failed uppon creation.");
                }
            }
        }

        // DELETE: api/Categories/5
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                bool isDeleted = await _categoryRepository.Delete(id);
                if (!isDeleted)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception)
            {

                throw new Exception("Error: Delete Category failed! Failed uppon deletion.");
            }
        }

        [HttpPost]
        [Route("uploadProfilePicture/{id}")]
        [Authorize]
        public async Task<ActionResult<byte[]>> UploadCategoryPicture(int id)
        {
            if (Request.Form.Files.Count == 0)
            {
                var updatedCategory = await _categoryRepository.RemoveImage(id);
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
                var updatedCategory = await _categoryRepository.AddImage(id, file);
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
    }
}
