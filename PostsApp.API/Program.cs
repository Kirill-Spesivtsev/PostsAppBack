using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PostsApp.API.Configuration;
using PostsApp.API.Middlewares;
using PostsApp.Domain.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices(builder);

builder.Logging.ClearProviders();
builder.Logging.AddSerilogConfiguration();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("DefaultCorsPolicy");

app.Run();
