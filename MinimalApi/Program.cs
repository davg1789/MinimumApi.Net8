using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Context;
using MinimalApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOutputCache(opt =>
{
    opt.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(30);     
});
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

var projectRootPath = Directory.GetCurrentDirectory();
//Ideal to keep out of the project
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(projectRootPath, "DataProtection-Keys")));

//Get encrypted connection string
var serviceProvider = builder.Services.BuildServiceProvider();
var protectorProvider = serviceProvider.GetRequiredService<IDataProtectionProvider>();
var protector = protectorProvider.CreateProtector("MinimalApi.ConnectionString");
var encryptedConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var decryptedConnectionString = protector.Unprotect(encryptedConnectionString);
builder.Services.AddDbContext<LocationContext>(options => options.UseSqlServer(decryptedConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseOutputCache();

app.MapGet("/location", async (LocationContext context) =>
    await context.Location.ToListAsync())
.WithName("location")
.WithOpenApi()
.CacheOutput();


app.Run();
