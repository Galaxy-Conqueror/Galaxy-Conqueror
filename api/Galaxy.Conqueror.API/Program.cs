using Galaxy.Conqueror.API.Configuration;
using Galaxy.Conqueror.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<GoogleAuthService>();

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