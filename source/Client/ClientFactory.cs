namespace Client
{
	public static class ClientFactory
	{
		private const string Url = "http://{0}/Service.svc";

		public static IClient Create(string host)
		{
			return new ServiceClient(string.Format(Url, host));
		}
	}
}

	