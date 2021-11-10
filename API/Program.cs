using StackExchange.Redis;
using Core.Entities.Identity;
var builder = WebApplication.CreateBuilder(args);

// add services to the container

builder.Services.AddControllers();

builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});

builder.Services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(config =>
{
    var configuration = ConfigurationOptions.Parse(builder.Configuration
        .GetConnectionString("Redis"), true);
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Skinet API", Version = "v1" });
    var securitySchema = new OpenApiSecurityScheme()
    {
        Description = "JWT Auth Bearer Schema",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Type = SecuritySchemeType.Http,
        Reference = new OpenApiReference()
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme,
        }
    };
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securitySchema);
    var securityRequirement = new OpenApiSecurityRequirement()
                {
                    {securitySchema, new List<string>() {JwtBearerDefaults.AuthenticationScheme} }
                };
    options.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
    });
});


// configure the http request pipeline
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
// if a request comes into Aapi but we dont have an endpoint that match 
//that particular request then we are going to hit this bit of middleware and
// its going to redirect to ErrorController
app.UseStatusCodePagesWithReExecute("/errors/{0}");
app.UseHttpsRedirection();

// In the new hosting model, the app.UseRouting() doesn't do any harm to leaveit in place, but it is 
// something we can actually remove because this is implicityly enabled that we want to use routing
// app.UseRouting(); 
app.UseStaticFiles(); // wwwroot
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(x =>
{
    x.SwaggerEndpoint("/swagger/v1/swagger.json", "SKiNet APi v1");
});

app.MapControllers();
app.MapFallbackToController("Index","Fallback");
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});



// apply migrations or database if it does not exist
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var loggerFactory = services.GetRequiredService<ILoggerFactory>();
try
{
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    // seed product data
    await StoreContextSeed.SeedAsync(context, loggerFactory);

    // seed user
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var identityContext = services.GetRequiredService<AppIdentityDbContext>();
    await identityContext.Database.MigrateAsync();
    await AppIdentityDbContextSeed.SeedUserAsync(userManager);
}
catch (Exception e)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(e, "An error occured during migration");

}

await app.RunAsync();

//namespace API
//{
//    public class Program
//    {
//        public static async Task Main(string[] args)
//        {
//          var host =  CreateHostBuilder(args).Build();
//            // apply migrations or database if it does not exist
//            using var scope = host.Services.CreateScope();
//            var services = scope.ServiceProvider;
//            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
//            try
//            {
//                var context = services.GetRequiredService<StoreContext>();
//                 await context.Database.MigrateAsync();
//                 // seed product data
//                 await StoreContextSeed.SeedAsync(context, loggerFactory);

//                 // seed user
//                 var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
//                 var identityContext = services.GetRequiredService<AppIdentityDbContext>();
//                 await identityContext.Database.MigrateAsync();
//                 await AppIdentityDbContextSeed.SeedUserAsync(userManager);
//            }
//            catch (Exception e)
//            {
//                var logger = loggerFactory.CreateLogger<Program>();
//                logger.LogError(e,"An error occured during migration");

//            }

//            await host.RunAsync();
//        }

//        public static IHostBuilder CreateHostBuilder(string[] args) =>
//            Host.CreateDefaultBuilder(args).
//                ConfigureAppConfiguration((hostingContext, config) =>
//                {
//                    var env = hostingContext.HostingEnvironment;
//                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
//                    config.AddEnvironmentVariables();
//                })
//                .ConfigureWebHostDefaults(webBuilder =>
//                {
//                    var assemblyName = typeof(Startup).GetTypeInfo().Assembly.FullName;

//                    // webBuilder.UseStartup<Startup>();
//                    webBuilder.UseStartup(assemblyName);

//                });
//    }
//}
