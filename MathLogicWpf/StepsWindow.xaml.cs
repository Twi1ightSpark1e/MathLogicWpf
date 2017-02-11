using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
		public bool CanAddAlphabet => alphabetTextBox.Text != string.Empty;
		public bool CanAddPermuts => (beforePermutsTextBox.Text != string.Empty) || (afterPermutsTextBox.Text != string.Empty);
		public bool CanStart => inputTextBox.Text != string.Empty;

		public string Text
		{
			get
			{
				return DataClass.Text;
			}
			set
			{
				DataClass.Text = value;
				if (null != this.PropertyChanged)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("Text"));
				}
			}
		}

		private static BindingList<string> permutsList;
		private static BindingList<string> alphabetList;

		public event PropertyChangedEventHandler PropertyChanged;

		public StepsWindow()
		{
			Owner = MainWindow.LastInstance;
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

		private void AlphabetAddChange(Action successAction)
		{
			string result = alphabetTextBox.Text;
			if (result.Contains(".."))
			{
				try
				{
					DirtyWork.CharRange(result[0], result[3]);
				}
				catch
				{
					DirtyWork.ShowCustomMessageDialog(this, "Ошибка!", "Выбрано неправильное множество символов!");
					return;
				}
			}
			successAction.Invoke();
			alphabetTextBox.Text = string.Empty;
		}

		private void PermutsAddChange(Action<string, string, bool> successAction)
		{
			string before = beforePermutsTextBox.Text ?? string.Empty;
			string after = afterPermutsTextBox.Text ?? string.Empty;
			bool final = finalCheckBox.IsChecked ?? false;
			if (Permuts.Any((x) => x.Key == after))
			{
				DirtyWork.ShowCustomMessageDialog(this, "Ошибка!", "Замена с такого значения уже имеется!");
				return;
			}
			successAction.Invoke(before, after, final);
			beforePermutsTextBox.Text = string.Empty;
			afterPermutsTextBox.Text = string.Empty;
			finalCheckBox.IsChecked = false;
		}

		private void addAlphabetButton_Click(object sender, RoutedEventArgs e)
		{
			AlphabetAddChange(() => { alphabetList.Add(alphabetTextBox.Text); });
		}

		private void changeAlphabetButton_Click(object sender, RoutedEventArgs e)
		{
			AlphabetAddChange(() => { alphabetList[alphabetListBox.SelectedIndex] = alphabetTextBox.Text; });
		}

		private void deleteAlphabetButton_Click(object sender, RoutedEventArgs e)
		{
			alphabetList.RemoveAt(alphabetListBox.SelectedIndex);
			if (alphabetList.Count != 0)
				alphabetListBox.SelectedIndex = 0;
		}

		private void addPermutsButton_Click(object sender, RoutedEventArgs e)
		{
			PermutsAddChange((before, after, final) =>
				{
					//Since permutsList is constructed from new collection (made by linq) we must add new permutation here too
					Permuts.Add(new Triple<string, string, bool>(before, after, final));
					permutsList.Add(string.Format("{0} ->{1} {2}", before, final ? "." : string.Empty, after));
				});
		}

		private void changePermutsButton_Click(object sender, RoutedEventArgs e)
		{
			PermutsAddChange((before, after, final) =>
				{
					Permuts[permutsListBox.SelectedIndex].Key = before;
					Permuts[permutsListBox.SelectedIndex].Value = after;
					Permuts[permutsListBox.SelectedIndex].Final = final;
					permutsList[permutsListBox.SelectedIndex] = string.Format("{0} ->{1} {2}", before, final ? "." : string.Empty, after);
				});
		}

		private void deletePermutsButton_Click(object sender, RoutedEventArgs e)
		{
			Permuts.RemoveAt(permutsListBox.SelectedIndex);
			permutsList.RemoveAt(permutsListBox.SelectedIndex);
			if (permutsList.Count != 0)
				permutsListBox.SelectedIndex = 0;
		}

		private void startButton_Click(object sender, RoutedEventArgs e)
		{
			if (!DirtyWork.CheckByAlphabet(this, Alphabet, Permuts, inputTextBox.Text))
				return;
			alphabetTabItem.IsEnabled = permutsTabItem.IsEnabled = textTabItem.IsEnabled = false;
			startButton.Content = "Стоп";
			startButton.Click -= startButton_Click;
			startButton.Click += stopButton_Click;
			InputText = inputTextBox.Text;
			new Thread(DoWork).Start();
		}

		private void stopButton_Click(object sender, RoutedEventArgs e)
		{
			DirtyWork.Stop = true;
			startButton.Content = "Начать";
			startButton.Click -= stopButton_Click;
			startButton.Click += startButton_Click;
		}

		private void DoWork()
		{
			string result = DirtyWork.DoWork(this, Permuts, DirtyWork.Mode.Simple);
			this.Invoke(() =>
			{
				DirtyWork.ShowCustomMessageDialog(this, "Готово!", $"Результат: {result}");
				alphabetTabItem.IsEnabled = permutsTabItem.IsEnabled = textTabItem.IsEnabled = true;
				stopButton_Click(this, new RoutedEventArgs());
			});
		}

		private void MetroWindow_Closing(object sender, CancelEventArgs e)
		{
			Owner.Show();
		}

		private void alphabetListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			alphabetTextBox.Text = alphabetListBox.SelectedValue?.ToString();
		}

		private void permutsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int index = permutsListBox.SelectedIndex;
			if (index != -1)
			{
				beforePermutsTextBox.Text = Permuts[index].Key;
				afterPermutsTextBox.Text = Permuts[index].Value;
				finalCheckBox.IsChecked = Permuts[index].Final;
			}
		}
	}
}
