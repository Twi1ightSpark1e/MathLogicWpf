using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MathLogicWpf
{
	/// <summary>
	/// Логика взаимодействия для CustomMessageDialog.xaml
	/// </summary>
	public partial class CustomMessageDialog : UserControl, INotifyPropertyChanged
	{
		private string _title;
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
				if (null != this.PropertyChanged)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("Title"));
				}
			}
		}

		private string _message;
		public string Message
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
				if (null != this.PropertyChanged)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("Message"));
				}
			}
		}

		public CustomMessageDialog()
		{
			InitializeComponent();
			DataContext = this;
			Loaded += (sender, e) => {
				okButton.Focusable = true;
				okButton.Focus();
				Keyboard.Focus(okButton);
			};
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
