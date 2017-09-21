using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Utils.Logging;

namespace Service
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class LogParamsAttribute : Attribute, IContractBehavior
	{
		public LogParamsAttribute(Type type)
		{
			Type = type;
		}

		public Type Type { get; }

		public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
		{
		}

		public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
			DispatchRuntime dispatchRuntime)
		{
			ILog log = LogManager.GetLogger(Type);
			IParameterInspector parameterInspector = new ParameterLogger(log);

			foreach (var operation in dispatchRuntime.Operations)
			{
				operation.ParameterInspectors.Add(parameterInspector);
			}
		}

		public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
			ClientRuntime clientRuntime)
		{
		}

		public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint,
			BindingParameterCollection bindingParameters)
		{
		}
	}
}