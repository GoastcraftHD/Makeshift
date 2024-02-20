using System;
using System.Collections.Generic;
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
using System.IO;

namespace Editor
{
	/// <summary>
	/// Interaction logic for EnginePathDialog.xaml
	/// </summary>
	public partial class EnginePathDialog : Window
	{
		public string MakeshiftPath { get; private set; }

		public EnginePathDialog()
		{
			InitializeComponent();
			Owner = Application.Current.MainWindow;
		}

		private void OnOkBtnClick(object sender, RoutedEventArgs e)
		{
			string path = PathTextBox.Text;
			MessageTextBlock.Text = String.Empty;

			if (String.IsNullOrEmpty(path))
			{
				MessageTextBlock.Text = "Invalid path.";
			}
			else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
			{
				MessageTextBlock.Text = "Invalid character(s) used in path.";
			}
			else if (!Directory.Exists(Path.Combine(path, @"Engine\src\EngineAPI\")))
			{
				MessageTextBlock.Text = "Unable to find the engine at the specified location,";
			}

			if (String.IsNullOrEmpty(MessageTextBlock.Text))
			{
				if (path.Last() != Path.DirectorySeparatorChar)
				{
					path += @"\";
				}

				MakeshiftPath = path;
				DialogResult = true;
				Close();
			}
		}
    }
}
