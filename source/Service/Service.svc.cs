using ServiceInterface;
using Utils.Logging;

namespace Service
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
	[LogParams(typeof(Service))]
	public class Service : IService
	{
		public string Foo(string value)
		{
			return "ret:" + value;
		}
	}
}
