using System;
using System.ServiceModel;
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

		[Test]
		public void FaultExceptionOnServer()
		{
			IClient client = ClientFactory.Create(host);
			ClientException message = Assert.Throws<ClientException>(() => client.ThrowFault("Xyzzy"));
			Assert.AreEqual("Xyzzy", message.Message);
		}

		[Test]
		public void NonFaultExceptoionOnServer()
		{
			IClient client = ClientFactory.Create(host);
			Exception exception = Assert.Throws<FaultException> (() => client.ThrowException("Plugh!"));
			Assert.That(exception.Message.Contains("The server was unable to process the request due to an internal error"));
		}
	}
}
