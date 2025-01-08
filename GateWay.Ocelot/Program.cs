using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using System.Reflection;
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile($"{AppContext.BaseDirectory}/Config/ocelot.consul.json", true, true);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddOcelot()
    .AddConsul()
    .AddPolly()
    ;
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync($"Hello Ocelot,{app.Environment.EnvironmentName}!");
})
.WithOpenApi();
app.UseOcelot().Wait();
app.Run();
