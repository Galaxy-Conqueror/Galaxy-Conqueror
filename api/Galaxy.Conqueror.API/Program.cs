using System.Data;
using Npgsql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Galaxy.Conqueror.API.Configuration;
using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Configuration.Database;

var builder = WebApplication.CreateBuilder(args);

// Allows us to maintain C# naming conventions
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// Setup DB connection
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://accounts.google.com";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://accounts.google.com",
            ValidateAudience = true,
            ValidAudience = "281509747189-8hu7g0egsp28dv5q7mhfgksu802mo03v.apps.googleusercontent.com",
            ValidateLifetime = true,
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpClient<GoogleAuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<SpaceshipService>();
builder.Services.AddScoped<PlanetService>();
builder.Services.AddScoped<BattleService>();
builder.Services.AddScoped<ResourceExtractorService>();
builder.Services.AddScoped<TurretService>();
builder.Services.AddScoped<IAiService>();

builder.Services.AddScoped<ISetupService, SetupService>();

builder.Services.AddHostedService<ResourceUpdaterService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Adds endpoints registered in app config
app.ConfigureApp();

app.Run();