using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Client;
using Gui.Models;
using Gui.Utils;
using Utils.Logging;

namespace Gui.ViewModels
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private static ILog log = LogManager.GetLogger(typeof(MainWindowViewModel));

		private object result;
		public ICommand SubmitCommand => new RelayCommand(Submit);
		public ICommand QueryCommand => new RelayCommand(Query);
		public ICommand FaultCommand => new RelayCommand(Fault);
		public ICommand ExceptCommand => new RelayCommand(Except);

		// should match value in Service.csproj IISUrl
		public string Host { get; set; } = "localhost:50668";

		public string Context { get; set; } = "<Context>";

		public object Result
		{
			get => result;
			set
			{
				result = value;
				OnPropertyChanged();
			}
		}

		private void Submit()
		{
			log.Info("Submit");
			try
			{
				using (IClient client = CreateClient())
				{
					Result = client.Foo(Context);
				}
			}
			catch (Exception e)
			{
				Result = e;
			}
		}

		private void Query()
		{
			log.Info("Query");
			try
			{
				using (IClient client = CreateClient())
				{
					Result = new QueryResult(client.Query(10));
				}
			}
			catch (Exception e)
			{
				Result = e;
			}
		}

		private void Except()
		{
			log.Info("Except");
			try
			{
				using (IClient client = CreateClient())
				{
					client.ThrowException("Except:" + Context);
				}
				Result = "Unexpected";
			}
			catch (Exception e)
			{
				Result = e;
			}
		}

		private void Fault()
		{
			log.Info("Fault");
			try
			{
				using (IClient client = CreateClient())
				{
					client.ThrowFault("Fault:" + Context);
				}
				Result = "Unexpected";
			}
			catch (Exception e)
			{
				Result = e;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private IClient CreateClient()
		{
			return ClientFactory.Create(Host);
		}
	}
}