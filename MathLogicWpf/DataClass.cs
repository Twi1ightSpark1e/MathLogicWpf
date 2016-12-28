using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MathLogicWpf
{
	static class DataClass
	{
		public static string InputText { get; set; }
		private static List<DataGridRow> _steps = new List<DataGridRow>();
		public static List<DataGridRow> Steps => _steps;

		private static List<string> _alphabet = new List<string>();
		public static List<string> Alphabet => _alphabet;

		private static List<Triple<string, string, bool>> _permuts = new List<Triple<string, string, bool>>();
		public static List<Triple<string, string, bool>> Permuts => _permuts;
		public static List<string> PermutsListBox => (from x in _permuts
													  select string.Format("{0} ->{1} {2}", x.Key, (x.Final ? "." : string.Empty), x.Value)).ToList();

		public static string Text;
	}
}
