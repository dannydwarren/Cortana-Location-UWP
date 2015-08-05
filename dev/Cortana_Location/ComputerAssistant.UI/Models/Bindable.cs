using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ComputerAssistant.UI.Models
{
	public class Bindable: INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}