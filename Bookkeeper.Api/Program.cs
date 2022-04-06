var builder = WebApplication.CreateBuilder(args);


// Add necessary services

builder.Services.AddControllers();


// Build App

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
