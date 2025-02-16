// Then build and run
using _2_GasTongz.Application;
using _2_GasTongz.Application.Interfaces;
using _3_GasTongz.Infrastructure.Repos;
using _1_GasTongz;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using _3_GasTongz.Infrastructure.DbPersistance;
using _3_GasTongz.Infrastructure;
using _4_GasTongz.API.Middleware;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddInfrastructureServices();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*
The AssemblyMarker is a simple, empty class in your Application layer that serves as a stable reference point for .NET to locate the assembly (DLL) where your MediatR handlers (commands/queries) live. Here's why it's necessary:

Decoupling Layers:

Your API project references the Application project, but the Application project doesn’t need to reference the API.

By using typeof(Application.AssemblyMarker).Assembly, you’re telling MediatR: “Scan the assembly where this marker class lives (the Application layer) to find handlers.”

Avoid Magic Strings:

Without the marker, you’d need to hardcode the assembly name as a string (e.g., Assembly.Load("Application")), which is brittle and prone to typos or renaming issues.
 
 */
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
builder.Services.AddScoped<DapperContext>();


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy => policy
            .WithOrigins("http://localhost:5173") // Update to your frontend URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();


app.UseCors("AllowReact");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
