namespace Utils.Logging
{
	public interface ILog
	{
		void Debug(string message);
		void Debug(string format, params object[] arguments);

		void Info(string message);
		void Info(string format, params object[] arguments);

		void Warning(string message);
		void Warning(string format, params object[] arguments);

		void Error(string message);
		void Error(string format, params object[] arguments);

		void Critical(string message);
		void Critical(string format, params object[] arguments);
	}
}