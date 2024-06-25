using Basket.Api.Basket.CheckoutBasket;
using Basket.Api.Basket.DeleteBasket;
using Basket.Api.Basket.GetBasket;
using Basket.Api.Basket.StoreBasket;
using Basket.Api.Data;
using Basket.Api.Models;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Carter;
using Discount.Grpc;
using Marten;
using StackExchange.Redis;
using System.Text.Json;
using BuildingBlocks.Messaging.MassTransit;
using MassTransit;
using MassTransit.Transports;

var builder = WebApplication.CreateBuilder(args);

//var connection = ConnectionMultiplexer.Connect("localhost:6379");
//var db = connection.GetDatabase();

//db.StringSet("testKey", "testValue");
//var value = db.StringGet("testKey");

//Console.WriteLine(value); // Deve imprimir "testValue"
//						  // Add services to the container.
//						  //Application Services
var cart = new ShoppingCart
{
	UserName = "User1",
	Items = new List<ShoppingCartItem>
		{
			new ShoppingCartItem { ProductId = new Guid(), Quantity = 1, Color = "Red", Price = 10.0M },
			new ShoppingCartItem { ProductId = new Guid(), Quantity = 2, Color = "Blue", Price = 20.0M },
            // Adicione mais itens conforme necessário
        }
};
var connection = ConnectionMultiplexer.Connect("localhost:6379");
var db = connection.GetDatabase();

db.StringSet("testKey2", JsonSerializer.Serialize(cart));
var assembly = typeof(Program).Assembly;
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
	config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
});

builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
	config.RegisterServicesFromAssembly(assembly);
	config.AddOpenBehavior(typeof(ValidationBehavior<,>));
	config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
//Data Services
builder.Services.AddMarten(opts =>
{
	opts.Connection(builder.Configuration.GetConnectionString("Database")!);
	opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();

builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = builder.Configuration.GetConnectionString("Redis");
	options.InstanceName = "Basket";
});

//Grpc Services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
	options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return handler;
});

//Async Communication Services
builder.Services.AddMessageBroker(builder.Configuration);

builder.Services.AddScoped<GetBasketEndpoints>();
builder.Services.AddScoped<CheckoutBasketEndpoints>();
builder.Services.AddScoped<DeleteBasketEndpoints>();
builder.Services.AddScoped<StoreBasketEndpoints>();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();
builder.Services.AddScoped<PublishEndpoint>();
//builder.Services.AddScoped<PublishEndpointProvider>();


//builder.Services.AddMassTransit(x =>
//{
//    x.UsingRabbitMq((context, cfg) =>
//    {
//        cfg.Host("rabbitmq://localhost", h =>
//        {
//            h.Username("guest");
//            h.Password("guest");
//        });

//        // Configurações adicionais do RabbitMQ e MassTransit, se necessário
//    });
//});

//// Isso garante que o MassTransit adiciona IPublishEndpoint ao contêiner de injeção de dependência
//builder.Services.AddMassTransitHostedService();


//Cross-Cutting Services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();
app.MapCarter();
app.UseExceptionHandler(options => { });
// Configure the HTTP request pipeline.

app.Run();

