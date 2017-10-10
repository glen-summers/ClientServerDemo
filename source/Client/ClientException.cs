using System;
using System.Runtime.Serialization;

namespace Client
{
	public class ClientException : Exception
	{
		public ClientException(string message) : base(message)
		{
		}

		protected ClientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}