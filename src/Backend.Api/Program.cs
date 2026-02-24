using Backend.Modules.Order;
using System.Text;
using Backend.Modules.Auth;
using Backend.Modules.SomeEntity;
using Backend.Modules.SomeEntity.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Backend.Modules.Order;
using Microsoft.OpenApi;
using System.Security.Claims;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// JWT
var secret   = builder.Configuration["JWT_SECRET"]!;
var issuer   = builder.Configuration["JWT_ISSUER"]!;
var audience = builder.Configuration["JWT_AUDIENCE"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = issuer,
            ValidAudience            = audience,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };
    });

builder.Services.AddAuthorization();

// Modules
builder.Services.AddEntityModules(builder.Configuration);
builder.Services.AddOrdersModules(builder.Configuration);
builder.Services.AddAuthModule(builder.Configuration);



builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend.Api v1"));
}

//Migrations
app.ApplyAuthMigrations();
app.ApplyOrderMigrations();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
// app.UseHttpsRedirection();
app.MapControllers();

app.Run();