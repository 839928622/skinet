using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepos _productRepos;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;

        public ProductsController(IProductRepos productRepos,
            IGenericRepository<Product> productRepo,
            IGenericRepository<ProductBrand> productBrandRepo,
            IGenericRepository<ProductType> productTypeRepo)
        {
            _productRepos = productRepos;
            _productRepo = productRepo;
            _productBrandRepo = productBrandRepo;
            _productTypeRepo = productTypeRepo;
        }

        [HttpGet("products")]
        public async  Task<ActionResult<List<Product>>> GetProductsAsync() =>
            Ok(await _productRepo.ListAsync(new ProductsWithTypesAndBrandsSpecification())) ;

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id) => 
            Ok(await _productRepo.ListAsync(new ProductsWithTypesAndBrandsSpecification(id)));

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
         return   Ok(await _productBrandRepo.ListAllAsync());
        }


        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductTypes()
        {
            return Ok(await _productTypeRepo.ListAllAsync());
        }
    }
}
