using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Client;
using Gui.Models;
using Gui.Utils;

namespace Gui.ViewModels
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private object result;
		public ICommand SubmitCommand => new RelayCommand(Submit);
		public ICommand QueryCommand => new RelayCommand(Query);
		public ICommand FaultCommand => new RelayCommand(Fault);
		public ICommand ExceptCommand => new RelayCommand(Except);

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
			try
			{
				Result = ClientFactory.Create(Host).Foo(Context);
			}
			catch (Exception e)
			{
				Result = e.ToString();
			}
		}

		private void Query()
		{
			try
			{
				Result = new QueryResult(ClientFactory.Create(Host).Query(10));
			}
			catch (Exception e)
			{
				Result = e.ToString();
			}
		}

		private void Except()
		{
			try
			{
				ClientFactory.Create(Host).ThrowException("Except:" + Context);
				Result = "Unexpected";
			}
			catch (Exception e)
			{
				Result = e;
			}
		}

		private void Fault()
		{
			try
			{
				ClientFactory.Create(Host).ThrowFault("Fault:" + Context);
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
	}
}