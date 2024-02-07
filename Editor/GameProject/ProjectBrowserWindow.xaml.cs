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
using System.Windows.Shapes;

namespace Editor.GameProject
{
	/// <summary>
	/// Interaction logic for ProjectBrowserWindow.xaml
	/// </summary>
	public partial class ProjectBrowserWindow : Window
	{
		public ProjectBrowserWindow()
		{
			InitializeComponent();
			Loaded += OnProjectBrowserWindowLoaded;
		}

		private void OnProjectBrowserWindowLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnProjectBrowserWindowLoaded;

			if (!OpenProject.Projects.Any())
			{
				OpenProjectBtn.IsEnabled = false;
				OpenProjectView.Visibility = Visibility.Hidden;
				OnToggleBtnClick(CreateProjectBtn, new RoutedEventArgs());
			}
		}

		private void OnToggleBtnClick(object sender, RoutedEventArgs e)
		{
			if (sender == OpenProjectBtn)
			{
				if (CreateProjectBtn.IsChecked == true)
				{
					CreateProjectBtn.IsChecked = false;
					BrowserContent.Margin = new Thickness(0);
				}

				OpenProjectBtn.IsChecked = true;
			}
			else
			{
				if (OpenProjectBtn.IsChecked == true)
				{
					OpenProjectBtn.IsChecked = false;
					BrowserContent.Margin = new Thickness(-800, 0, 0, 0);
				}

				CreateProjectBtn.IsChecked = true;
			}
		}
	}
}
