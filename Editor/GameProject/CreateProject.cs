using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Editor.Utilities;

namespace Editor.GameProject
{
	[DataContract]
	public class ProjectTemplate
	{
		[DataMember]
		public string ProjectType { get; set; }
		[DataMember]
		public string ProjectFile { get; set; }
		[DataMember]
		public List<string> Folders { get; set; }
		public byte[] Icon { get; set; }
		public byte[] Screenshot { get; set; }
		public string IconFilePath { get; set; }
		public string ScreenshotFilePath { get; set; }
		public string ProjectFilePath { get; set; }
		public string TemplatePath { get; set; }
	}

	class CreateProject : ViewModelBase
	{
		//TODO: don't hardcode paths
		private readonly string _templatePath = @"..\..\..\Editor\ProjectTemplates\";

		private string _projectName = "NewProject";
		public string ProjectName
		{
			get => _projectName;
			set
			{
				if (_projectName != value)
				{
					_projectName = value;
					ValidateProjectPath();
					OnPropertyChanged(nameof(ProjectName));
				}
			}
		}

		private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\MakeshiftProjects\";
		public string ProjectPath
		{
			get => _projectPath;
			set
			{
				if (_projectPath != value)
				{
					_projectPath = value;
					ValidateProjectPath();
					OnPropertyChanged(nameof(ProjectPath));
				}
			}
		}

		private bool _isValid;
		public bool IsValid
		{
			get => _isValid;
			set
			{
				if (_isValid != value)
				{
					_isValid = value;
					OnPropertyChanged(nameof(IsValid));
				}
			}
		}

		private string _errorMsg;
		public string ErrorMsg
		{
			get => _errorMsg;
			set
			{
				if (_errorMsg != value)
				{
					_errorMsg = value;
					OnPropertyChanged(nameof(ErrorMsg));
				}
			}
		}

		private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
		public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

		public CreateProject()
		{
			ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);

			try
			{
				string[] templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
				Debug.Assert(templateFiles.Any());

				foreach (string file in templateFiles)
				{
					ProjectTemplate template = Serializer.FromFile<ProjectTemplate>(file);
					template.TemplatePath = Path.GetDirectoryName(file);
					template.IconFilePath = Path.GetFullPath(Path.Combine(template.TemplatePath, "Icon.png"));
					template.Icon = File.ReadAllBytes(template.IconFilePath);
					template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(template.TemplatePath, "Screenshot.png"));
					template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
					template.ProjectFilePath = Path.GetFullPath(Path.Combine(template.TemplatePath, template.ProjectFile));

					_projectTemplates.Add(template);
				}

				ValidateProjectPath();
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
				Logger.Log(MessageType.Error, $"Failed to read project templates");
				throw;
			}
		}

		public string CreateNewProject(ProjectTemplate template)
		{
			ValidateProjectPath();

			if (!IsValid)
			{
				return String.Empty;
			}

			if (ProjectPath[ProjectPath.Length - 1] != Path.DirectorySeparatorChar)
			{
				ProjectPath += @"\";
			}

			string path = $@"{ProjectPath}{ProjectName}\";

			try
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				foreach (string folder in template.Folders)
				{
					Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
				}

				DirectoryInfo dirInfo = new DirectoryInfo(path + @".Makeshift\");
				dirInfo.Attributes |= FileAttributes.Hidden;

				File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
				File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

				string projectXml = File.ReadAllText(template.ProjectFilePath);
				projectXml = String.Format(projectXml, ProjectName, path);
				
				string projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
				File.WriteAllText(projectPath, projectXml);

				CreateScriptingFiles(template, path);

				return path;
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
				Logger.Log(MessageType.Error, $"Failed to create {ProjectName}");
				throw;
			}
		}

		private void CreateScriptingFiles(ProjectTemplate template, string projectPath)
		{
			string premakeTemplatePath = Path.Combine(template.TemplatePath, @"premake\");

			Debug.Assert(File.Exists(Path.Combine(premakeTemplatePath, "premake5-Template.lua")));
			Debug.Assert(File.Exists(Path.Combine(premakeTemplatePath, "premake5.exe")));
			Debug.Assert(File.Exists(Path.Combine(premakeTemplatePath, "LICENSE.txt")));
			Debug.Assert(File.Exists(Path.Combine(premakeTemplatePath, "Win-GenerateProject.bat")));

			string engineAPIPath = Path.Combine(MainWindow.MakeshiftPath, @"Engine\src\EngineAPI\");
			Debug.Assert(Directory.Exists(engineAPIPath));

			string premakeProjectPath = Path.Combine(Path.Combine(projectPath, @".Makeshift\"), "premake");
			Directory.CreateDirectory(premakeProjectPath);
			Debug.Assert(Directory.Exists(premakeProjectPath));

			string premakeLua = File.ReadAllText(Path.Combine(premakeTemplatePath, "premake5-Template.lua"));
			premakeLua = premakeLua.Replace("{0}", ProjectName);
			premakeLua = premakeLua.Replace("{1}", engineAPIPath.Replace("\\", "/"));
			File.WriteAllText(Path.GetFullPath(Path.Combine(premakeProjectPath, "premake5.lua")), premakeLua);

			File.Copy(Path.Combine(premakeTemplatePath, "premake5.exe"), Path.Combine(premakeProjectPath, "premake5.exe"));
			File.Copy(Path.Combine(premakeTemplatePath, "LICENSE.txt"), Path.Combine(premakeProjectPath, "LICENSE.txt"));
			File.Copy(Path.Combine(premakeTemplatePath, "Win-GenerateProject.bat"), Path.Combine(premakeProjectPath, "Win-GenerateProject.bat"));

			Process proc = new Process();
			proc.StartInfo.FileName = Path.Combine(premakeProjectPath, "Win-GenerateProject.bat");
			proc.Start();
		}

		private bool ValidateProjectPath()
		{
			string path = ProjectPath;

			if (String.IsNullOrEmpty(ProjectPath.Trim()))
			{
				ErrorMsg = "Select a valid project folder.";
				IsValid = false;

				return IsValid;
			}

			if (path[path.Length - 1] != Path.DirectorySeparatorChar)
			{
				path += @"\";
			}

			path += $@"{ProjectName}\";

			IsValid = false;

			if (String.IsNullOrEmpty(ProjectName.Trim()))
			{
				ErrorMsg = "Type in a project name.";
			}
			else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
			{
				ErrorMsg = "Invalid character(s) used in project name.";
			}
			else if (String.IsNullOrEmpty(ProjectPath.Trim()))
			{
				ErrorMsg = "Select a valid project folder.";
			}
			else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
			{
				ErrorMsg = "Invalid character(s) used in project path.";
			}
			else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
			{
				ErrorMsg = "Selected project folder already exists and is not empty.";
			}
			else
			{
				ErrorMsg = String.Empty;
				IsValid = true;
			}

			return IsValid;
		}
	}
}
