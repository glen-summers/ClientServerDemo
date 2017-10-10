namespace Client
{
	public interface IClient
	{
		string Foo(string value);
		string[] Query(int count);
		void ThrowFault(string message);
		void ThrowException(string message);
	}
}