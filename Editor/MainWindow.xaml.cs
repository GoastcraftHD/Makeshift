using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using Editor.GameProject;
using Path = System.IO.Path;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		//TODO: set when installing engine
		public static string MakeshiftPath { get; private set; } = @"C:\dev\VisualStudio\Makeshift";

		public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosing;
        }

        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
			Closing -= OnMainWindowClosing;
			Project.Current?.Unload();
        }

		private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
	        Loaded -= OnMainWindowLoaded;
	        GetEnginePath();
	        OpenProjectBrowser();
        }

		private void GetEnginePath()
		{
			string enginePath = Environment.GetEnvironmentVariable("MAKESHIFT_ENGINE_PATH", EnvironmentVariableTarget.User);

			if (enginePath == null || !Directory.Exists(Path.Combine(enginePath, @"Engine\src\EngineAPI")))
			{
				EnginePathDialog dlg = new EnginePathDialog();
				if (dlg.ShowDialog() == true)
				{
					MakeshiftPath = dlg.MakeshiftPath;
					Environment.SetEnvironmentVariable("MAKESHIFT_ENGINE_PATH", MakeshiftPath, EnvironmentVariableTarget.User);
				}
				else
				{
					Application.Current.Shutdown();
				}
			}
			else
			{
				MakeshiftPath = enginePath;
			}
		}

		private void OpenProjectBrowser()
        {
			ProjectBrowserWindow projectBrowser = new ProjectBrowserWindow();

			if (projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null)
			{
				Application.Current.Shutdown();
			}
			else
			{
				Project.Current?.Unload();
				DataContext = projectBrowser.DataContext;
			}
		}

	}
}
