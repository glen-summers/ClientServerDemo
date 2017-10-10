using System.ServiceModel;

namespace ServiceInterface
{
	[ServiceContract]
	public interface IService
	{
		[OperationContract]
		[FaultContract(typeof(ServiceExceptionDetail))]
		string Foo(string value);

		[OperationContract]
		[FaultContract(typeof(ServiceExceptionDetail))]
		string[] Query(int count);

		[OperationContract]
		[FaultContract(typeof(ServiceExceptionDetail))]
		void ThrowFault(string message);

		[OperationContract]
		[FaultContract(typeof(ServiceExceptionDetail))]
		void ThrowException(string message);
	}
}
