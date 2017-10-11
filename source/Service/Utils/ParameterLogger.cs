using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Dispatcher;
using Utils.Logging;

namespace Service.Utils
{
	internal class ParameterLogger : IParameterInspector
	{
		private readonly ILog log;

		public ParameterLogger(ILog log)
		{
			this.log = log;
		}

		public object BeforeCall(string operationName, object[] inputs)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			log.Info("{0} ==> input [{1}]", operationName, Collate(inputs));
			return stopwatch;
		}

		public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
		{
			var stopwatch = (Stopwatch)correlationState;
			stopwatch.Stop();
			log.Info("{0} <== returned [{2}] output [{3}], Duration: {1} ms", operationName,
				stopwatch.Elapsed.TotalMilliseconds, Format(returnValue), Collate(outputs));
		}

		private static string Collate(object[] items)
		{
			return items == null ? string.Empty : string.Join(", ", items.Select(Format));
		}

		private static string Format(object item)
		{
			return item is Array array && array.Rank == 1
				? $"{array.GetType().GetElementType()}[{array.Length}]"
				: item?.ToString() ?? "(null)";
		}
	}
}