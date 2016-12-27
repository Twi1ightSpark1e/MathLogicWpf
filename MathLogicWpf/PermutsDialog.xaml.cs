using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MathLogicWpf
{
	/// <summary>
	/// Логика взаимодействия для PermutsDialog.xaml
	/// </summary>
	public partial class PermutsDialog : UserControl, INotifyPropertyChanged
	{
		private string _before;
		public string Before
		{
			get
			{
				return _before;
			}
			set
			{
				_before = value;
				if (null != this.PropertyChanged)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("Before"));
				}
			}
		}

		private string _after;
		public string After
		{
			get
			{
				return _after;
			}
			set
			{
				_after = value;
				if (null != this.PropertyChanged)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("After"));
				}
			}
		}

		private bool _final;
		public bool Final
		{
			get
			{
				return _final;
			}
			set
			{
				_final = value;
				if (null != this.PropertyChanged)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("Final"));
				}
			}
		}

		public PermutsDialog()
		{
			InitializeComponent();
			DataContext = this;
			Loaded += (sender, e) => {
				fromTextBox.Focusable = true;
				fromTextBox.Focus();
				Keyboard.Focus(fromTextBox);
			};
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void PermutsDialog_KeyUp(object sender, KeyEventArgs e)
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
