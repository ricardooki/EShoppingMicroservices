using Basket.Api.Dto;
using Carter;
using Mapster;
using MediatR;

namespace Basket.Api.Basket.CheckoutBasket
{
	public record CheckoutBasketRequest(BasketCheckoutDto BasketCheckoutDto);
	//public record CheckoutBasketRequest(TesteDto BasketCheckoutDto);
	public record CheckoutBasketResponse(bool IsSuccess);

	public class TesteDto()
	{
		public string UserName { get; set; }
		//public Guid CustomerId { get; set; } = default!;
		public decimal TotalPrice { get; set; }

		// Shipping and BillingAddress
		//public string FirstName { get; set; }
		//public string LastName { get; set; } = default!;
		//public string EmailAddress { get; set; } = default!;
		//public string AddressLine { get; set; } = default!;
		//public string Country { get; set; } = default!;
		//public string State { get; set; } = default!;
		//public string ZipCode { get; set; } = default!;

		// Payment
		//public string CardName { get; set; } = default!;
		//public string CardNumber { get; set; } = default!;
		//public string Expiration { get; set; } = default!;
		//public string CVV { get; set; } = default!;
		//public int PaymentMethod { get; set; } = default!;
	}

	public class CheckoutBasketEndpoints : ICarterModule
	{
		public void AddRoutes(IEndpointRouteBuilder app)
		{
			app.MapPost("/basket/checkout", async (CheckoutBasketRequest request, ISender sender) =>
			{
				var command = request.Adapt<CheckoutBasketCommand>();

				var result = await sender.Send(command);

				var response = result.Adapt<CheckoutBasketResponse>();

				return Results.Ok(response);
			})
			.WithName("CheckoutBasket")
			.Produces<CheckoutBasketResponse>(StatusCodes.Status201Created)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Checkout Basket")
			.WithDescription("Checkout Basket");
		}
	}
}
