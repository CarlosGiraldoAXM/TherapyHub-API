using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TherapuHubAPI.Configuration;
using TherapuHubAPI.Models;
using TherapuHubAPI.Repositorio;
using TherapuHubAPI.Repositorio.IRepositorio;
using TherapuHubAPI.Services.IServices;
using TherapuHubAPI.Services.Implementations;
using TherapuHubAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ContextDB>(options =>
    options.UseSqlServer(connectionString));

// Repository Pattern
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<ITipoUsuarioRepositorio, TipoUsuarioRepositorio>();
builder.Services.AddScoped<IMenuRepositorio, MenuRepositorio>();
builder.Services.AddScoped<ITipoEventoRepositorio, TipoEventoRepositorio>();
builder.Services.AddScoped<IEventosRepositorio, EventosRepositorio>();
builder.Services.AddScoped<IEventoUsuariosRepositorio, EventoUsuariosRepositorio>();
builder.Services.AddScoped<ICompaniaRepositorio, CompaniaRepositorio>();
builder.Services.AddScoped<ICompanyChatsRepositorio, CompanyChatsRepositorio>();
builder.Services.AddScoped<IChatMessagesRepositorio, ChatMessagesRepositorio>();
builder.Services.AddScoped<IMessageReadsRepositorio, MessageReadsRepositorio>();
builder.Services.AddScoped<IStaffRepositorio, StaffRepositorio>();
builder.Services.AddScoped<IStaffStatusRepositorio, StaffStatusRepositorio>();
builder.Services.AddScoped<IStaffRolesRepositorio, StaffRolesRepositorio>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITipoUsuarioService, TipoUsuarioService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<ITipoEventoService, TipoEventoService>();
builder.Services.AddScoped<IEventosService, EventosService>();
builder.Services.AddScoped<ICompaniaService, CompaniaService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IStaffService, StaffService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key no configurada");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TherapuHubAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TherapuHubAPI";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:8080",
                "http://localhost:5173",
                "https://localhost:8080",
                "https://localhost:5173",
                "http://127.0.0.1:8080",
                "http://127.0.0.1:5173",
                "https://127.0.0.1:8080",
                "https://127.0.0.1:5173",
                "https://therapyhub-suite.vercel.app"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TherapuHub API",
        Version = "v1",
        Description = "API para TherapuHub"
    });

    // Configurar JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TherapuHub API v1");
    c.RoutePrefix = "swagger";
});

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

// CORS debe estar antes de UseHttpsRedirection para que funcione correctamente
app.UseCors("AllowFrontend");

// Solo redirigir HTTPS en producción, no en desarrollo
if (!app.Environment.IsDevelopment())
{
    //app.UseHttpsRedirection();
}

app.Urls.Add("http://0.0.0.0:8080");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
