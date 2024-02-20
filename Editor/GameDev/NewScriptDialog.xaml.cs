using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media.Animation;
using Editor.GameProject;
using Editor.Utilities;

namespace Editor.GameDev
{
	/// <summary>
	/// Interaction logic for NewScriptDialog.xaml
	/// </summary>
	public partial class NewScriptDialog : Window
	{
		private static readonly string _cppCode =
@"#include ""{0}.h""

namespace {1}
{{
	REGISTER_SCRIPT({0});

	void {0}::Start()
	{{
		
	}}

	void {0}::Update(float deltaTime)
	{{
		
	}}
}}
";

		private static readonly string _hCode =
@"#pragma once

namespace {1}
{{
	class {0} : public Makeshift::Script::EntityScript
	{{
	public:
		constexpr explicit {0}(Makeshift::GameEntity::Entity entity)
			: Makeshift::Script::EntityScript(entity) {{}}

		void Start() override;
		void Update(float deltaTime) override;

	private:

	}};
}}
";
		public NewScriptDialog()
		{
			InitializeComponent();
			Owner = Application.Current.MainWindow;
			ScriptPath.Text = @"Scripts\";
		}

		private void OnScriptNameTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			if (!IsValid())
			{
				return;
			}

			string name = ScriptName.Text.Trim();
			MessageTextBlock.Text = $"{name}.cpp and {name}.h will be added to {Project.Current.Name}";

		}

		private void OnScriptPathTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			IsValid();
		}

		private async void OnCreateBtnClick(object sender, RoutedEventArgs e)
		{
			if (!IsValid())
			{
				return;
			}

			IsEnabled = false;
			BusyAnimation.Opacity = 0;
			BusyAnimation.Visibility = Visibility.Visible;
			DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(500)));
			BusyAnimation.BeginAnimation(OpacityProperty, fadeIn);

			try
			{
				string name = ScriptName.Text.Trim();
				string path = Path.GetFullPath(Path.Combine(Project.Current.Path, ScriptPath.Text.Trim()));
				string solution = Project.Current.Solution;
				string projectName = Project.Current.Name;
				await Task.Run(() => CreateScript(name, path, solution, projectName));
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				Logger.Log(MessageType.Error, $"Failed to create script {ScriptName.Text}");
			}
			finally
			{
				DoubleAnimation fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(200)));
				fadeOut.Completed += (s, e) =>
				{
					BusyAnimation.Opacity = 0;
					BusyAnimation.Visibility = Visibility.Hidden;
					Close();
				};

				BusyAnimation.BeginAnimation(OpacityProperty, fadeOut);
			}
		}

		private void CreateScript(string name, string path, string solution, string projectName)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			string cpp = Path.GetFullPath(Path.Combine(path, $"{name}.cpp"));
			string h = Path.GetFullPath(Path.Combine(path, $"{name}.h"));

			using (StreamWriter sw = File.CreateText(cpp))
			{
				sw.Write(String.Format(_cppCode, name, projectName.Replace(" ", "_")));
			}

			using (StreamWriter sw = File.CreateText(h))
			{
				sw.Write(String.Format(_hCode, name, projectName.Replace(" ", "_")));
			}

			string[] files = new string[]
			{
				cpp, h
			};

			for (int i = 0; i < 3; i++)
			{
				if (!VisualStudio.AddFilesToSolution(solution, projectName, files))
				{
					Thread.Sleep(1000);
				}
				else
				{
					break;
				}
			}
		}

		bool IsValid()
		{
			bool isValid = false;
			string name = ScriptName.Text.Trim();
			string path = ScriptPath.Text.Trim();
			string errorMsg = String.Empty;

			if (String.IsNullOrEmpty(name))
			{
				errorMsg = "Type in a script name.";
			}
			else if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1 || name.Any(x => Char.IsWhiteSpace(x)))
			{
				errorMsg = "Invalid character(s) used in script name.";
			}
			else if (String.IsNullOrEmpty(path))
			{
				errorMsg = "Select a valid script folder.";
			}
			else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
			{
				errorMsg = "Invalid character(s) used in script path.";
			}
			else if (!Path.GetFullPath(Path.Combine(Project.Current.Path, path)).Contains(Path.Combine(Project.Current.Path, @"Scripts\")))
			{
				errorMsg = "Script must be added to (a sub-folder of) Scripts.";
			}
			else if (File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.cpp"))) ||
			         File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.h"))))
			{
				errorMsg = $"Script {name} already exists in this folder.";
			}
			else
			{
				isValid = true;
			}

			if (!isValid)
			{
				MessageTextBlock.Foreground = FindResource("Editor.RedBrush") as Brush;
			}
			else
			{
				MessageTextBlock.Foreground = FindResource("Editor.FontBrush") as Brush;
			}

			MessageTextBlock.Text = errorMsg;

			return isValid;
		}
	}
}
