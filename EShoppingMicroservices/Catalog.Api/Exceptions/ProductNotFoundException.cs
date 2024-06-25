using BuildingBlocks.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Api.Exceptions
{
	public class ProductNotFoundException : NotFoundException
	{
		public ProductNotFoundException(Guid Id) : base("Product", Id)
		{
		}
	}
}
