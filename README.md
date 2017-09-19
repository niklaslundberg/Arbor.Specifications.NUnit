# Arbor.Specifications.NUnit

Arbor.Specifications.NUnit is a context/specification library for use with the NUnit testing framework based on https://github.com/derekgreer/nunit.specifications.

# Quickstart

The following is an example NUnit test written using the ContextSpecification base class from NUnit.Specifications:

```C#
using NUnit.Framework;
using Arbor.Specifications.NUnit;

public class OrderSpecs
{
	[Subject("Order Processing")]
	public class OrderSpecification : ContextSpecification
	{
		static OrderService _orderService;
		static bool _results;
		static Order _order;
		
		Given an_order_is_created = async () =>
		{
			_orderService = new OrderService();
			_order = new Order();
		};
		
		When placing_order = async () => _results = await _orderService.PlaceOrderAsync(_order);

		It should_successfully_place_the_order = async () => Assert.IsTrue(_results);
	}
}
```
