using System.ServiceModel;

namespace ServiceInterface
{
	[ServiceContract]
	public interface IService
	{
		[OperationContract]
		string Foo(string value);
	}
}
