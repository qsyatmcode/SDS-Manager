using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDSManager
{
	abstract class Action
	{
		public abstract void Process(Window window, Window otherWindow);

		public readonly ConsoleKey[] Keys;

		protected Action(params ConsoleKey[] keys)
		{
			Keys = keys;
		}

		public bool HasAKey(ConsoleKey desiredKey)
		{
			if(Keys.Length == 0) return false;
			
			foreach (var key in Keys)
			{
				if(key ==  desiredKey) return true;
			}

			return false;
		}

		protected object[] GetDirectoryContent(DirectoryInfo directory)
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

		//protected Window GetOtherWindow(Window window)
		//{
		//	Window otherWindow;
		//	if (window == _leftWindow)
		//		return _rightWindow;
		//	else
		//		return _leftWindow;
		//}
	}

	sealed class NoneAction : Action
	{
		public override void Process(Window window, Window otherWindow)
		{
			return;
		}

		public NoneAction() {}
	}

	sealed class OpenAction : Action
	{
		public override void Process(Window window, Window otherWindow)
		{
			//Console.Beep(750, 150);

			if (window.SelectedFolder != null)
			{
				if (window.CurrentDirectory != null) window.PrevDirectories.Add(window.CurrentDirectory);
				window.CurrentDirectory = window.SelectedFolder;
				window.ContentObjects = GetDirectoryContent(window.CurrentDirectory);
			}
			else if (window.SelectedFile != null)
			{
				otherWindow.ContentType = WindowContentType.TextView;

			}
		}

		public OpenAction(params ConsoleKey[] keys) : base(keys) {}
	}

	sealed class CancelAction : Action
	{
		public override void Process(Window window, Window otherWindow)
		{
			//Console.Beep(500, 150);

			if (window.PrevDirectories.Count > 0)
			{
				window.CurrentDirectory = window.PrevDirectories.Last();
				window.PrevDirectories.RemoveAt(window.PrevDirectories.Count - 1);
				window.ContentObjects = GetDirectoryContent(window.CurrentDirectory);
			}
		}

		public CancelAction(params ConsoleKey[] keys) : base(keys) {}
	}

	sealed class UpAction : Action
	{
		public override void Process(Window window, Window otherWindow)
		{
			window.SelectedObjectIndex--;
		}

		public UpAction(params ConsoleKey[] keys) : base(keys) {}
	}

	sealed class DownAction : Action
	{
		public override void Process(Window window, Window otherWindow)
		{
			window.SelectedObjectIndex++;
		}

		public DownAction(params ConsoleKey[] keys) : base(keys) {}
	}
}
