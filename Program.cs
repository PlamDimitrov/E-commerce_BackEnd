using Microsoft.EntityFrameworkCore;
using ecommerce_API.Data;
using ecommerce_API.Extensions;
using ecommerce_API.Helpers;
using ecommerce_API.JwtHelpers;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ecommerce_APIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ecommerce_APIContext") ?? throw new InvalidOperationException("Connection string 'ecommerce_APIContext' not found.")));

builder.Services.AddJWTTokenServices(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(x => x
        .WithOrigins("http://localhost:7044/")
        .AllowCredentials()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true)
    );
}

app.UseCookiePolicy();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PeriodicallyCaller.Call(43200000, JwtHelpers.CleanJwtObsoleteTokenFromDatabase);

app.Run();
