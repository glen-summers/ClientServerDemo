using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace Utils.Logging
{
	internal class FileTraceListener : TraceListener
	{
		private const int MaxFileSize = 5000000;
		private const int ReserveDiskSpace = 10000000;
		private const string LogLocalTimeFormat = "dd MMM yyyy, HH:mm:ss.fff";
		private const string HeaderFooterSeparator = "------------------------------------------------";
		private static readonly Encoding encoding = new UTF8Encoding(false);

		private readonly object streamMonitor = new object();
		private StreamWriter streamWriter;
		private string fullFileName;
		private DateTime date;

		public static string Delimiter => " : ";

		public string FullLogFileName
		{
			get
			{
				lock (streamMonitor)
				{
					EnsureStreamIsOpen();
					string path = fullFileName;
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path).Demand();
					return path;
				}
			}
		}

		public override void Write(string message)
		{
			lock (streamMonitor)
			{
				try
				{
					HandleFileRollover();
					EnsureStreamIsOpen();
					if (!ResourcesAvailable(encoding.GetByteCount(message)))
					{
						return;
					}
					streamWriter.Write(message);
				}
				catch
				{
					CloseStream();
					throw;
				}
			}
		}

		public override void WriteLine(string message)
		{
			lock (streamMonitor)
			{
				try
				{
					EnsureStreamIsOpen();
					HandleFileRollover();
					if (!ResourcesAvailable(encoding.GetByteCount(message + Environment.NewLine)))
					{
						return;
					}
					streamWriter.WriteLine(message);
				}
				catch
				{
					CloseStream();
					throw;
				}
			}
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
			{
				return;
			}

			var stringBuilder = new StringBuilder();
			if ((TraceOutputOptions & TraceOptions.DateTime) == TraceOptions.DateTime)
			{
				stringBuilder.Append(eventCache.DateTime.ToLocalTime().ToString(LogLocalTimeFormat, CultureInfo.InvariantCulture));
			}

			if ((TraceOutputOptions & TraceOptions.ThreadId) == TraceOptions.ThreadId)
			{
				stringBuilder.Append(Delimiter);
				stringBuilder.Append("[ ");
				stringBuilder.Append(eventCache.ThreadId.ToString(CultureInfo.InvariantCulture).PadRight(4));
				stringBuilder.Append(" ]");
			}

			if ((TraceOutputOptions & TraceOptions.Timestamp) == TraceOptions.Timestamp)
			{
				stringBuilder.Append(Delimiter);
				stringBuilder.Append(eventCache.Timestamp.ToString(CultureInfo.InvariantCulture));
			}

			stringBuilder.Append(Delimiter);
			stringBuilder.Append(Translate(eventType));

			stringBuilder.Append(Delimiter);
			stringBuilder.Append(message);

			WriteLine(stringBuilder.ToString());
		}

		private string Translate(TraceEventType eventType)
		{
			switch (eventType)
			{
				case TraceEventType.Critical:
					return "CRITICAL";
				case TraceEventType.Error:
					return "ERROR   ";
				case TraceEventType.Warning:
					return "WARNING ";
				case TraceEventType.Information:
					return "INFO    ";
				case TraceEventType.Verbose:
					return "DEBUG   ";
				default:
					return ((int)eventType).ToString(CultureInfo.InvariantCulture);
			}
		}

		// ReSharper disable MethodOverloadWithOptionalParameter
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			TraceEvent(eventCache, source, eventType, id, String.Format(CultureInfo.InvariantCulture, format, args));
		}

		public override void Flush()
		{
			lock (streamMonitor)
			{
				streamWriter?.Flush();
			}
		}

		public override void Close()
		{
			Dispose(true);
		}

		protected override void Dispose(bool disposing)
		{
			lock (streamMonitor)
			{
				if (disposing)
				{
					CloseStream();
				}
			}
		}

		private void EnsureStreamIsOpen()
		{
			if (streamWriter == null)
			{
				streamWriter = GetStream();
			}
		}

		private void HandleFileRollover()
		{
			if (streamWriter!=null && (streamWriter.BaseStream.Length >= MaxFileSize || DateTime.Compare(date, DateTime.Now.Date) != 0))
			{
				CloseStream();
			}
		}

		private bool ResourcesAvailable(long newEntrySize)
		{
			return streamWriter.BaseStream.Length + newEntrySize <= MaxFileSize &&
			       GetFreeDiskSpace() - newEntrySize >= ReserveDiskSpace;
		}

		private long GetFreeDiskSpace()
		{
			string pathRoot = Path.GetPathRoot(Path.GetFullPath(fullFileName));
			return new DriveInfo(pathRoot).AvailableFreeSpace;
		}

		private void CloseStream()
		{
			if (streamWriter != null)
			{
				try
				{
					WriteFooter(streamWriter);
				}
				catch (IOException)
				{}
				try
				{
					streamWriter.Close();
				}
				catch (IOException)
				{}
				streamWriter = null;
			}
		}

		private StreamWriter GetStream()
		{
			string path = Path.GetTempPath();
			Process currentProcess = Process.GetCurrentProcess();
			var baseFileName = currentProcess.ProcessName + "_" + currentProcess.Id.ToString(CultureInfo.InvariantCulture);

			DateTime day = DateTime.Now.Date;
			var fileName = baseFileName + "_" + day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
			var logFileName = Path.Combine(path, fileName);

			StreamWriter newStreamWriter = null;
			for (int num = 0; newStreamWriter == null && num < Int32.MaxValue; ++num)
			{
				string str = num != 0
					? Path.GetFullPath(logFileName + "_" + num.ToString(CultureInfo.InvariantCulture) + ".log")
					: Path.GetFullPath(logFileName + ".log");

				if (File.Exists(str))
				{
					continue;
				}

				try
				{
					Stream stream = File.Open(str, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
					newStreamWriter = new StreamWriter(stream, encoding) {AutoFlush = true};
					WriteHeader(newStreamWriter);
					fullFileName = str;
					date = day;
					return newStreamWriter;
				}
				catch (IOException)
				{}
			}
			throw new InvalidOperationException("Exhausted possible stream names " + Path.GetFullPath(logFileName));
		}

		private void WriteHeader(StreamWriter writer)
		{
			Process currentProcess = Process.GetCurrentProcess();
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			double precision = NativeMethods.TimePrecision;
			WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();

			writer.WriteLine(HeaderFooterSeparator);
			writer.WriteLine("Opened      : {0:dd MMM yyyy, HH:mm:ss (zzz)}", DateTime.Now);
			writer.WriteLine("OpenedUtc   : {0:u}", DateTime.UtcNow);
			writer.WriteLine("ProcessName : {0} ({1} bit)", currentProcess.ProcessName, Environment.Is64BitProcess ? 64 : 32);
			writer.WriteLine("ProcessId   : {0}", currentProcess.Id);
			writer.WriteLine("ClrVersion  : {0}", Environment.Version);
			writer.WriteLine("UserName    : {0}\\{1}", Environment.UserDomainName, Environment.UserName);
			if (windowsIdentity != null)
			{
				writer.WriteLine("SID         : {0}", windowsIdentity.User);
			}
			if (entryAssembly != null)
			{
				writer.WriteLine("CodeBase    : {0}",
					Path.GetFullPath(Uri.UnescapeDataString(new UriBuilder(entryAssembly.EscapedCodeBase).Path)));
			}
			else
			{
				writer.WriteLine("Executable  : {0}", commandLineArgs[0]);
			}
			writer.WriteLine("CommandLine : {0}", string.Join(" ", commandLineArgs.Skip(1)));

			writer.WriteLine("MachineName : {0}", Environment.MachineName);
			writer.WriteLine("Processors  : {0}", Environment.ProcessorCount);
			writer.WriteLine("System      : {0} ({1} bit)", Environment.OSVersion, Environment.Is64BitOperatingSystem ? 64 : 32);
			writer.WriteLine("Precision   : {0:0.####} ms", precision * 1000);

			writer.WriteLine(HeaderFooterSeparator);
		}

		private void WriteFooter(StreamWriter writer)
		{
			writer.WriteLine(HeaderFooterSeparator);
			writer.WriteLine("Closed      {0:dd MMM yyyy, HH:mm:ss (zzz)}", DateTime.Now);
			writer.WriteLine(HeaderFooterSeparator);
		}

		internal static class NativeMethods
		{
			[DllImport("ntdll.dll")]
			private static extern int NtQueryTimerResolution(out int minimumResolution, out int maximumResolution, out int actualResolution);

			public static double TimePrecision
			{
				get
				{
					int minimumResolution;
					int maximumResolution;
					int actualResolution;
					NtQueryTimerResolution(out minimumResolution, out maximumResolution, out actualResolution);
					return actualResolution / 1.0e7;
				}
			}
		}
	}
}