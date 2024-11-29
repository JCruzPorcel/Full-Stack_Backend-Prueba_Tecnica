using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ExamenBackend.Data;
using ExamenBackend.Repositories.Interfaces;
using ExamenBackend.Repositories;
using ExamenBackend.Services.Interfaces;
using ExamenBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddControllers();

// Configuración de Swagger para documentación de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gestor Productos", Version = "v1" });
});

// Configuración de CORS para permitir solicitudes desde el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var frontendUrl = "http://localhost:4200"; // URL del frontend, ajustar según sea necesario
        if (!string.IsNullOrEmpty(frontendUrl))
        {
            policy.WithOrigins(frontendUrl)   // Permite solicitudes solo desde la URL del frontend
                  .AllowAnyMethod()          // Permite cualquier método HTTP
                  .AllowAnyHeader()          // Permite cualquier encabezado
                  .AllowCredentials();       // Permite credenciales (si es necesario)
        }
        else
        {
            throw new InvalidOperationException("La variable de entorno 'FRONTEND_URL' no está definida. No se puede configurar CORS.");
        }
    });
});

// Configuración de DbContext para SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Registro de servicios y repositorios
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gestor Productos V1");
        c.RoutePrefix = "swagger";  // Ruta para acceder a Swagger UI
    });
}

// Habilitar CORS con la política configurada
app.UseCors("AllowFrontend");

app.UseAuthorization(); // Usar autorización para los controladores

// Mapeo de controladores para la API
app.MapControllers();

// Iniciar la aplicación
app.Run();
