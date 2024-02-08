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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Editor.Utilities
{
	/// <summary>
	/// Interaction logic for LoggerView.xaml
	/// </summary>
	public partial class LoggerView : UserControl
	{
		public LoggerView()
		{
			InitializeComponent();
		}

		private void OnClearBtnClick(object sender, RoutedEventArgs e)
		{
			Logger.Clear();
		}

		private void OnMessageFilterBtnClick(object sender, RoutedEventArgs e)
		{
			int filter = 0x0;

			if (ToggleInfo.IsChecked == true)
			{
				filter |= (int)MessageType.Info;
			}

			if (ToggleWarn.IsChecked == true)
			{
				filter |= (int)MessageType.Warning;
			}

			if (ToggleError.IsChecked == true)
			{
				filter |= (int)MessageType.Error;
			}

			Logger.SetMessageFilter(filter);
		}
	}
}
