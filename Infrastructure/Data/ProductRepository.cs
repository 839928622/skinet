using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
  public  class ProductRepository : IProductRepos
    {
        private readonly StoreContext _storeContext;

        public ProductRepository(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public async Task<Product> GetProductByIdAsync(int id) => await _storeContext.Products.FindAsync(id);


        public async Task<IReadOnlyList<Product>> GetProductsAsync() => await _storeContext.Products.ToListAsync();

    }
}
