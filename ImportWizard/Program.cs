// File: ImportWizard.WebApi/Program.cs

using System;
using System.Text;
using Azure.Messaging.ServiceBus;                         // ← for ServiceBusClient & ServiceBusSender
using ImportWizard.Data;
using ImportWizard.Dtos.Validation;
using ImportWizard.Repositories.Implementations;
using ImportWizard.Repositories.Interfaces;
using ImportWizard.Services;
using ImportWizard.Services.Implementations;
using ImportWizard.Services.Interfaces;
using ImportWizard.WebApi.Filters;                        // for FormFileOperationFilter
using ImportWizard.WebApi.Models;                         // for ValidateRowsRequest
using ImportWizard.WebApi.Services.Implementations;
using ImportWizard.WebApi.Services.Interfaces;
using ImportWizard.Sb.HostedServices;
using ImportWizard.Sb.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;                          // for ApiBehaviorOptions
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;                       // ← for IOptions<T>
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1) CONFIGURE OPTIONS BINDING
builder.Services.Configure<RolesConfig>(
    builder.Configuration.GetSection("RolesConfig"));
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

// ────────────────────────────────────────────────────────────────────────────────
// 🔹 NEW: Service Bus settings
builder.Services.Configure<ServiceBusSettings>(
    builder.Configuration.GetSection("AzureServiceBus"));

// 🔹 NEW: Register the ServiceBusClient
builder.Services.AddSingleton(sp =>
{
    var sbOpts = sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
    return new ServiceBusClient(sbOpts.ConnectionString);
});

// 🔹 NEW: Register the ServiceBusSender for enqueuing messages
builder.Services.AddSingleton(sp =>
{
    var sbOpts = sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
    var client = sp.GetRequiredService<ServiceBusClient>();
    return client.CreateSender(sbOpts.TopicName);
});
// ────────────────────────────────────────────────────────────────────────────────

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

// 3) CONFIGURE JWT AUTHENTICATION
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
        ClockSkew = TimeSpan.FromMinutes(5)
    };
});

// 4) REGISTER REPOSITORIES & SERVICES
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategorySectionRepository, CategorySectionRepository>();
builder.Services.AddScoped<ISectionColumnRepository, SectionColumnRepository>();
builder.Services.AddScoped<ICategoryHierarchyRepository, CategoryHierarchyRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategorySectionService, CategorySectionService>();
builder.Services.AddScoped<ISectionColumnService, SectionColumnService>();
builder.Services.AddScoped<ICategoryHierarchyService, CategoryHierarchyService>();
builder.Services.AddScoped<IImportValidationService, ImportValidationService>();
builder.Services.AddScoped<IImportResultService, ImportResultService>();
builder.Services.AddScoped<IImportInputService, ImportInputService>();
builder.Services.AddScoped<ISaveTemplateService, SaveTemplateService>();

// 🔹 Ensure this comes *after* ServiceBusSender is registered
builder.Services.AddScoped<IMessagePublisher, ServiceBusMessagePublisher>();

// 5) CORS POLICY
builder.Services.AddCors(o => o.AddPolicy("AllowAngular", p =>
    p.WithOrigins("http://localhost:4200",
    "https://importdev-deb0a5byawhrfgd7.eastus-01.azurewebsites.net")
     .AllowAnyHeader()
     .AllowAnyMethod()));

// 6) SUPPRESS AUTOMATIC 400 ON [ApiController] INVALID-MODEL
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);

// 7) CONTROLLERS + SWAGGER + OPERATION FILTER
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.OperationFilter<FormFileOperationFilter>());

var app = builder.Build();

// Seed a default user if missing (dev only)
using (var scope = app.Services.CreateScope())
{
    var um = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    const string testUser = "testuser";
    const string testPwd = "P@ssw0rd!";
    if (await um.FindByNameAsync(testUser) == null)
        await um.CreateAsync(new IdentityUser { UserName = testUser }, testPwd);
}

app.UseCors("AllowAngular");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
