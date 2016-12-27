using System;
using System.Collections.Generic;
using System.Linq;

using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace MathLogicWpf
{
	static class DirtyWork
	{
		private delegate void InvokeWork();
		public static bool Stop { get; set; }

		public enum Mode
		{
			Simple = 0x1,
			Expert = 0x2
		}

		public static bool CheckByAlphabet(MetroWindow parent, List<string> alphabet, List<Triple<string, string, bool>> permuts, string text)
		{
			var _alphabet = new List<string>();
			foreach (var a in alphabet)
			{
				if (a.Contains(".."))
					_alphabet.Add(CharRange(a.First(), a.Last()));
				else _alphabet.Add(a);
			}
			bool found;
			foreach (Triple<string, string, bool> x in permuts)
			{
				foreach (char c in x.Key)
				{
					found = false;
					foreach (string a in _alphabet)
					{
						if (a.Contains(c))
						{
							found = true;
							break;
						}
					}
					if (!found)
					{
						ShowCustomMessageDialog(parent, "Ошибка!", string.Format("В списках замен используется символ (\"{0}\"), отсутствующий в алфавите!", c));
						return false;
					}
				}
				foreach (char c in x.Value)
				{
					found = false;
					foreach (string a in _alphabet)
					{
						if (a.Contains(c))
						{
							found = true;
							break;
						}
					}
					if (!found)
					{
						ShowCustomMessageDialog(parent, "Ошибка!", string.Format("В списках замен используется символ (\"{0}\"), отсутствующий в алфавите!", c));
						return false;
					}
				}
			}
			foreach (char c in text)
			{
				found = false;
				foreach (string a in _alphabet)
				{
					if (a.Contains(c))
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					ShowCustomMessageDialog(parent, "Ошибка!", string.Format("В исходном тексте используется символ (\"{0}\"), отсутствующий в алфавите!", c));
					return false;
				}
			}
			return true;
		}

		public static string CharRange(char startSymbol, char endSymbol)
		{
			return new string(Enumerable.Range(startSymbol, endSymbol - startSymbol + 1).Select(c => (char)c).ToArray());
		}

        public static int StringFind(string source, string subject)
        {
            if (subject == string.Empty)
                return 0;
            if (source.Contains(subject))
            {
                for (int i = 0; i <= source.Length - subject.Length; i++)
                    if (source.Substring(i, subject.Length) == subject)
                        return i;
            }
            return -1;
        }

		public static int StringFindAny(string source, string[] subjects, out int index)
		{
			if (subjects.Length == 0)
			{
				index = -1;
				return -1;
			}
			int minpos = source.Length;
			index = -1;
			for (int i = 0; i < subjects.Length; i++)
			{
				int pos = StringFind(source, subjects[i]);
				if (pos != -1)
					if ((pos < minpos) || (minpos == source.Length))
					{
						minpos = pos;
						index = i;
					}
			}
			return (minpos == source.Length ? -1 : minpos);
		}

        public static string DoWork(MetroWindow owner, List<Triple<string, string, bool>> permuts, Mode mode)
        {
            int step = 1;
            Stop = false;
            string text = DataClass.InputText;
            int i = 0;
            int pos = -1;
            bool found = false;
            while (!Stop)
            {
                var x = permuts[i];
                do
                {
                    if (Stop)
                        break;
                    pos = StringFind(text, x.Key);
					if (pos != -1)
					{
						string before = text;
						if (x.Value == string.Empty)
							text = text.Remove(pos, x.Key.Length);
						else if (x.Key == string.Empty)
							text = text.Insert(pos, x.Value);
						else
							text = text.Remove(pos, x.Key.Length).Insert(pos, x.Value);
						if (mode == Mode.Expert)
							owner.Invoke(() =>
							{
								ExpertWindow.StepsList.Add(new DataGridRow
								{
									Step = step,
									Before = before,
									After = text
								});
							});
						step++;
						if (permuts[i].Final)
							Stop = true;
						found = true;
						if (x.Key == string.Empty)
							break;
					}
                }
                while (pos != -1);
                if (!found)
                    i = (i + 1) % permuts.Count;
                else i = 0;
                if ((i == 0) && (!found))
                    Stop = true;
                else found = false;
            }
            return text;
        }

		public static async void ShowCustomMessageDialog(MetroWindow parent, string title, string message)
		{
			CustomDialog dialog = new CustomDialog();
			var add = new CustomMessageDialog();
			add.Title = title;
			add.Message = message;
			add.okButton.Click += (sender, e) =>
			{
				parent.HideMetroDialogAsync(dialog);
			};
			dialog.Height = 200;
			dialog.Width = parent.Width + 100;
			dialog.Content = add;
			MetroDialogSettings settings = new MetroDialogSettings()
			{
				AnimateShow = true,
				AnimateHide = true,
			};
			await parent.ShowMetroDialogAsync(dialog, settings);
		}
    }
}
