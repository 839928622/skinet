﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;
using Core.Entities;
using Core.Entities.OrderAggregate;

namespace Infrastructure.Data.SeedData
{
  public  class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                if (!context.ProductBrands.Any())
                {
                    // the root path is base on where the folder you run the app
                    // so the root is API project's folder
                    var brandsData = File.ReadAllText("../Infrastructure/Data/SeedData/brands.json"); 

                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                    brands.ForEach(p =>
                    {
                        context.ProductBrands.Add(p);
                    });
                }

                if (!context.ProductTypes.Any())
                {
                    var typesData = File.ReadAllText("../Infrastructure/Data/SeedData/types.json");

                    var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
                    types.ForEach(p =>
                    {
                        context.ProductTypes.Add(p);
                    });
                }

                if (!context.Products.Any())
                {
                    var productsData = File.ReadAllText("../Infrastructure/Data/SeedData/products.json");

                    var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                    products.ForEach(p =>
                    {
                        context.Products.Add(p);
                    });
                }

                if (!context.DeliveryMethods.Any())
                {
                    var deliveryMethodsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/delivery.json");

                    var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);
                    deliveryMethods.ForEach(d =>
                    {
                        context.DeliveryMethods.Add(d);
                    });
                }

                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<StoreContextSeed>();
                logger.LogError(e,"An error occured during data seeding");
            }
        }
    }
}
