using Client.Apis;
using Microsoft.OpenApi.Models;
using Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.AddGrpcClient<Greeter.GreeterClient>(o => o.Address = new Uri("http://localhost:5859"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseGreetEndPoints();
app.Run();
