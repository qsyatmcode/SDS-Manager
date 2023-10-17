using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SDSManager
{
	internal class Render
	{
		private readonly ConsoleColor _windowsBordersColor;
		private readonly ConsoleColor _windowsBackgroundColor;
		private readonly ConsoleColor _foldersColor;
		private readonly ConsoleColor _filesColor;
		private readonly ConsoleColor _selectedItemColor;
		private readonly ConsoleColor _backgroundAuxiliaryElementsColor;
		private readonly ConsoleColor _foregroundAuxiliaryElementsColor;

		private readonly Window _leftWindow;
		private readonly Window _rightWindow;

		public void ProcessAction(ActionType actionType)
		{
			Window window = GetWindowByContentType(WindowContentType.Directory);
			if (actionType == ActionType.None)
			{
				return;
			}
			else if (actionType == ActionType.Open)
			{
				//Window window = GetWindowByContentType(WindowContentType.Directory);

				if (window.SelectedFolder != null)
				{
					if (window.CurrentDirectory != null) window.PrevDirectories.Add(window.CurrentDirectory);
					window.CurrentDirectory = window.SelectedFolder;
					window.ContentObjects = GetDirectoryContent(window.CurrentDirectory);
				}
				else if (window.SelectedFile != null)
				{
					Window otherWindow = GetOtherWindow(window);
					//if (window.SelectedFile.Extension == ".txt")
					//{
						otherWindow.ContentType = WindowContentType.TextView;
						//otherWindow.Content = GetFileContent(window.SelectedFile);
					//}
				}
			}
			else if (actionType == ActionType.Cancel)
			{
				//Window window = GetWindowByContentType(WindowContentType.Directory);

				if (window.PrevDirectories.Count > 0)
				{
					window.CurrentDirectory = window.PrevDirectories.Last();
					window.PrevDirectories.RemoveAt(window.PrevDirectories.Count - 1);
					window.ContentObjects = GetDirectoryContent(window.CurrentDirectory);
				}
			}
			else if (actionType == ActionType.Down)
			{
				window.SelectedObjectIndex++;
				ChangeSelected();
			}
			else if (actionType == ActionType.Up)
			{
				window.SelectedObjectIndex--;
				ChangeSelected();
			}

			void ChangeSelected()
			{
				if (window.ContentObjects[window.SelectedObjectIndex] is DirectoryInfo)
				{
					window.SelectedFile = null;
					window.SelectedFolder = window.ContentObjects[window.SelectedObjectIndex] as DirectoryInfo;
				}
				else if (window.ContentObjects[window.SelectedObjectIndex] is FileInfo)
				{
					window.SelectedFolder = null;
					window.SelectedFile = window.ContentObjects[window.SelectedObjectIndex] as FileInfo;
				}
			}
		}

		public void Draw()
		{
			Console.Clear();
			
			DrawWindows();

			DrawAuxiliaryElements();

			DrawContent();
		}

		private void DrawContent()
		{
			Window[] windows = new Window[] { _leftWindow, _rightWindow };

			foreach (var window in windows)
			{
				
				DrawWindowContent(window);
				
			}
		}

		private void DrawWindowContent(Window window)
		{
			int creationDateWidth = 12;
			int procentWidth = 5;
			int titleWidth = window.Width - creationDateWidth - procentWidth;

			int leftPadding = window.LeftPadding;
			int topPos = window.TopPadding;

			Console.BackgroundColor = _windowsBackgroundColor;
			for (int i = 0; i < window.ContentObjects.Length; i++)
			{
				if (topPos >= window.Height + window.TopPadding)
					break; // out of window borders

				if (i == window.SelectedObjectIndex)
				{
					Console.ForegroundColor = _selectedItemColor;
					DrawObject(window.ContentObjects[i]);
				}else if (window.ContentObjects[i] is DirectoryInfo)
				{
					Console.ForegroundColor = _foldersColor;
					DrawObject(window.ContentObjects[i]);
				}
				else if (window.ContentObjects[i] is FileInfo)
				{
					Console.ForegroundColor = _filesColor;
					DrawObject(window.ContentObjects[i]);
				}
				
				topPos++;
			}



			void DrawObject(object obj)
			{
				int procent = 99; //(int)GetProcentOfParentDirSize(); // there may be data loss and incorrect output
				DateTime creationTime = GetCreationTime();
				string title = GetTitle();
				string creationTimeString = GetDateString();

				Console.SetCursorPosition(leftPadding, topPos);
				Console.Write(title);
				WriteBorder();
				Console.Write(creationTimeString);
				WriteBorder();
				Console.Write($"{procent}%");

				void WriteBorder()
				{
					ConsoleColor prevColor = Console.ForegroundColor;
					Console.ForegroundColor = _windowsBordersColor;
					Console.Write("\u2502");
					Console.ForegroundColor = prevColor;
				}

				string GetDateString()
				{
					return creationTime.ToString("MM/dd/yyyy");
				}
				string GetTitle()
				{
					if (obj is DirectoryInfo)
					{
						DirectoryInfo drawing = (DirectoryInfo)obj;
						if (drawing.Name.Length > titleWidth)
							return drawing.Name.Substring(0, titleWidth);
						else
							return drawing.Name + new string(' ', titleWidth - drawing.Name.Length);
					}
					else if (obj is FileInfo)
					{
						FileInfo drawing = (FileInfo)obj;
						if (drawing.Name.Length > titleWidth)
							return drawing.Name.Substring(0, titleWidth);
						else
							return drawing.Name;
					}
					else
					{
						throw new ArgumentException();
					}
				}
				DateTime GetCreationTime()
				{
					if (obj is DirectoryInfo)
					{
						return (obj as DirectoryInfo).CreationTime;

					}
					else if (obj is FileInfo)
					{
						return (obj as FileInfo).CreationTime;
					}
					else
					{
						throw new ArgumentException();
					}
				}
				long GetProcentOfParentDirSize()
				{
					if (obj is DirectoryInfo)
					{
						DirectoryInfo drawingDirectory = obj as DirectoryInfo;
						DirectoryInfo parent = drawingDirectory.Parent;
						if (parent == null)
							return 0;
						long parentSize = DirSize(parent);
						long drawingDirectorySize = DirSize(drawingDirectory);
						return (drawingDirectorySize / parentSize) * 100;

					}
					else if (obj is FileInfo)
					{
						FileInfo drawingFile = obj as FileInfo;
						DirectoryInfo parent = new DirectoryInfo(GetParentFolderName(drawingFile.FullName));
						if (parent == null)
							return 0;
						long parentSize = DirSize(parent);
						long drawingFileSize = drawingFile.Length;
						return (drawingFileSize / parentSize) * 100;
					}
					else
					{
						throw new ArgumentException();
					}
				}
				string GetParentFolderName(string fullFileName)
				{
					int pos = fullFileName.LastIndexOf(@"\");
					return fullFileName.Substring(0, pos); // without last slash
				}

				long DirSize(DirectoryInfo directory)
				{
					long size = 0;

					FileInfo[] fis = directory.GetFiles();
					foreach (FileInfo fi in fis)
					{ 
						size += fi.Length;
					}

					DirectoryInfo[] dis = directory.GetDirectories();
					foreach (DirectoryInfo di in dis)
					{
						try
						{
							size += DirSize(di);
						}catch { }
					}

					return size;
				}
			}
		}

		private Window GetOtherWindow(in Window window)
		{
			Window otherWindow;
			if (window == _leftWindow)
				return _rightWindow;
			else
				return _leftWindow;
		}

		private object[] GetDirectoryContent(DirectoryInfo directory)
		{
			DirectoryInfo[] dirs = directory.GetDirectories();
			FileInfo[] files = directory.GetFiles();
			
			object[] result = new object[dirs.Length + files.Length];

			for (int i = 0; i < dirs.Length; i++)
			{
				result[i] = dirs[i];
			}

			for (int i = 0; i < files.Length; i++)
			{
				result[i + dirs.Length] = files[i];
			}

			return result;
		}

		//private string[] GetFileContent(FileInfo file)
		//{
			
		//}

		private Window GetWindowByContentType(WindowContentType type)
		{
			Window[] windows = new Window[] { _leftWindow, _rightWindow };

			foreach (var window in windows)
			{
				if (window.ContentType == type)
				{
					return window;
				}
			}

			throw new NullReferenceException();
		}

		private void DrawAuxiliaryElements()
		{
			Console.BackgroundColor = _backgroundAuxiliaryElementsColor;
			Console.ForegroundColor = _foregroundAuxiliaryElementsColor;

			DrawTime();
			DrawBottomHelp();

			Console.ResetColor();
		}

		private void DrawWindows()
		{
			DrawWindowBorder(_leftWindow);
			DrawWindowBorder(_rightWindow);
		}

		private void DrawBottomHelp()
		{
			Console.SetCursorPosition(0, Console.WindowHeight - 2);

			string content = " " + Environment.CurrentDirectory + " ";
			int padding = (Console.WindowWidth - content.Length) / 2;
			string space = new string('\u2592', padding);

			Console.BackgroundColor = _foregroundAuxiliaryElementsColor;
			Console.ForegroundColor = _backgroundAuxiliaryElementsColor;
			Console.Write(space);
			Console.BackgroundColor = _backgroundAuxiliaryElementsColor;
			Console.ForegroundColor = _foregroundAuxiliaryElementsColor;

			Console.Write(content);

			Console.BackgroundColor = _foregroundAuxiliaryElementsColor;
			Console.ForegroundColor = _backgroundAuxiliaryElementsColor;
			Console.Write(space);
			Console.BackgroundColor = _backgroundAuxiliaryElementsColor;
			Console.ForegroundColor = _foregroundAuxiliaryElementsColor;

			Console.SetCursorPosition(0, Console.WindowHeight - 1);

			content = " LEFT ARROW/BACKSPACE - cancel or exit    |    RIGHT ARROW/ENTER - open or read    |    DOWN ARROW - move down    |    UP ARROW - move up ";

			padding = (Console.WindowWidth - content.Length) / 2;
			space = new string('\u2591', padding);

			Console.BackgroundColor = _foregroundAuxiliaryElementsColor;
			Console.ForegroundColor = _backgroundAuxiliaryElementsColor;
			Console.Write(space);
			Console.BackgroundColor = _backgroundAuxiliaryElementsColor;
			Console.ForegroundColor = _foregroundAuxiliaryElementsColor;

			Console.Write(content);

			Console.BackgroundColor = _foregroundAuxiliaryElementsColor;
			Console.ForegroundColor = _backgroundAuxiliaryElementsColor;
			Console.Write(space);
			Console.BackgroundColor = _backgroundAuxiliaryElementsColor;
			Console.ForegroundColor = _foregroundAuxiliaryElementsColor;
		}

		private void DrawTime()
		{
			Console.SetCursorPosition(0, 0);

			string OSversion = Environment.OSVersion.VersionString;
			string userName = Environment.UserName;
			string currentTime = DateTime.Now.ToString("hh:mm tt");

			int spacesCount = Console.WindowWidth - (OSversion.Length + userName.Length + 4) - currentTime.Length;

			Console.Write($"{userName} | {OSversion}" + new string(' ', spacesCount) + currentTime + " ");
		}

		private string Title(in Window window)
		{
			string result = "";

			if (window.ContentType == WindowContentType.Directory)
			{
				result = window.CurrentDirectory.FullName;
			}else if (window.ContentType == WindowContentType.FileInfo || window.ContentType == WindowContentType.TextView)
			{
				//TODO:
				//result += window.SelectedFile.Name + " ";
				result += window.ContentType.ToString();
			}
			else
			{
				result += window.ContentType.ToString();
			}

			result = result.PadLeft(result.Length + 1);
			result = result.PadRight(result.Length + 1);

			return result;
		}

		private void DrawWindowBorder(in Window window) // ║ ╗ ╝ ╔ ═ ╚ 
		{
			string windowTitle = Title(window);

			// The loop runs line by line from top to bottom
			Console.ForegroundColor = _windowsBordersColor;
			Console.BackgroundColor = _windowsBackgroundColor;
			for (int i = window.TopPadding - 1; i < window.Height + window.TopPadding + 1; i++)
			{
				int borderLeftPadding = window.LeftPadding - 1;
				Console.SetCursorPosition(borderLeftPadding, i);

				// The first line (title)
				if (i == window.TopPadding - 1)
				{
					int titleStart = ((window.Width - 2) - windowTitle.Length) / 2;
					string titleBorder = new string('\u2550', titleStart);
					Console.WriteLine(@"╔" + titleBorder + windowTitle + titleBorder + @"╗");
				}else if (i == window.Height + window.TopPadding) // The last line
				{
					string bottomBorder = new string('\u2550', window.Width - 2);
					Console.WriteLine(@"╚" + bottomBorder + @"╝");
				}
				else // any other line in the window
				{
					string windowContentSpace = new string(' ', window.Width - 2);
					Console.WriteLine(@"║" + windowContentSpace + @"║");
				}
			}
			Console.ResetColor();
		}
		
		public Render(ConsoleColor windowsBordersColor = ConsoleColor.Cyan, 
			ConsoleColor windowsBackgroundColor = ConsoleColor.DarkBlue, 
			ConsoleColor selectedItemColor = ConsoleColor.Red, 
			ConsoleColor foldersColor = ConsoleColor.Yellow, 
			ConsoleColor filesColor = ConsoleColor.White,
			ConsoleColor backgroundAuxiliaryElementsColor = ConsoleColor.DarkCyan,
			ConsoleColor foregroundAuxiliaryElementsColor = ConsoleColor.Black)
		{
			int windowsHeight = Console.WindowHeight - 5;
			int windowWidth = Console.WindowWidth / 2;
			
			_leftWindow = new Window(
				windowWidth, windowsHeight, 1, 2, WindowContentType.Directory
				);
			_rightWindow = new Window(
				windowWidth, windowsHeight, _leftWindow.LeftPadding + _leftWindow.Width, 2, WindowContentType.FileInfo
			);

			//_leftWindow.ContentType = WindowContentType.Drives;
			//_rightWindow.ContentType = WindowContentType.FileInfo;
			_windowsBordersColor = windowsBordersColor;
			_windowsBackgroundColor = windowsBackgroundColor;
			_selectedItemColor = selectedItemColor;
			_foldersColor = foldersColor;
			_filesColor = filesColor;
			_backgroundAuxiliaryElementsColor = backgroundAuxiliaryElementsColor;
			_foregroundAuxiliaryElementsColor = foregroundAuxiliaryElementsColor;
		}
	}
}
