using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Client;

namespace Gui
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private string result;
		public ICommand Clicked => new RelayCommand(OnClicked);

		public string Host { get; set; } = "localhost:50668";

		public string Context { get; set; }

		public string Result
		{
			get => result;
			set
			{
				result = value;
				OnPropertyChanged();
			}
		}

		private void OnClicked()
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

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}