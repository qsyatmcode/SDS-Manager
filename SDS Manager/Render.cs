using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

		private readonly Window _leftWindow;
		private readonly Window _rightWindow;

		public void Draw(DirectoryInfo currentDirectory, ConsoleKey pressedKey)
		{
			Console.Clear();
			
			DrawWindows();
			//DrawHelp();
			DrawContent(pressedKey);
			//DrawWindows();
		}

		private int _selectedIndex = 0;
		private void DrawContent(ConsoleKey pressedKey)
		{
			Window[] windows = new Window[2] { _leftWindow, _rightWindow };

			foreach (var window in windows)
			{
				
				DrawWindowsContent(window, pressedKey);
				
			}
		}

		private void DrawWindowsContent(Window window, ConsoleKey pressedKey)
		{
			int leftContentDrawPadding = window.LeftPadding;

			int topPos = window.TopPadding; // Draw top pos

			if (window.ContentType == WindowContentType.Drives)
			{
				if (pressedKey == ConsoleKey.DownArrow)
				{
					GetOtherWindow(window).ContentType = WindowContentType.FileInfo;
					_selectedIndex++;
				}
				else if (pressedKey == ConsoleKey.UpArrow)
				{
					GetOtherWindow(window).ContentType = WindowContentType.FileInfo;
					_selectedIndex--;
				}

				if (_selectedIndex < 0)
				{
					_selectedIndex = 100;
				}
				else if (_selectedIndex > 110)
				{
					_selectedIndex = 0;
				}



				string[] drives = Environment.GetLogicalDrives();

				int selectedIndex = _selectedIndex % drives.Length;

				for (int i = 0; i < drives.Length; i++)
				{
					Console.SetCursorPosition(leftContentDrawPadding, topPos);

					if (selectedIndex == i)
					{
						window.SelectedFolder = new DirectoryInfo(drives[i]);
						Console.ForegroundColor = _selectedItemColor;
						Console.BackgroundColor = _windowsBackgroundColor;

						InputProcessing(pressedKey, window, drives[i]);
					}
					else
					{
						Console.ForegroundColor = _foldersColor;
						Console.BackgroundColor = _windowsBackgroundColor;
					}

					Console.Write(drives[i]); // WRITE?

					topPos++;
				}

				Console.SetCursorPosition(0, Console.WindowHeight - 1);
				Console.Write($"\tsI: {selectedIndex} | _sI: {_selectedIndex} {pressedKey} {window.ContentType}");
				Console.ResetColor();
				return;

			}else if (window.ContentType == WindowContentType.Directory)
			{
				Console.BackgroundColor = _windowsBackgroundColor;
				if (pressedKey == ConsoleKey.DownArrow)
				{
					_selectedIndex++;
					GetOtherWindow(window).ContentType = WindowContentType.FileInfo;
				}
				else if (pressedKey == ConsoleKey.UpArrow)
				{
					_selectedIndex--;
					GetOtherWindow(window).ContentType = WindowContentType.FileInfo;
				}

				if (_selectedIndex < 0)
				{
					_selectedIndex = 100;
				}
				else if (_selectedIndex > 110)
				{
					_selectedIndex = 0;
				}

				int contentIndex = 0; //index of item in current directory

				// DIRECTORY INFO:
				if ((window.CurrentDirectory.GetDirectories().Length + window.CurrentDirectory.GetFiles().Length) == 0)
				{
					Console.SetCursorPosition(leftContentDrawPadding, topPos);
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Write("EMPTY");

					InputProcessing(pressedKey, window, null);
					return;
				}
				int selectedIndex = _selectedIndex % (window.CurrentDirectory.GetDirectories().Length +
				                                      window.CurrentDirectory.GetFiles().Length);
				//SUBDIRS DRAW
				var dirs = window.CurrentDirectory.GetDirectories();
				for (int i = 0; i < dirs.Length; i++)
				{
					Console.SetCursorPosition(leftContentDrawPadding, topPos);
					Console.ForegroundColor = _foldersColor;

					if (selectedIndex == contentIndex)
					{
						window.SelectedFolder = dirs[i];
						Console.ForegroundColor = _selectedItemColor;

						InputProcessing(pressedKey, window, dirs[i].FullName);
					}

					int trimSymbolsCount = dirs[i].Name.Length - window.Width;
					if (trimSymbolsCount > 0)
						Console.Write($"{contentIndex}." +
						              dirs[i].Name.Remove(dirs[i].Name.Length - trimSymbolsCount,
							              trimSymbolsCount)); // WRITE?
					else
						Console.Write($"{contentIndex}." + dirs[i].Name); // WRITE?

					topPos++;
					contentIndex++;
				}

				//FILES DRAW
				var files = window.CurrentDirectory.GetFiles();
				for (int i = 0; i < files.Length; i++)
				{

					Console.SetCursorPosition(leftContentDrawPadding, topPos);
					Console.ForegroundColor = _filesColor;

					if (selectedIndex == contentIndex)
					{
						window.SelectedFile = files[i];
						Console.ForegroundColor = _selectedItemColor;

						InputProcessing(pressedKey, window, null);
					}

					int trimSymbolsCount = files[i].Name.Length - window.Width;
					if (trimSymbolsCount > 0)
						Console.Write($"{contentIndex}." +
						              files[i].Name.Remove(files[i].Name.Length - trimSymbolsCount,
							              trimSymbolsCount)); // WRITE?
					else
						Console.Write($"{contentIndex}." + files[i].Name); // WRITE?

					topPos++;
					contentIndex++;
				}

				Console.SetCursorPosition(0, Console.WindowHeight - 1);
				Console.Write("\tsI:" + selectedIndex.ToString() + "| _sI:" + _selectedIndex.ToString() + " " + pressedKey.ToString() + " " + window.ContentType.ToString());
				Console.ResetColor();

				

				return;
			}else if (window.ContentType == WindowContentType.FileInfo)
			{
				Console.SetCursorPosition(leftContentDrawPadding, topPos);
				topPos++;
				Console.Write("FILE INFO TEST VERSION:");

				Window otherWindow;
				if (window == _leftWindow)
					otherWindow = _rightWindow;
				else
					otherWindow = _leftWindow;

				if (otherWindow.SelectedFile != null)
				{
					//TODO:
					const int countOfData = 5;
					string[] DataOfFile = new string[countOfData]
					{
						otherWindow.SelectedFile.Name,
						otherWindow.SelectedFile.Length.ToString(),
						otherWindow.SelectedFile.DirectoryName,
						otherWindow.SelectedFile.Name,
						otherWindow.SelectedFile.IsReadOnly.ToString()
					};

					for (int i = 0; i < countOfData; i++)
					{
						Console.SetCursorPosition(leftContentDrawPadding, topPos);

						Console.Write(DataOfFile[i]);

						topPos++;
					}

					Console.ResetColor();
					Console.SetCursorPosition(0, Console.WindowHeight - 1);
					Console.Write("0 |" + _selectedIndex.ToString() + pressedKey.ToString() + window.ContentType.ToString());
					return;
				}else if (otherWindow.SelectedFolder != null)
				{
					const int countOfData = 3;
					string[] DataOfFile = new string[countOfData]
					{
						otherWindow.SelectedFolder.Name,
						otherWindow.SelectedFolder.FullName,
						otherWindow.SelectedFolder.Name
					};

					for (int i = 0; i < countOfData; i++)
					{
						Console.SetCursorPosition(leftContentDrawPadding, topPos);

						Console.Write(DataOfFile[i]);

						topPos++;
					}

					Console.ResetColor();
					Console.SetCursorPosition(0, Console.WindowHeight - 1);
					Console.Write("\t"+ "| _sI:" + _selectedIndex.ToString() + " " + pressedKey.ToString() + " " + window.ContentType.ToString());
					return;
				}
			}else if (window.ContentType == WindowContentType.TextView)
			{
				if (window.SelectedFile == null)
				{
					return;
				}
				Window otherWindow = GetOtherWindow(window);

				if (otherWindow.SelectedFile == null)
				{
					return;
				}
				string[] lines = File.ReadAllLines(otherWindow.SelectedFile.FullName); // may be null

				//int leftPadding = otherWindow.LeftPadding;
				int countOfLines = otherWindow.TopPadding + otherWindow.Height;
				int maxLineLength = otherWindow.Width;

				foreach (var line in lines)
				{
					if (topPos - window.TopPadding > countOfLines)
					{
						break;
					}
					Console.SetCursorPosition(leftContentDrawPadding, topPos);
					Console.ForegroundColor = _selectedItemColor;
					Console.BackgroundColor = _windowsBackgroundColor;

					int trimSymbolsCount = line.Length - maxLineLength;
					Console.Write(line.Remove(line.Length - trimSymbolsCount, trimSymbolsCount));

					topPos++;
				}

				return;
			}
		}

		private Window GetOtherWindow(Window window)
		{
			Window otherWindow;
			if (window == _leftWindow)
				return _rightWindow;
			else
				return _leftWindow;
		}

		private void InputProcessing(ConsoleKey pressedKey, Window window, string? enterPath)
		{
			if (pressedKey == ConsoleKey.Enter || pressedKey == ConsoleKey.RightArrow)
			{
				if (enterPath == null) // If it is not a directory, but a file
				{
					Window otherWindow;
					if (window == _leftWindow)
					{
						otherWindow = _rightWindow;
					}
					else
					{
						otherWindow = _leftWindow;
					}

					otherWindow.ContentType = WindowContentType.TextView;

					return;
				}
				window.PrevDirectories.Add(window.CurrentDirectory);
				window.CurrentDirectory = new DirectoryInfo(enterPath);
				if(window.ContentType != WindowContentType.Directory) window.ContentType = WindowContentType.Directory;
			}
			else if (pressedKey == ConsoleKey.Backspace || pressedKey == ConsoleKey.LeftArrow)
			{
				if (window.PrevDirectories.Count == 0)
				{
					window.ContentType = WindowContentType.Drives;
				}
				else
				{
					window.CurrentDirectory = window.PrevDirectories.Last();
					window.PrevDirectories.RemoveAt(window.PrevDirectories.Count - 1);
				}
			}
			else
			{
				// No control key is pressed
			}
		}

		private void DrawWindows()
		{
			DrawWindowBorder(_leftWindow);
			DrawWindowBorder(_rightWindow);
		}

		private string Title(Window window)
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

		private void DrawWindowBorder(Window window) // ║ ╗ ╝ ╔ ═ ╚ 
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

		public Render(ConsoleColor windowsBordersColor, ConsoleColor windowsBackgroundColor, ConsoleColor selectedItemColor, ConsoleColor foldersColor, ConsoleColor filesColor)
		{
			int windowsHeight = Console.WindowHeight - 5;
			int windowWidth = Console.WindowWidth / 2;
			
			_leftWindow = new Window(
				windowWidth, windowsHeight, 1, 2
				);
			_rightWindow = new Window(
				windowWidth, windowsHeight, _leftWindow.LeftPadding + _leftWindow.Width, 2
			);

			_leftWindow.ContentType = WindowContentType.Drives;
			_rightWindow.ContentType = WindowContentType.FileInfo;
			_windowsBordersColor = windowsBordersColor;
			_windowsBackgroundColor = windowsBackgroundColor;
			_selectedItemColor = selectedItemColor;
			_foldersColor = foldersColor;
			_filesColor = filesColor;
		}
	}
}
