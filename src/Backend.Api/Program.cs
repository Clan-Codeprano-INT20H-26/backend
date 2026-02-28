using System.Text;
using Backend.Module.Kit;
using Backend.Module.Kit.Infrastructure;
using Backend.Module.Tax;
using Backend.Module.Tax.Infrastructure;
using Backend.Modules.Auth;
using Backend.Modules.Auth.Infrastructure;
using Backend.Modules.Order;
using Backend.Modules.Shared;
using Backend.Modules.Payment;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// JWT

// Modules
builder.Services.AddOrdersModules(builder.Configuration);
builder.Services.AddAuthModule(builder.Configuration);
builder.Services.AddTaxModules(builder.Configuration);
builder.Services.AddKitModule(builder.Configuration);
builder.Services.AddSharedModule(builder.Configuration);
builder.Services.AddPaymentModule(builder.Configuration);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend.Api v1"));

if (app.Environment.IsDevelopment())
{}

//Migrations
app.ApplyAuthMigrations();
app.ApplyOrderMigrations();
app.ApplyTaxMigrations();
app.ApplyKitMigrations();   

//for tax
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAllAsync();
    var kitSeeder = scope.ServiceProvider.GetRequiredService<KitSeeder>();
    await kitSeeder.SeedAsync();
    var adminSeeder = scope.ServiceProvider.GetRequiredService<AdminSeeder>();
    await adminSeeder.SeedAsync();
}


app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
// app.UseHttpsRedirection();
app.MapControllers();

app.Run();