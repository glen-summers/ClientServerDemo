namespace Client
{
	public static class ClientFactory
	{
		private const string Url = "http://localhost:50124/Service.svc";

		public static IClient Create()
		{
			return new ServiceClient(Url);
		}
	}
}

	