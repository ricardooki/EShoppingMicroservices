using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Api.Products.Teste
{
	public class GetText : ICarterModule
	{
		public void AddRoutes(IEndpointRouteBuilder app)
		{
			
			//monte um mapeamento usando o MapGet teste e passando um parametro que vai se concatenar no retorno
			//exemplo: /teste?parametro=valor
			//o retorno deve ser "teste" + parametro
			//app.MapGet("/teste", () => "teste");
			app.MapGet("/teste/{texto}", (string texto) => { 
				
				return $"teste: {texto}"; 
			});

		}
	}
}
