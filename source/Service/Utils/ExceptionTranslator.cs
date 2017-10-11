using System;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using Server;
using ServiceInterface;

namespace Service.Utils
{
	public class ExceptionTranslator : IOperationInvoker
	{
		private readonly IOperationInvoker dispatchOperationInvoker;

		public ExceptionTranslator(IOperationInvoker dispatchOperationInvoker, MethodAttribute methodAttribute)
		{
			this.dispatchOperationInvoker = dispatchOperationInvoker;
		}

		public object[] AllocateInputs()
		{
			return dispatchOperationInvoker.AllocateInputs();
		}

		public object Invoke(object instance, object[] inputs, out object[] outputs)
		{
			try
			{
				return dispatchOperationInvoker.Invoke(instance, inputs, out outputs);
			}
			catch (ServerException e)
			{
				throw new FaultException<ServiceExceptionDetail>(new ServiceExceptionDetail(), e.Message);
			}
		}

		public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
		{
			return dispatchOperationInvoker.InvokeBegin(instance, inputs, callback, state);
		}

		public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
		{
			return dispatchOperationInvoker.InvokeEnd(instance, out outputs, result);
		}

		public bool IsSynchronous => dispatchOperationInvoker.IsSynchronous;
	}
}