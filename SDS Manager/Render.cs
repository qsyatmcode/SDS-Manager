﻿using System;
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

		public void Draw()
		{
			Console.Clear();
			
			/* TODO: РАЗДЕЛИТЬ ВВОД (ОБРАБОТКА НАЖАТОЙ КЛАВИШИ И ПОСЛЕДЮЩИЕ ДЕЙСТВИЯ) И ВЫВОД (МЕТОДЫ С ПРИСТАВКОЙ DRAW)
				Нужно поместить обработку ввода в отдельный класс, и сделать всё максимально гибким и расширяемым.
			*/
			DrawWindows();

			DrawContent();
		}

		private void DrawContent()
		{
			Window[] windows = new Window[2] { _leftWindow, _rightWindow };

			foreach (var window in windows)
			{
				
				DrawWindowsContent(window);
				
			}
		}

		private void DrawWindowsContent(Window window)
		{
			
		}

		private Window GetOtherWindow(Window window)
		{
			Window otherWindow;
			if (window == _leftWindow)
				return _rightWindow;
			else
				return _leftWindow;
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
