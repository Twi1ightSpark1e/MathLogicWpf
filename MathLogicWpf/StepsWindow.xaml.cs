using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

using static MathLogicWpf.DataClass;

namespace MathLogicWpf
{
	/// <summary>
	/// Логика взаимодействия для StepsWindow.xaml
	/// </summary>
	public partial class StepsWindow : MetroWindow, INotifyPropertyChanged
	{
		public bool IsAlphabetSelected => alphabetListBox.SelectedIndex != -1;
		public bool IsPermutsSelected => permutsListBox.SelectedIndex != -1;
		public bool CanAddAlphabet => addAlphabetTextBox.Text != string.Empty;
		public bool CanAddPermuts => (beforePermutsTextBox.Text != string.Empty) || (afterPermutsTextBox.Text != string.Empty);
		public bool CanStart => inputTextBox.Text != string.Empty;

		private static BindingList<string> permutsList;
		private static BindingList<string> alphabetList;

		public event PropertyChangedEventHandler PropertyChanged;

		public StepsWindow()
		{
			InitializeComponent();
			permutsList = new BindingList<string>(PermutsListBox);
			alphabetList = new BindingList<string>(Alphabet);
			permutsListBox.ItemsSource = permutsList;
			alphabetListBox.ItemsSource = alphabetList;
			DataContext = this;
			alphabetListBox.SelectionChanged += (sender, e) =>
			{
				PropertyChanged(this, new PropertyChangedEventArgs("IsAlphabetSelected"));
			};
			permutsListBox.SelectionChanged += (sender, e) =>
			{
				PropertyChanged(this, new PropertyChangedEventArgs("IsPermutsSelected"));
			};
		}

		public void alphabetTextBox_TextChanged(object sender, RoutedEventArgs e)
		{
			PropertyChanged(this, new PropertyChangedEventArgs("CanAddAlphabet"));
		}

		public void permutsTextBox_TextChanged(object sender, RoutedEventArgs e)
		{
			PropertyChanged(this, new PropertyChangedEventArgs("CanAddPermuts"));
		}

		private void inputTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			PropertyChanged(this, new PropertyChangedEventArgs("CanStart"));
		}

		private void addAlphabetButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void changeAlphabetButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void deleteAlphabetButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void addPermutsButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void changePermutsButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void deletePermutsButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
