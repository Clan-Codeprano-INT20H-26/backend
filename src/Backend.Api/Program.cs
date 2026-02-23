using Backend.Modules.SomeEntity;
using Backend.Modules.SomeEntity.Infrastructure;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddEntityModules(builder.Configuration);

builder.Services.AddDbContext<SomeEntityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

if (string.IsNullOrEmpty(builder.Configuration.GetConnectionString("DefaultConnection")))
{
    throw new InvalidOperationException("String connection 'DefaultConnection' is not found.");
}

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend.Api v1"));
}

app.UseHttpsRedirection();


app.Run();