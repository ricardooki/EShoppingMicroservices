using Basket.Api.Models;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace Basket.Api.Data
{

	public class CachedBasketRepository : IBasketRepository	
	{
		private readonly IBasketRepository repository;
		private readonly IDistributedCache cache;
		public CachedBasketRepository(IBasketRepository repository, 
			IDistributedCache cache)
        {
			this.repository = repository;
			this.cache = cache;
		}

        public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
		{
			try
			{
				var connection = ConnectionMultiplexer.Connect("localhost:6379");
				var db = connection.GetDatabase();

				//db.StringSet("testKey", "testValue");
				//var value = db.StringGet("testKey");

				var cachedBasket = db.StringGet(userName);  //await cache.GetStringAsync(userName, cancellationToken);
				if (!string.IsNullOrEmpty(cachedBasket))
					return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;

				var basket = await repository.GetBasket(userName, cancellationToken);
				await cache.SetStringAsync(userName, JsonSerializer.Serialize(basket), cancellationToken);
				return basket;
			}
			catch (Exception ex)
			{

				throw;
			}
			
		}

		public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
		{
			await repository.StoreBasket(basket, cancellationToken);

			await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket), cancellationToken);

			return basket;
		}

		public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
		{
			await repository.DeleteBasket(userName, cancellationToken);

			await cache.RemoveAsync(userName, cancellationToken);

			return true;
		}
	}

}
