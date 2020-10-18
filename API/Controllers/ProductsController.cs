using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepos _productRepos;

        public ProductsController(IProductRepos productRepos)
        {
            _productRepos = productRepos;
        }

        [HttpGet]
        public async  Task<ActionResult<List<Product>>> GetProductsAsync() => Ok(await _productRepos.GetProductsAsync()) ;

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id) => Ok(await _productRepos.GetProductByIdAsync(id));

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
         return   Ok(await _productRepos.GetProductBrandsAsync());
        }


        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductTypes()
        {
            return Ok(await _productRepos.GetProductTypesAsync());
        }
    }
}
