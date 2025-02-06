using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;

namespace API.Controllers
{
    public class ProductsController(IUnitOfWork unit) : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)
        {
            var spec = new ProductSpecification(specParams);

            return await CreatePagedResult(unit.Repository<Product>() , spec , specParams.PageIndex , specParams .PageSize);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await unit.Repository<Product>().GetByIdAsync(id);
            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(CreateProductDto productDto)
        {
            var product = new Product {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Type = productDto.Type,
                PictureUrl = productDto.PictureUrl,
                Description = productDto.Description,
                Price = productDto.Price,
                QuantityInStock = productDto.QuantityInStock
            };

            unit.Repository<Product>().Add(product);
            if (await unit.Complete())
                return CreatedAtAction("GetProduct",new { id= product.Id},product);

            return BadRequest("Problem While Creating The Product");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id || !unit.Repository<Product>().Exist(id)) return NotFound();

            unit.Repository<Product>().Update(product);

            if (await unit.Complete())
                return NoContent();

            return BadRequest("Problem While Updating The Product");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await unit.Repository<Product>().GetByIdAsync(id);

            if (product is null) return NotFound();

            unit.Repository<Product>().Delete(product);

            if (await unit.Complete())
                return NoContent();

            return BadRequest("Problem While Deleting The Product");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            var spec = new BrandListSpecification();

            return Ok(await unit.Repository<Product>().ListAsync(spec));
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            var spec = new TypeListSpecification();

            return Ok(await unit.Repository<Product>().ListAsync(spec));
        }

    }
}