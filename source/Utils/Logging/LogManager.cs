using System;
using System.Diagnostics;
using System.Reflection;

namespace Utils.Logging
{
	public static class LogManager
	{
		private static readonly TraceSource traceSource = CreateSource();
		private static readonly ILog log = GetLogger(typeof(LogManager));

		public static string LogFileName => ((FileTraceListener) traceSource.Listeners[0]).FullLogFileName;

		static LogManager()
		{
			AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
			foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				Log(loadedAssembly);
			}

			AppDomain.CurrentDomain.UnhandledException += UnhandledException;
			AppDomain.CurrentDomain.DomainUnload+= DomainUnload;
			AppDomain.CurrentDomain.ProcessExit+= ProcessExit;
		}

		public static ILog GetLogger(Type type)
		{
			return GetLogger(type.FullName);
		}

		public static ILog GetLogger(string name)
		{
			return new Log(traceSource, name + FileTraceListener.Delimiter);
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

		private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			Log(args.LoadedAssembly);
		}

		private static void Log(Assembly assembly)
		{
			AssemblyName name = assembly.GetName();
			var fileVersionAttribute = (AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyFileVersionAttribute));
			var fileVersion = fileVersionAttribute == null ? "none" : fileVersionAttribute.Version;

			log.Debug(
				"Loaded: Name=[{0,-40}], FileVersion:[{1,-12}], AssemblyVersion:[{2,-12}], GAC:[{3}], Runtime:[{4}], FullName:[{5}], Location:[{6}]",
				name.Name, fileVersion, name.Version, assembly.GlobalAssemblyCache, assembly.ImageRuntimeVersion, name.FullName,
				assembly.Location);
		}

		private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			log.Critical(e.ExceptionObject.ToString());
		}

		private static void DomainUnload(object sender, EventArgs e)
		{
			log.Info("DomainUnload");
		}

		private static void ProcessExit(object sender, EventArgs e)
		{
			foreach (IDisposable listener in traceSource.Listeners)
			{
				listener.Dispose();
			}
		}
	}
}