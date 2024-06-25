using Carter;
using Catalog.Api.Models;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Api.Products.GetProducts
{
	public record GetProductsRequest(int? PageNumber = 1, int? PageSize = 10);
	public record GetProductsResponse(IEnumerable<Product> Products);

	public class GetProductsEndpoint : ICarterModule
	{
		public void AddRoutes(IEndpointRouteBuilder app)
		{
			app.MapGet("/products", async ([AsParameters] GetProductsRequest request, ISender sender) =>
			{
				try
				{
					var query = request.Adapt<GetProductsQuery>();

					var result = await sender.Send(query);

					var response = result.Adapt<GetProductsResponse>();

					return Results.Ok(response);
				}
				catch (Exception ex)
				{
					throw;
				}
				
			})
			.WithName("GetProducts")
			.Produces<GetProductsResponse>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Get Products")
			.WithDescription("Get Products");
		}
	}
}
