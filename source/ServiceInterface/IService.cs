using System.ServiceModel;

namespace ServiceInterface
{
	[ServiceContract]
	public interface IService
	{
		[OperationContract]
		void Foo(string value);
	}
}
