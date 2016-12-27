using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
	/// Логика взаимодействия для ExpertWindow.xaml
	/// </summary>
	public partial class ExpertWindow : MetroWindow, INotifyPropertyChanged
	{
		public bool IsAlphabetSelected => alphabetListBox.SelectedIndex != -1;
		public bool IsPermutsSelected => permutsListBox.SelectedIndex != -1;
		public bool CanStart => inputTextBox.Text != string.Empty;

		private static BindingList<DataGridRow> stepsList;
		public static BindingList<DataGridRow> StepsList => stepsList;
		private static BindingList<string> permutsList;
		private static BindingList<string> alphabetList;

		private CustomDialog dialog;
		private AlphabetDialog lastAlphabetDialog;
		private PermutsDialog lastPermutsDialog;
		private MetroDialogSettings settings = new MetroDialogSettings()
		{
			AnimateShow = true,
			AnimateHide = true,
		};

		public event PropertyChangedEventHandler PropertyChanged;

		public ExpertWindow()
		{
			Owner = MainWindow.LastInstance;
			InitializeComponent();
			DataContext = this;
			stepsList = new BindingList<DataGridRow>(DataClass.Steps);
			permutsList = new BindingList<string>(PermutsListBox);
			alphabetList = new BindingList<string>(Alphabet);
			stepsDataGrid.ItemsSource = stepsList;
			permutsListBox.ItemsSource = permutsList;
			alphabetListBox.ItemsSource = alphabetList;
			alphabetListBox.SelectionChanged += (sender, e) =>
			{
				PropertyChanged(this, new PropertyChangedEventArgs("IsAlphabetSelected"));
			};
			permutsListBox.SelectionChanged += (sender, e) =>
			{
				PropertyChanged(this, new PropertyChangedEventArgs("IsPermutsSelected"));
			};
		}

		private void MetroWindow_Closed(object sender, EventArgs e)
		{
			stepsList.Clear();
			Owner.Show();
		}

		private void backButton_Click(object sender, RoutedEventArgs e)
		{
			base.OnClosed(e);
			Close();
		}

		private async void addAlphabetButton_Click(object sender, RoutedEventArgs e)
		{
			lastAlphabetDialog = new AlphabetDialog();
			lastAlphabetDialog.okButton.Click += addOkButton_Click;
			lastAlphabetDialog.cancelButton.Click += cancelButton_Click;
			dialog = new CustomDialog { Height = 200, Content = lastAlphabetDialog };
			await this.ShowMetroDialogAsync(dialog, settings);
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.HideMetroDialogAsync(dialog);
		}

		private void addOkButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender == lastAlphabetDialog?.okButton)
			{
				string result = lastAlphabetDialog.Result;
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
				alphabetList.Add(result);
			}
			else if (sender == lastPermutsDialog?.okButton)
			{
				if (lastPermutsDialog.Before == null)
					lastPermutsDialog.Before = string.Empty;
				if (lastPermutsDialog.After == null)
					lastPermutsDialog.After = string.Empty;
				if (Permuts.Any((x) => x.Key == lastPermutsDialog.Before))
				{
					DirtyWork.ShowCustomMessageDialog(this, "Ошибка!", "Замена с такого значения уже имеется!");
					return;
				}
				//Since permutsList is constructed from new collection (made by linq) we must add new permutation here too
				Permuts.Add(new Triple<string, string, bool>(lastPermutsDialog.Before, lastPermutsDialog.After, lastPermutsDialog.Final));
				permutsList.Add(string.Format("{0} ->{1} {2}", lastPermutsDialog.Before, lastPermutsDialog.Final ? "." : string.Empty, lastPermutsDialog.After));
			}
			this.HideMetroDialogAsync(dialog);
		}

		private async void addPermutsButton_Click(object sender, RoutedEventArgs e)
		{
			lastPermutsDialog = new PermutsDialog();
			lastPermutsDialog.okButton.Click += addOkButton_Click;
			lastPermutsDialog.cancelButton.Click += cancelButton_Click;
			dialog = new CustomDialog { Height = 200, Content = lastPermutsDialog };
			MetroDialogSettings settings = new MetroDialogSettings()
			{
				AnimateShow = true,
				AnimateHide = true,
			};
			await this.ShowMetroDialogAsync(dialog, settings);
		}

		private void startButton_Click(object sender, RoutedEventArgs e)
		{
			if (!DirtyWork.CheckByAlphabet(this, Alphabet, Permuts, inputTextBox.Text))
				return;
			addAlphabetButton.IsEnabled = addPermutsButton.IsEnabled = alphabetListBox.IsEnabled =
				permutsListBox.IsEnabled = backButton.IsEnabled = inputTextBox.IsEnabled = false;
			startButton.Content = "Стоп";
			startButton.Click -= startButton_Click;
			startButton.Click += stopButton_Click;
			stepsList.Clear();
			stepsDataGrid.ItemsSource = null;
			stepsDataGrid.ItemsSource = stepsList;
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
			string result = DirtyWork.DoWork(this, Permuts, DirtyWork.Mode.Expert);
			stepsList = new BindingList<DataGridRow>(DataClass.Steps);
			this.Invoke(() =>
			{
				DirtyWork.ShowCustomMessageDialog(this, "Готово!", $"Результат: {result}");
				addAlphabetButton.IsEnabled = addPermutsButton.IsEnabled = alphabetListBox.IsEnabled =
					permutsListBox.IsEnabled = backButton.IsEnabled = inputTextBox.IsEnabled = true;
				deleteAlphabetButton.SetBinding(Button.IsEnabledProperty, new Binding("AlphabetSelected") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
				deletePermutsButton.SetBinding (Button.IsEnabledProperty, new Binding("PermutsSelected") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
				stopButton_Click(this, new RoutedEventArgs());
			});
		}

		private async void alphabetListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (alphabetListBox.SelectedItem != null)
			{
				lastAlphabetDialog = new AlphabetDialog();
				lastAlphabetDialog.Result = alphabetListBox.SelectedItem.ToString();
				lastAlphabetDialog.okButton.Click += (s, newevent) =>
				{
					string result = lastAlphabetDialog.Result;
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
					alphabetList[alphabetListBox.SelectedIndex] = result;
					this.HideMetroDialogAsync(dialog);
				};
				lastAlphabetDialog.cancelButton.Click += cancelButton_Click;
				dialog = new CustomDialog { Height = 200, Content = lastAlphabetDialog };
				await this.ShowMetroDialogAsync(dialog, settings);
			}
		}

		private async void permutsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (permutsListBox.SelectedItem != null)
			{
				lastPermutsDialog = new PermutsDialog();
				lastPermutsDialog.Before = Permuts[permutsListBox.SelectedIndex].Key;
				lastPermutsDialog.After = Permuts[permutsListBox.SelectedIndex].Value;
				lastPermutsDialog.Final = Permuts[permutsListBox.SelectedIndex].Final;
				lastPermutsDialog.okButton.Click += (s, newevent) =>
				{
					if (lastPermutsDialog.Before == null)
						lastPermutsDialog.Before = string.Empty;
					if (lastPermutsDialog.After == null)
						lastPermutsDialog.After = string.Empty;
					if (Permuts.Any((x) => (x.Key == lastPermutsDialog.Before) && (Permuts[permutsListBox.SelectedIndex] != x)))
					{
						DirtyWork.ShowCustomMessageDialog(this, "Ошибка!", "Замена с такого значения уже имеется!");
						return;
					}
					Permuts[permutsListBox.SelectedIndex].Key = lastPermutsDialog.Before;
					Permuts[permutsListBox.SelectedIndex].Value = lastPermutsDialog.After;
					Permuts[permutsListBox.SelectedIndex].Final = lastPermutsDialog.Final;
					permutsList[permutsListBox.SelectedIndex] = string.Format("{0} ->{1} {2}", lastPermutsDialog.Before, lastPermutsDialog.Final ? "." : string.Empty, lastPermutsDialog.After);
					this.HideMetroDialogAsync(dialog);
				};
				lastPermutsDialog.cancelButton.Click += cancelButton_Click;
				dialog = new CustomDialog { Height = 200, Content = lastPermutsDialog };
				await this.ShowMetroDialogAsync(dialog, settings);
			}
		}

		private void deleteAlphabetButton_Click(object sender, RoutedEventArgs e)
		{
			alphabetList.RemoveAt(alphabetListBox.SelectedIndex);
			if (alphabetList.Count != 0)
				alphabetListBox.SelectedIndex = 0;
		}

		private void deletePermutsButton_Click(object sender, RoutedEventArgs e)
		{
			Permuts.RemoveAt(permutsListBox.SelectedIndex);
			permutsList.RemoveAt(permutsListBox.SelectedIndex);
			if (permutsList.Count != 0)
				permutsListBox.SelectedIndex = 0;
		}

		private void MetroWindow_Closing(object sender, CancelEventArgs e)
		{
			base.OnClosed(e);
		}

		private void inputTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			PropertyChanged(this, new PropertyChangedEventArgs("CanStart"));
		}
	}

	public class DataGridRow
	{
		public int Step { get; set; }
		public string Before { get; set; }
		public string After { get; set; }
	}

	class Triple<T1, T2, T3>
	{
		public T1 Key { get; set; }
		public T2 Value { get; set; }
		public T3 Final { get; set; }

		public Triple(T1 key, T2 value, T3 final)
		{
			Key = key;
			Value = value;
			Final = final;
		}
	}
}
