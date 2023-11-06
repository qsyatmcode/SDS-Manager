using System;

namespace SDSManager
{
	internal class Program
	{
		static void Main(string[] args)
		{
			int consoleWidth = 200;
			int consoleHeight = 50;
			Console.SetWindowSize(consoleWidth, consoleHeight);
			Console.SetBufferSize(consoleWidth, consoleHeight);




			InputHandler inputHandler = new InputHandler(
				new ConsoleKey[] {ConsoleKey.Enter, ConsoleKey.RightArrow},
				new ConsoleKey[] {ConsoleKey.Backspace, ConsoleKey.LeftArrow},
				new ConsoleKey[] { ConsoleKey.UpArrow },
				new ConsoleKey[] { ConsoleKey.DownArrow }
				);

			Render render = new Render();

			DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);

			while (true)
			{
				render.Draw();

				render.ProcessAction(inputHandler.Read());
			}

			Console.ReadKey();
		}
	}
}