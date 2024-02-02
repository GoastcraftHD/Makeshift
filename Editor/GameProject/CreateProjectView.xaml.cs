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

namespace Editor.GameProject
{
	/// <summary>
	/// Interaction logic for CreateProjectView.xaml
	/// </summary>
	public partial class CreateProjectView : UserControl
	{
		public CreateProjectView()
		{
			InitializeComponent();
		}

		private void OnCreateBtnClick(object sender, RoutedEventArgs e)
		{
			CreateProject vm = DataContext as CreateProject;
			string projectPath = vm.CreateNewProject(TemplateListBox.SelectedItem as ProjectTemplate);

			bool dialogResult = false;
			Window win = Window.GetWindow(this);

			if (!String.IsNullOrEmpty(projectPath))
			{
				dialogResult = true;
			}

			win.DialogResult = dialogResult;
			win.Close();
        }
    }
}
