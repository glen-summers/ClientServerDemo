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
			IClient client = ClientFactory.Create();
			client.Foo("Hello");
		}
	}
}
