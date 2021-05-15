using System;
using System.Linq;
using System.Text;
using API.IntegrationTests.Extensions;
using Core.Entities.Identity;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.IntegrationTests.Helpers
{
    public class FakeStartup //: Startup
    {
        public FakeStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("StoreConnection"));
            });

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
            //            options =>
            //            {
            //                options.LoginPath = new PathString("/auth/login");
            //                options.AccessDeniedPath = new PathString("/auth/denied");
            //            });
            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection"));
            });
            //var builder = services.AddIdentityCore<ApplicationUser>();
            //builder = new IdentityBuilder(builder.UserType, builder.Services);
            //builder.AddEntityFrameworkStores<AppIdentityDbContext>();
            //builder.AddSignInManager<SignInManager<ApplicationUser>>();

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters()
            //        {
            //            ValidateIssuerSigningKey = true, // if we leave this false , any user can send token they want 
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:key"])),
            //           // ValidIssuer = Configuration["Token:Issuer"], //because  we add Issuer  when we generate that token , we want to validate that 
            //            ValidateIssuer = false,
            //            ValidateAudience = false // token can have Issuer and Audience, Audience means who the token was issue to 

            //        };
            //    });
            //services.AddScoped<ITokenService,TokenService>();
            //services.AddAuthorization();
            services.AddIdentityServices(Configuration);

            services.AddControllers();
            services.AddApplicationServices();
            services.AddScoped<IOrderService, OrderService>();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var serviceScope = serviceScopeFactory.CreateScope();
            var storeContext = serviceScope.ServiceProvider.GetService<StoreContext>();
            var identityContext = serviceScope.ServiceProvider.GetService<AppIdentityDbContext>();
            if (storeContext == null)
            {
                throw new NullReferenceException("Cannot get instance of storeContext");
            }

            if (storeContext.Database.GetDbConnection().ConnectionString.ToLower().Contains("live.db"))
            {
                throw new Exception("LIVE SETTINGS IN TESTS!");
            }

            //dbContext.Database.EnsureDeleted();
             
             //Ensures that the database for the context exists. If it exists, no action is taken. If it does not
             //exist then the database and all its schema are created. If the database exists, then no effort is made
             //to ensure it is compatible with the model for this context.
        
         
             //Note that this API does not use migrations to create the database. In addition, the database that is
             //created cannot be later updated using migrations. If you are targeting a relational database and using migrations,
             //you can use the DbContext.Database.Migrate() method to ensure the database is created and all migrations
             //are applied.
            
           // dbContext.Database.EnsureCreated();
            storeContext.Database.Migrate();
            if (identityContext == null)
            {
                throw new NullReferenceException("Cannot get instance of identityContext");
            }
            identityContext.Database.Migrate();
            //seeding data 
           
            
            storeContext.SaveChanges();
        }
    }
}