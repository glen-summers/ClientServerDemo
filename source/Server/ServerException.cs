using System;

namespace Server
{
	public class ServerException : Exception
	{
		public ServerException(string message) : base(message)
		{
		}
	}
}
