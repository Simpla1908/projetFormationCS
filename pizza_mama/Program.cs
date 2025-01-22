using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using pizza_mama.Data;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration de l'authentification JWT
var key = builder.Configuration.GetValue<string>("Jwt:Secret"); // Clé secrète pour signer le JWT
var issuer = builder.Configuration.GetValue<string>("Jwt:Issuer"); // Émetteur du JWT

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Utilise true en production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero, // Permet une expiration plus stricte
            ValidIssuer = issuer,
            ValidAudience = issuer
        };
    });

// Configuration de la base de données
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();
builder.Services.AddControllers();

var app = builder.Build();

// Configuration du pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // HSTS pour renforcer la sécurité
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Utilisation de l'authentification et de l'autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
