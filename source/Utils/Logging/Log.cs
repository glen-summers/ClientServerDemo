using System.Diagnostics;

namespace Utils.Logging
{
	public class Log : ILog
	{
		private readonly TraceSource source;
		private readonly string prefix;

		public Log(TraceSource source, string prefix)
		{
			this.source = source;
			this.prefix = prefix;
		}

		public void Debug(string message)
		{
			source.TraceEvent(TraceEventType.Verbose, 0, message);
		}

		public void Debug(string format, params object[] arguments)
		{
			source.TraceEvent(TraceEventType.Verbose, 0, format, arguments);
		}

		public void Info(string message)
		{
			source.TraceEvent(TraceEventType.Information, 0, prefix + message);
		}

		public void Info(string format, params object[] arguments)
		{
			source.TraceEvent(TraceEventType.Information, 0, prefix + format, arguments);
		}

		public void Warning(string message)
		{
			source.TraceEvent(TraceEventType.Warning, 0, prefix + message);
		}

		public void Warning(string format, params object[] arguments)
		{
			source.TraceEvent(TraceEventType.Warning, 0, prefix + format, arguments);
		}

		public void Error(string message)
		{
			source.TraceEvent(TraceEventType.Error, 0, prefix + message);
		}

		public void Error(string format, params object[] arguments)
		{
			source.TraceEvent(TraceEventType.Error, 0, prefix + format, arguments);
		}

		public void Critical(string message)
		{
			source.TraceEvent(TraceEventType.Critical, 0, prefix + message);
		}

		public void Critical(string format, params object[] arguments)
		{
			source.TraceEvent(TraceEventType.Critical, 0, prefix + format, arguments);
		}
	}
}