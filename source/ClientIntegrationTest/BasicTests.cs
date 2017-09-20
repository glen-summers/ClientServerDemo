using Client;
using NUnit.Framework;

namespace ClientIntegrationTest
{
	[TestFixture]
	public class BasicTests
	{
		[Test]
		public void Foo()
		{
			var client = ClientFactory.Create();
			client.Foo("Hello");
		}
	}
}
