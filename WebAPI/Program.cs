using DataAccess.Context;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Business.Abstract;
using Business.Concrete;
using Business.Mappings;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Circular reference sorununu çözmek için
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        // Null değerleri ignore et
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// CORS Configuration - Frontend ile CORS problemini çözmek için
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            // Production ortamında domain'inizi buraya ekleyin
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Emlak Takip API", 
        Version = "v1",
        Description = "Emlak Takip Sistemi API Dokümantasyonu",
        Contact = new OpenApiContact
        {
            Name = "Emlak Takip",
            Email = "info@emlaktakip.com"
        }
    });

    // JWT Authentication için Swagger konfigürasyonu
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Örnek: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
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
            new string[] { }
        }
    });

    // XML dokümantasyon için (isteğe bağlı)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Database Configuration
builder.Services.AddDbContext<EmlakDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity Configuration
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<EmlakDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var key = Encoding.ASCII.GetBytes(jwtKey!);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
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

// AutoMapper
builder.Services.AddAutoMapper(typeof(CustomerProfile), typeof(PropertyProfile), typeof(SaleProfile));

// Repository Registration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Business Layer Services
builder.Services.AddScoped<IPropertyService, PropertyManager>();
builder.Services.AddScoped<ICustomerService, CustomerManager>();
builder.Services.AddScoped<IAuthService, AuthManager>();
builder.Services.AddScoped<IUserActivityService, UserActivityManager>();
builder.Services.AddScoped<ISaleService, SaleManager>();

// Background Services
builder.Services.AddHostedService<WebAPI.Services.UserActivityBackgroundService>();

var app = builder.Build();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<EmlakDbContext>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Migration'ları uygula
        await context.Database.MigrateAsync();
        
        // Seed data'yı çalıştır (kullanıcılar ve örnek veriler)
        await SeedData.SeedAllAsync(userManager, roleManager, context);
        
        // Delta update: SeedData'da yeni eklenen ama DB'de olmayan verileri ekle
        await SeedData.UpdateSeedDataAsync(context);
        
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Seed data ve delta update başarıyla tamamlandı");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seed data sırasında hata oluştu: {ErrorMessage}", ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Emlak Takip API v1");
        c.RoutePrefix = string.Empty; // Swagger'ı root URL'de göster
    });
}

app.UseRouting();

// CORS middleware'ini ekle - Frontend ile API arasındaki CORS sorununu çözmek için
app.UseCors("AllowFrontend");

// Production ortamında HTTPS redirect kapalı (Render kendi SSL terminasyonunu yapar)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();

// Token validation middleware - JWT token durumunu kontrol eder ve kullanıcı durumunu günceller
app.UseMiddleware<WebAPI.Middleware.TokenValidationMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Health check endpoint for Render
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();



record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
