// Program.cs

using ImportWizard.Data;
using ImportWizard.Dtos.Validation;
using ImportWizard.Repositories.Implementations;
using ImportWizard.Repositories.Interfaces;
using ImportWizard.Services.Implementations;
using ImportWizard.Services.Interfaces;
using ImportWizard.WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;                // ← for ApiBehaviorOptions
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RolesConfig>(
    builder.Configuration.GetSection("RolesConfig"));

// 1) BIND JwtSettings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// 2) ADD DbContext + Identity
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(opts =>
    {
        opts.Password.RequireDigit = false;
        opts.Password.RequiredLength = 6;
        opts.Password.RequireNonAlphanumeric = false;
        opts.Password.RequireUppercase = false;
        opts.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 3) CONFIGURE JWT AUTH
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

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
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ClockSkew = TimeSpan.Zero
    };
});

// 4) EXISTING REPOS & SERVICES
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategorySectionRepository, CategorySectionRepository>();
builder.Services.AddScoped<ISectionColumnRepository, SectionColumnRepository>();
builder.Services.AddScoped<ICategoryHierarchyRepository, CategoryHierarchyRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategorySectionService, CategorySectionService>();
builder.Services.AddScoped<ISectionColumnService, SectionColumnService>();
builder.Services.AddScoped<ICategoryHierarchyService, CategoryHierarchyService>();

// 5) CORS, Controllers, Swagger
builder.Services.AddCors(o => o.AddPolicy("AllowAngular", p =>
    p.WithOrigins("http://localhost:4200")
     .AllowAnyHeader()
     .AllowAnyMethod()));

// ── ADD THIS to suppress automatic 400 on model‐state errors ──
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Create default test user if missing
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider
                           .GetRequiredService<UserManager<IdentityUser>>();
    const string testUser = "testuser";
    const string testPwd = "P@ssw0rd!";

    if (await userManager.FindByNameAsync(testUser) == null)
    {
        await userManager.CreateAsync(
            new IdentityUser { UserName = testUser },
            testPwd
        );
    }
}

app.UseCors("AllowAngular");

if (app.Environment.IsDevelopment())
{
    
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// authentication → authorization
app.UseAuthentication();
app.UseAuthorization();

// Map all your [ApiController]s (including ImportValidationController)
app.MapControllers();

app.Run();
