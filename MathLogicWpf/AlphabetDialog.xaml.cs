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
	/// Логика взаимодействия для AddAlphabetDialog.xaml
	/// </summary>
	public partial class AlphabetDialog : UserControl, INotifyPropertyChanged
	{
		private string _result;
		public string Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
				if (null != this.PropertyChanged)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("Result"));
				}
			}
		}

		public AlphabetDialog()
		{
			InitializeComponent();
			DataContext = this;
			Loaded += (sender, e) => {
				inputTextBox.Focusable = true;
				inputTextBox.Focus();
				Keyboard.Focus(inputTextBox);
			};
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void Alphabet_KeyUp(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			if (e.Key == Key.Enter)
			{
				okButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
			}
			else if (e.Key == Key.Escape)
			{
				cancelButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
			}
		}
	}
}
