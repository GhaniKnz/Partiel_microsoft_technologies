using ECommerceApi.Data;
using ECommerceApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Configuration de la base de données in-memory
builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseInMemoryDatabase("ECommerceDb"));

// === CHOIX DU SERVICE DE STOCK ===
// Option 1 : Utiliser le Singleton (sans base de données) - GARDER DANS LA SOLUTION
// builder.Services.AddSingleton<IStockService, StockService>();

// Option 2 : Utiliser la base de données in-memory (ACTIVE)
builder.Services.AddScoped<IStockService, DatabaseStockService>();

// Service des codes promos (utilise la base de données)
builder.Services.AddScoped<IPromoCodeService, DatabasePromoCodeService>();

// Enregistrement du service de commandes
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Initialiser la base de données avec les données de seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
