using System.Text;
using Calmind.Api.Data;
using Calmind.Api.Middleware;
using Calmind.Api.Repositories;
using Calmind.Api.Repositories.Interfaces;
using Calmind.Api.Services;
using Calmind.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ========== BANCO DE DADOS ==========
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CalmindContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

// ========== REPOSITORIES ==========
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<ICapsuleRepository, CapsuleRepository>();
builder.Services.AddScoped<ICollaboratorRepository, CollaboratorRepository>();

// ========== SERVICES ==========
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICapsuleService, CapsuleService>();
builder.Services.AddScoped<ICollaboratorService, CollaboratorService>();
builder.Services.AddSingleton<JwtService>();

// ========== JWT AUTHENTICATION ==========
var jwtKey = builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey não configurado");
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Em produção, mude para true
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ========== CONTROLLERS ==========
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ========== SWAGGER COM JWT ==========
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CalMind API",
        Version = "v1",
        Description = "API para gerenciamento de reservas de cápsulas de relaxamento"
    });

    // Configuração JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
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
            new string[] {}
        }
    });
});

var app = builder.Build();

// ========== MIGRATIONS AUTOMÁTICAS ==========
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CalmindContext>();
    db.Database.Migrate();
}

// ========== MIDDLEWARE DE EXCEÇÕES GLOBAL ==========
var logger = app.Services.GetRequiredService<ILogger<Program>>();
app.ConfigureExceptionHandler(logger);

// ========== SWAGGER ==========
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ========== AUTENTICAÇÃO E AUTORIZAÇÃO ==========
app.UseAuthentication(); // ? OBRIGATÓRIO antes do UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();