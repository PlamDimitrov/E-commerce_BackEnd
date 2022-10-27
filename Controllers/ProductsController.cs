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
using ecommerce_API.Interfaces;
using ecommerce_API.Repository;

namespace ecommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: api/Products
        [HttpGet]
        [Route("getAll")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            try
            {
                var product = await _productRepository.GetAll();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(product);
            }
            catch (Exception)
            {
                throw new Exception("Error: Not possible to get all products!");
            }
        }

        // GET: api/Products/5
        [HttpGet("getOne/{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetOne(id);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(product);
            }
            catch (Exception)
            {
                throw new Exception("Error: Not possible to get all products!");
            }
        }
        [HttpGet("getOne")]
        public async Task<ActionResult<Product>> GetProduct(string title)
        {
            try
            {
                var product = await _productRepository.GetOne(title);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(product);
            }
            catch (Exception)
            {
                throw new Exception("Error: Not possible to get all products!");
            }
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("edit/{id}")]
        [Authorize]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            try
            {
                await _productRepository.Update(product);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_productRepository.CheckIfExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            Product createdProduct = new Product();
            try
            {
                createdProduct = await _productRepository.Create(product);
            return Ok(createdProduct);
            }
            catch (Exception)
            {
                throw new Exception("Error: Not possible to create a product!");
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productExists = _productRepository.CheckIfExists(id);
            if (productExists)
            {
                try
                {
                    var isProductDeleted = await _productRepository.Delete(id);
                    if (isProductDeleted)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Error: Not possible to delete the product!");
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}
