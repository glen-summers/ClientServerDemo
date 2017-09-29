using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Client;

namespace Gui
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private object result;
		public ICommand SubmitCommand => new RelayCommand(Submit);
		public ICommand QueryCommand => new RelayCommand(Query);

		public string Host { get; set; } = "localhost:50668";

		public string Context { get; set; }

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

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}