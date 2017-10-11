using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Utils.Logging;

namespace Service.Utils
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class LogParamsAttribute : Attribute, IContractBehavior, IErrorHandler
	{
		private readonly ILog log;

		public LogParamsAttribute(Type type)
		{
			Type = type;
			log = LogManager.GetLogger(Type);
		}

		public Type Type { get; }

		public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
		{
		}

		public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
			DispatchRuntime dispatchRuntime)
		{
			IParameterInspector parameterInspector = new ParameterLogger(log);

			foreach (var operation in dispatchRuntime.Operations)
			{
				operation.ParameterInspectors.Add(parameterInspector);
			}

			dispatchRuntime.ChannelDispatcher.ErrorHandlers.Add(this);
		}

		public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
			ClientRuntime clientRuntime)
		{
		}

		public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint,
			BindingParameterCollection bindingParameters)
		{
		}

		public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
			
		}

		public bool HandleError(Exception error)
		{
			log.Error(error.Message);
			return true;
		}
	}
}