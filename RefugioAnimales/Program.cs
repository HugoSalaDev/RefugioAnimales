using Microsoft.EntityFrameworkCore;
using RefugioAnimales.Data;

// Configuración de la aplicación
var builder = WebApplication.CreateBuilder(args);

// Añadir servicios MVC
builder.Services.AddControllersWithViews();

// Configurar la base de datos con SQL Server LocalDB
builder.Services.AddDbContext<RefugioContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar sesiones para mantener el login
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ========================================
// CONFIGURACIÓN PARA SUBIDA DE ARCHIVOS
// ========================================

// Configurar límites de Kestrel (servidor web)
builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // 100 MB
});

// Configurar límites de formularios
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
    options.ValueLengthLimit = 104857600;
    options.MultipartHeadersLengthLimit = 104857600;
});

var app = builder.Build();

// Configurar el pipeline de peticiones HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Activar sesiones
app.UseAuthorization();

// Rutas personalizadas
app.MapControllerRoute(
    name: "animales",
    pattern: "Animales",
    defaults: new { controller = "Refugio", action = "Animales" });

app.MapControllerRoute(
    name: "detalle",
    pattern: "Animales/Detalle/{id}",
    defaults: new { controller = "Refugio", action = "Detalle" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Refugio}/{action=Inicio}/{id?}");

// Inicializar datos (cargar imágenes)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<RefugioContext>();
    DbInitializer.Initialize(context);
}

app.Run();