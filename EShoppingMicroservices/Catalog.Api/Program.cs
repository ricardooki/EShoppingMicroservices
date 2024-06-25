// See https://aka.ms/new-console-template for more information
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Carter;
using Catalog.Api.Data;
using Catalog.Api.Products.CreateProduct;
using Catalog.Api.Products.DeleteProduct;
using Catalog.Api.Products.GetProductByCategory;
using Catalog.Api.Products.GetProductById;
using Catalog.Api.Products.GetProducts;
using Catalog.Api.Products.Teste;
using Catalog.Api.Products.UpdateProduct;
using FluentValidation;
using HealthChecks.UI.Client;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var assembly = typeof(Program).Assembly;

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
	config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
});

builder.Services.AddMediatR(config =>
{
	config.RegisterServicesFromAssembly(assembly);
	config.AddOpenBehavior(typeof(ValidationBehavior<,>));
	config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();
builder.Services.AddScoped<GetText>();
builder.Services.AddScoped<GetProductsEndpoint>();
builder.Services.AddScoped<GetProductByIdEndpoint>();
builder.Services.AddScoped<GetProductByCategoryEndpoint>();
builder.Services.AddScoped<CreateProductEndpoint>();
builder.Services.AddScoped<UpdateProductEndpoint>();
builder.Services.AddScoped<DeleteProductEndpoint>();

builder.Services.AddMarten(opts =>
{
	opts.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

if (builder.Environment.IsDevelopment())
	builder.Services.InitializeMartenWith<CatalogInitialData>();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
	.AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapCarter();
app.UseExceptionHandler(options => { });
//app.UseHealthChecks("/health",
//	new HealthCheckOptions
//	{
//		ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//	});


app.Run();