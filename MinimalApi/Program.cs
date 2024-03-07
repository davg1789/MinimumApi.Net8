using Microsoft.EntityFrameworkCore;
using MinimalApi.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOutputCache(opt =>
{
    opt.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(30);     
});
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LocationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseOutputCache();

app.MapGet("/location", async (LocationContext context) =>
await context.Location.ToListAsync())
.WithName("location")
.WithOpenApi()
.CacheOutput();


app.Run();
