using System;
using System.Diagnostics;

namespace Utils.Logging
{
	public static class LogManager
	{
		private static readonly TraceSource traceSource = CreateSource();

		public static string LogFileName => ((FileTraceListener) traceSource.Listeners[0]).FullLogFileName;

		public static ILog GetLogger(Type type)
		{
			return new Log(traceSource, type.FullName + FileTraceListener.Delimiter);
		}

		private static TraceSource CreateSource()
		{
			var traceListener = new FileTraceListener {TraceOutputOptions = TraceOptions.DateTime | TraceOptions.ThreadId};
			var source = new TraceSource("Demo");
			source.Listeners.Clear();
			source.Listeners.Add(traceListener);
			source.Switch = new SourceSwitch("Info") {Level = SourceLevels.Information};
			return source;
		}
	}
}