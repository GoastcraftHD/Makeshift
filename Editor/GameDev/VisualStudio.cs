﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Editor.Utilities;

namespace Editor.GameDev
{
	class VisualStudio
	{
		private static EnvDTE80.DTE2 _vsInstance = null;
		private static readonly string _progId = "VisualStudio.DTE.17.0";

		public static void OpenVisualStudio(string solutionPath)
		{
			IRunningObjectTable rot = null;
			IEnumMoniker monikerTable = null;
			IBindCtx bindCtx = null;

			try
			{
				if (_vsInstance == null)
				{
					int hResult = GetRunningObjectTable(0, out rot);

					if (hResult < 0 || rot == null)
					{
						throw new COMException($"GetRunningObjectTable() returned HRESULT: {hResult:X8}");
					}

					rot.EnumRunning(out monikerTable);
					monikerTable.Reset();

					hResult = CreateBindCtx(0, out bindCtx);

					if (hResult < 0 || bindCtx == null)
					{
						throw new COMException($"CreateBindCtx() returned HRESULT: {hResult:X8}");
					}

					IMoniker[] currentMoniker = new IMoniker[1];

					while (monikerTable.Next(1, currentMoniker, IntPtr.Zero) == 0)
					{
						string name = String.Empty;
						currentMoniker[0].GetDisplayName(bindCtx, null, out name);

						if (name.Contains(_progId))
						{
							hResult = rot.GetObject(currentMoniker[0], out object obj);

							if (hResult < 0 || obj == null)
							{
								throw new COMException(
									$"Running object table's GetObject() returned HRESULT: {hResult:X8}");
							}

							EnvDTE80.DTE2 dte = obj as EnvDTE80.DTE2;
							string solutionName = dte.Solution.FullName;

							if (solutionName == solutionPath)
							{
								_vsInstance = dte;
								break;
							}
						}
					}

					if (_vsInstance == null)
					{
						Type visualStudioType = Type.GetTypeFromProgID(_progId, true);
						_vsInstance = Activator.CreateInstance(visualStudioType) as EnvDTE80.DTE2;
					}
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
				Logger.Log(MessageType.Error, "Failed to opne Visual Studio");
			}
			finally
			{
				if (monikerTable != null)
				{
					Marshal.ReleaseComObject(monikerTable);
				}

				if (rot != null)
				{
					Marshal.ReleaseComObject(rot);
				}

				if (bindCtx != null)
				{
					Marshal.ReleaseComObject(bindCtx);
				}
			}
		}

		public static void CloseVisualStudio()
		{
			if (_vsInstance?.Solution.IsOpen == true)
			{
				_vsInstance.ExecuteCommand("File.SaveAll"); // It's better to save twice, than to loose all the work due to an error
				_vsInstance.Solution.Close(true);
			}

			_vsInstance?.Quit();
		}

		public static bool AddFilesToSolution(string solution, string projectName, string[] files)
		{
			Debug.Assert(files?.Length > 0);

			OpenVisualStudio(solution);

			try
			{
				if (!_vsInstance.Solution.IsOpen)
				{
					_vsInstance.Solution.Open(solution);
				}
				else
				{
					_vsInstance.ExecuteCommand("File.SaveAll");
				}

				foreach (EnvDTE.Project project in _vsInstance.Solution.Projects)
				{
					if (project.UniqueName.Contains(projectName))
					{
						foreach (string file in files)
						{
							project.ProjectItems.AddFromFile(file);
						}
					}
				}

				string cpp = files.FirstOrDefault(x => Path.GetExtension(x) == ".cpp");

				if (!String.IsNullOrEmpty(cpp))
				{
					_vsInstance.ItemOperations.OpenFile(cpp, EnvDTE.Constants.vsViewKindTextView).Visible = true;
				}

				_vsInstance.MainWindow.Activate();
				_vsInstance.MainWindow.Visible = true;
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
				Logger.Log(MessageType.Error, "Failed to add files to Visual Studio project");
				return false;
			}

			return true;
		}

		[DllImport("ole32.dll")]
		private static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);
		[DllImport("ole32.dll")]
		private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);
	}
}
