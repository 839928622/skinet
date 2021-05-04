using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API.UnitTests.Helper
{
  public abstract class TestBase
    {
        protected StoreContext GetStoreContext()
        {
            var options = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new StoreContext(options);
        }
    }
}
