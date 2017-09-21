using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ServiceInterface;
using Utils.Logging;

namespace Client
{
	internal class ServiceClient : ClientBase<IService>, IClient
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ServiceClient));

		public ServiceClient(string endpoint) : base(GetBinding(), new EndpointAddress(endpoint))
		{
			log.Info("Ctor {0}", endpoint);

//			try
//			{
//				Channel.Connect();
//			}
//			catch ...
		}

		private static Binding GetBinding()
		{
			var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
			{
				MessageEncoding = WSMessageEncoding.Text,
				MaxReceivedMessageSize = int.MaxValue,
				ReaderQuotas = { MaxArrayLength = int.MaxValue },
				UseDefaultWebProxy = false,
				TransferMode = TransferMode.StreamedResponse
			};
			binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
			return binding;
		}

		public string Foo(string value)
		{
			log.Info("Foo");
			try
			{
				return Channel.Foo(value);
			}
			catch (FaultException<ServiceExceptionDetail> e)
			{
				throw new InvalidOperationException(e.Message, e);
			}
		}
	}
}