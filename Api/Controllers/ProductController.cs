using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Api.DTO;
using Bussines.Abstract;
using KokoMija.Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api
{
    // localhost:4200/api/products
    // localhost:4200/api/products/2
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController: ControllerBase
    {
        private IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
                var products = await _productService.GetAll();
                var productsDTO = new List<ProductDTO>();

                foreach (var item in products)
                {
                    var firstProductImage = item.ProductImages.FirstOrDefault();
                    productsDTO.Add(new ProductDTO{
                        ProductId=item.ProductId,
                        Name = item.ProductName,
                        Url = item.Url,
                        Price = item.Price,
                        Description = item.Description,
                        ImageUrl = firstProductImage?.Image?.ImageUrl
                    });
                    
                }

                var serializerSettings = new JsonSerializerSettings
                {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                };

                var serializedProducts = JsonConvert.SerializeObject(productsDTO, serializerSettings);

                return Ok(serializedProducts);
        }
        
        [HttpGet("{id}")]
        public  async Task<IActionResult> GetProduct(int id)
        {
            var p = await _productService.GetById(id);

            if(p==null)
            {
                return NotFound(); // 404
            }
            return Ok(p); // 200
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product entity)
        {
            await _productService.CreateAsync(entity);
            return CreatedAtAction(nameof(GetProduct), new {id=entity.ProductId},entity);
        }
        // localhost:5000/api/products/2
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product entity)
        {
            if (id!=entity.ProductId)
            {
                return BadRequest();
            }

            var product = await _productService.GetById(id);

            if(product==null)
            {
                return NotFound();
            }

            await _productService.UpdateAsync(product,entity);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetById(id);

            if(product==null)
            {
                return NotFound();
            }

            await _productService.DeleteAsync(product);
            return NoContent();
        }

        private static ProductDTO ProductToDTO(Product p)
        {
            return new ProductDTO{
                    ProductId = p.ProductId,
                    Name = p.ProductName,
                    Url = p.Url,
                    Description = p.Description,
                    Price = p.Price,
                };
        }
    }
}