using Client;
using NUnit.Framework;

namespace ClientIntegrationTest
{
	[TestFixture]
	public class BasicTests
	{
		private string host = "localhost:50668";

		[Test]
		public void Foo()
		{
			IClient client = ClientFactory.Create(host);
			client.Foo("Hello");
		}
	}
}
