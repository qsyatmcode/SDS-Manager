﻿namespace SDSManager
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.SetWindowSize(200, 50);
			Console.SetBufferSize(200, 50);

			Render render = new Render(
				ConsoleColor.Cyan,
				ConsoleColor.DarkBlue,
				ConsoleColor.Red,
				ConsoleColor.Yellow,
				ConsoleColor.White
				);

			DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);

			ConsoleKey pressedKey = ConsoleKey.A;

			bool flag = false;
			while (true)
			{
				render.Draw(currentDirectory, pressedKey);

				pressedKey = Console.ReadKey().Key;

				//break;
			}

			Console.ReadKey();
		}
	}
}