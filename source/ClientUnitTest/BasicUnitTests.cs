using NUnit.Framework;

namespace ClientUnitTest
{
	[TestFixture]
	public class BasicUnitTests
	{
		[Test]
		public void Test1()
		{
			var x = new Client.ServiceClient("http://localhost:666/Baad.svc");
		}
	}
}