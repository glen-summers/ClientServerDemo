using System.ServiceModel;
using Client;
using NUnit.Framework;

namespace ClientIntegrationTest
{
	[TestFixture]
	public class BasicTests
	{
		// should match value in Service.csproj IISUrl
		private string host = "localhost:50668";

		[Test]
		public void Foo()
		{
			using (IClient client = ClientFactory.Create(host))
			{
				client.Foo("Hello");
			}
		}

		[Test]
		public void FaultExceptionOnServer()
		{
			string message;
			using (IClient client = ClientFactory.Create(host))
			{
				message = Assert.Throws<ClientException>(() => client.ThrowFault("Xyzzy")).Message;
			}
			Assert.AreEqual("Xyzzy", message);
		}

		[Test]
		public void NonFaultExceptoionOnServer()
		{
			string message;
			using (IClient client = ClientFactory.Create(host))
			{
				message = Assert.Throws<FaultException>(() => client.ThrowException("Plugh!")).Message;
			}
			Assert.That(message.Contains("The server was unable to process the request due to an internal error"));
		}
	}
}
