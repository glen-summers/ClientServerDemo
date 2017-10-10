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
		}

		private static Binding GetBinding()
		{
			var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
			{
				MessageEncoding = WSMessageEncoding.Text,
				MaxReceivedMessageSize = int.MaxValue,
				ReaderQuotas = { MaxArrayLength = int.MaxValue },
				UseDefaultWebProxy = true,
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
				log.Warning("ExceptionFault: {0} : {1}", e.GetType(), e.Message);
				throw new ClientException(e.Message);
			}
			catch (Exception e)
			{
				log.Warning("Exception: {0} : {1}", e.GetType(), e.Message);
				throw;
			}
		}

		public string[] Query(int count)
		{
			log.Info("Query");
			try
			{
				return Channel.Query(count);
			}
			catch (FaultException<ServiceExceptionDetail> e)
			{
				log.Warning("ExceptionFault: {0} : {1}", e.GetType(), e.Message);
				throw new ClientException(e.Message);
			}
			catch (Exception e)
			{
				log.Warning("Exception: {0} : {1}", e.GetType(), e.Message);
				throw;
			}
		}

		public void ThrowFault(string message)
		{
			log.Info("ThrowFault");
			try
			{
				Channel.ThrowFault(message);
			}
			catch (FaultException<ServiceExceptionDetail> e)
			{
				log.Warning("ExceptionFault: {0} : {1}", e.GetType(), e.Message);
				throw new ClientException(e.Message);
			}
			catch (Exception e)
			{
				log.Warning("Exception: {0} : {1}", e.GetType(), e.Message);
				throw;
			}
		}

		public void ThrowException(string message)
		{
			log.Info("ThrowException");
			try
			{
				Channel.ThrowException(message);
			}
			catch (FaultException<ServiceExceptionDetail> e)
			{
				log.Warning("ExceptionFault: {0} : {1}", e.GetType(), e.Message);
				throw new ClientException(e.Message);
			}
			catch (Exception e)
			{
				log.Warning("Exception: {0} : {1}", e.GetType(), e.Message);
				throw;
			}
		}
	}
}