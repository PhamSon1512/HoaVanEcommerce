using HoaVanEcommerce.BE.Application.Interfaces;
using HoaVanEcommerce.BE.Application.Services;
using HoaVanEcommerce.BE.Application.Settings;
using HoaVanEcommerce.Domain.Entities;
using HoaVanEcommerce.Infrastructure.Data;
using HoaVanEcommerce.Infrastructure.Repositories;
using HoaVanEcommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
builder.Services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));

// Configure JWT Authentication
var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
if (jwtSettings != null && !string.IsNullOrEmpty(jwtSettings.Key))
{
    var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.Key);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Custom event to handle token with or without "Bearer" prefix
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                
                if (string.IsNullOrEmpty(authHeader))
                {
                    // Try to get token from query string as fallback
                    var tokenFromQuery = context.Request.Query["token"].ToString();
                    if (!string.IsNullOrEmpty(tokenFromQuery))
                    {
                        context.Token = tokenFromQuery;
                    }
                    return Task.CompletedTask;
                }
                
                // If token starts with "Bearer ", remove it (standard format)
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                }
                else
                {
                    // Token without "Bearer" prefix - accept it directly
                    context.Token = authHeader.Trim();
                }
                
                return Task.CompletedTask;
            }
        };
    });
}

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IShopSettingRepository, ShopSettingRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IShopSettingService, ShopSettingService>();
builder.Services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();

const string CorsPolicyName = "AllowAngularClient";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "HoaVanEcommerce API",
        Version = "v1",
        Description = "API documentation for HoaVanEcommerce e-commerce platform"
    });

    // Add JWT Bearer authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: \"Bearer abc123\" or just paste the token without 'Bearer' prefix.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // Make Swagger use JWT Bearer authentication
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);

// Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
