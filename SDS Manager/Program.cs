namespace SDSManager
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.SetWindowSize(200, 50);
			Console.SetBufferSize(200, 50);

			InputHandler inputHandler = new InputHandler(
				new ConsoleKey[] {ConsoleKey.Enter, ConsoleKey.RightArrow},
				new ConsoleKey[] {ConsoleKey.Backspace, ConsoleKey.LeftArrow}
				);

			Render render = new Render();

			DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);

			ConsoleKey pressedKey = ConsoleKey.A;

			while (true)
			{
				render.ProcessAction(inputHandler.Read());

				render.Draw();

				pressedKey = Console.ReadKey().Key;
			}

			Console.ReadKey();
		}
	}
}