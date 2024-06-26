﻿using System;

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

		public void ProcessAction(Action action)
		{
			Window window = GetWindowByContentType(WindowContentType.Directory);
			GetOtherWindow(window).ContentType = WindowContentType.FileInfo;
			
			action.Process(window, GetOtherWindow(window));
			ChangeSelected();

			void ChangeSelected()
			{

				if (window.ContentObjects.Length <= 0)
				{
					window.SelectedFolder = null;
					window.SelectedFile = null;
					return;
				} 
				else if (window.ContentObjects[window.SelectedObjectIndex] is DirectoryInfo)
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
			int leftPadding = window.LeftPadding;
			int topPos = window.TopPadding;

			int creationDateWidth = 12;
			int procentWidth = 3;
			int titleWidth = window.Width - creationDateWidth - procentWidth;

			if (window.ContentType == WindowContentType.FileInfo)
			{
				Console.ForegroundColor = _filesColor;

				FileInfoDraw();

				return;
			} else if (window.ContentType == WindowContentType.TextView)
			{
				Console.ForegroundColor = _filesColor;
				
				TextViewDraw();

				return;
			}else if (window.ContentType == WindowContentType.Directory)
			{
				Console.BackgroundColor = _windowsBackgroundColor;

				if (window.ContentObjects.Length <= 0)
				{
					EmptyFolderFill();
					return;
				}

				int drawObjectsLength = window.Height <= window.ContentObjects.Length ? window.Height : window.ContentObjects.Length;


				object[] contentObjectsToDraw = new object[window.Height];
				if (window.Height - window.SelectedObjectIndex <= 0)
				{
					int offset = (int)Math.Abs(offset = window.Height - window.SelectedObjectIndex) + 1;

					Array.Copy(window.ContentObjects, offset, contentObjectsToDraw, 0, drawObjectsLength);
				}
				else
				{
					Array.Copy(window.ContentObjects, contentObjectsToDraw, drawObjectsLength);
				}

				for (int i = 0; i < contentObjectsToDraw.Length; i++)
				{
					if (topPos >= window.Height + window.TopPadding)
						break; // out of window borders

					if (i == window.SelectedObjectIndex || contentObjectsToDraw[i] == window.ContentObjects[window.SelectedObjectIndex])
					{
						Console.ForegroundColor = _selectedItemColor;
						DrawObject(contentObjectsToDraw[i]);
					}
					else if (contentObjectsToDraw[i] is DirectoryInfo)
					{
						Console.ForegroundColor = _foldersColor;
						DrawObject(contentObjectsToDraw[i]);
					}
					else if (contentObjectsToDraw[i] is FileInfo)
					{
						Console.ForegroundColor = _filesColor;
						DrawObject(contentObjectsToDraw[i]);
					}

					topPos++;
				}
			}


			void EmptyFolderFill()
			{
				Console.ForegroundColor = _foldersColor;
				string sign = "EMPTY FOLDER";
				int topMiddle = topPos + (window.Height / 2);
				int leftMiddle = leftPadding + (window.Width / 2) - (sign.Length / 2);
				Console.SetCursorPosition(leftMiddle, topMiddle);
				Console.Write(sign);
			}

			void DrawObject(object obj)
			{
				DateTime creationTime = GetCreationTime();
				string title = GetTitle();
				string creationTimeString = GetDateString();

				Console.SetCursorPosition(leftPadding, topPos);
				Console.Write(title);
				Console.Write(creationTimeString);

				string GetDateString()
				{
					return creationTime.ToString("MM/dd/yyyy");
				}

				string GetTitle()
				{
					if (obj is DirectoryInfo drawingDirectory)
					{
						if (drawingDirectory.Name.Length > titleWidth)
							return drawingDirectory.Name.Substring(0, titleWidth);
						else
							return drawingDirectory.Name + new string(' ', titleWidth - drawingDirectory.Name.Length);
					}
					else if (obj is FileInfo drawingFile)
					{
						if (drawingFile.Name.Length > titleWidth)
							return drawingFile.Name.Substring(0, titleWidth);
						else
							return drawingFile.Name + new string(' ', titleWidth - drawingFile.Name.Length);
					}
					else
					{
						throw new ArgumentException();
					}
				}

				DateTime GetCreationTime()
				{
					if (obj is DirectoryInfo drawingDirectory)
					{
						return drawingDirectory.CreationTime;

					}
					else if (obj is FileInfo drawingFile)
					{
						return drawingFile.CreationTime;
					}
					else
					{
						throw new ArgumentException();
					}
				}
			}

			void FileInfoDraw()
			{
				Window otherWindow = GetOtherWindow(window);
				string fileName = otherWindow.SelectedFile == null ? 
					otherWindow.SelectedFolder?.Name ?? "NO INFO" : otherWindow.SelectedFile?.Name ?? "NO INFO";
				long size = otherWindow.SelectedFile?.Length ?? 0;

				Console.SetCursorPosition(leftPadding, topPos + 10);
				Console.Write(new string(' ', ((window.Width - 2) - fileName.Length) / 2) + fileName);

				Console.SetCursorPosition(leftPadding, topPos + 12);
				Console.Write(new string(' ', ((window.Width - 2) - (size.ToString().Length + 6)) / 2) + $"{size:n0} bytes");
			}

			void TextViewDraw()
			{
				string fileName = GetOtherWindow(window).SelectedFile.Name;
				string[] content = new string[] { "NO INFO" };
				try
				{
					content = File.ReadAllLines(GetOtherWindow(window).SelectedFile?.FullName ?? "NO INFO");
				}
				catch { }

				Console.SetCursorPosition(leftPadding, topPos);
				Console.Write(new string(' ', (window.Width - fileName.Length) / 2) + $" {fileName} ");
				topPos++;
				foreach (var line in content)
				{
					if (topPos >= window.TopPadding + window.Height)
						break;
					Console.SetCursorPosition(leftPadding, topPos);

					if(line.Length > window.Width)
						Console.WriteLine(line.Remove(window.Width - 2));
					else
						Console.WriteLine(line);

					topPos++;
				}
			}
		}

		private long DirectorySize(DirectoryInfo directory)
		{
			long size = 0;

			FileInfo[] fis = null;
			try
			{
				fis = directory.GetFiles();
			}
			catch
			{
				return size;
			}

			foreach (FileInfo fi in fis)
			{
				size += fi.Length;
			}

			return size;
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

		private void DrawWindowBorder(Window window) // ║ ╗ ╝ ╔ ═ ╚ 
		{
			string windowTitle = Title();

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
					if (window.ContentType == WindowContentType.Directory)
					{
						string size = DirSizeStr();
						string bottomBorder = new string('\u2550', ((window.Width - 2) - size.Length) / 2);
						Console.WriteLine(@"╚" + bottomBorder + size + bottomBorder + @"╝");
					}
					else
					{
						string bottomBorder = new string('\u2550', (window.Width - 2));
						Console.WriteLine(@"╚" + bottomBorder + @"╝");
					}
				}
				else // any other line in the window
				{
					string windowContentSpace = new string(' ', window.Width - 2);
					Console.WriteLine(@"║" + windowContentSpace + @"║");
				}
			}
			Console.ResetColor();

			string DirSizeStr()
			{
				long size = DirectorySize(new(GetParentFolderName(window.CurrentDirectory.FullName)));

				return new string($" {size:n0} bytes ");

				string GetParentFolderName(string fullFileName)
				{
					int pos = fullFileName.LastIndexOf(@"\");
					return fullFileName.Substring(0, pos + 1); // without last slash
				}
			}
			string Title()
			{
				string result = "";

				if (window.ContentType == WindowContentType.Directory)
				{
					result = window.CurrentDirectory.FullName;
				}
				else if (window.ContentType == WindowContentType.FileInfo || window.ContentType == WindowContentType.TextView)
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

				if (result.Length % 2 != 0)
				{
					result = result.PadRight(result.Length + 1);
				}

				return result;
			}
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
