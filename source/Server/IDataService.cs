namespace Server
{
	internal interface IDataService
	{
		string GetVersion();
		int Insert(string value);
		string[] Query(int count);
	}
}
