namespace Gui.Models
{
	internal class QueryResult
	{
		public QueryResult(string[] result)
		{
			Result = result;
		}

		public string[] Result { get; }
	}
}