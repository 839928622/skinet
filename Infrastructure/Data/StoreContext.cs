using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class StoreContext : DbContext
    {


        public StoreContext()
        {

        }

        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {

        }



        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands
        {
            get;
            set;
        }
        public DbSet<ProductType> ProductTypes { get; set; }
}
}
