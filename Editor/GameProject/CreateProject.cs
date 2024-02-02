﻿using System;
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
					template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png"));
					template.Icon = File.ReadAllBytes(template.IconFilePath);
					template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
					template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
					template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

					_projectTemplates.Add(template);
				}

				ValidateProjectPath();
			}
			catch (Exception e)
			{
				//TODO: Add proper logging
				Debug.WriteLine(e.Message);
			}
		}

		public string CreateNewProject(ProjectTemplate template)
		{
			ValidateProjectPath();

			if (!IsValid)
			{
				return String.Empty;
			}

			string path = ProjectPath;

			if (path[path.Length - 1] != Path.DirectorySeparatorChar)
			{
				path += @"\";
			}

			path += $@"{ProjectName}\";

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

				DirectoryInfo dirInfo = new DirectoryInfo(path + @".Makeshift");
				dirInfo.Attributes |= FileAttributes.Hidden;

				File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
				File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

				string projectXml = File.ReadAllText(template.ProjectFilePath);
				projectXml = String.Format(projectXml, ProjectName, ProjectPath);
				
				string projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
				File.WriteAllText(projectPath, projectXml);

				return path;
			}
			catch (Exception e)
			{
				//TODO: Add proper logging
				Debug.WriteLine(e.Message);
				return String.Empty;
			}
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
