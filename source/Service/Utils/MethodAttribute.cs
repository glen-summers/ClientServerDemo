using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Service.Utils
{
	public class MethodAttribute : Attribute, IOperationBehavior
	{
		public void Validate(OperationDescription operationDescription)
		{
		}

		public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
		{
			dispatchOperation.Invoker = new ExceptionTranslator(dispatchOperation.Invoker, this);
		}

		public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
		{
		}

		public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
		{
		}
	}
}