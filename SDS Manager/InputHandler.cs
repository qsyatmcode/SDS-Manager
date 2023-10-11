using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDSManager
{
	enum ActionType
	{
		Open,
		Cancel,
		Up,
		Down,
		None
	}

	class InputHandler
	{
		private readonly ConsoleKey[] _openKeys;
		private readonly ConsoleKey[] _cancelKeys;
		
		public ActionType Read()
		{
			ConsoleKey pressedKey = Console.ReadKey().Key;

			foreach (var key in _openKeys)
			{
				if (pressedKey == key)
				{
					return ActionType.Open;
				}
			}
			foreach (var key in _cancelKeys)
			{
				if (pressedKey == key)
				{
					return ActionType.Cancel;
				}
			}

			return ActionType.None;
		}

		public InputHandler(ConsoleKey[] openKeys, ConsoleKey[] cancelKeys)
		{
			_openKeys = openKeys;
			_cancelKeys = cancelKeys;
		}
	}
}
