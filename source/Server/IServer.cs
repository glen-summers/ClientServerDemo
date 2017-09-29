namespace Server
{
	public interface IServer
	{
		string Foo(string value);

		string[] Query(int count);
	}
}