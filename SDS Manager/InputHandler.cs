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
		private readonly ConsoleKey[] _upKeys;
		private readonly ConsoleKey[] _downKeys;

		private readonly Dictionary<ConsoleKey[], ActionType> _keyType;

		public ActionType Read()
		{
			ConsoleKey pressedKey = Console.ReadKey().Key;

			var types = new[] { _openKeys, _cancelKeys, _upKeys, _downKeys};

			foreach (var type in types)
			{
				foreach (var key in type)
				{
					if (pressedKey == key)
					{
						return _keyType[type];
					}
				}
			}

			return ActionType.None;
		}

		public InputHandler(ConsoleKey[] openKeys, ConsoleKey[] cancelKeys, ConsoleKey[] upKeys, ConsoleKey[] downKeys)
		{
			_openKeys = openKeys;
			_cancelKeys = cancelKeys;
			_upKeys = upKeys;
			_downKeys = downKeys;

			_keyType = new Dictionary<ConsoleKey[], ActionType>
			{
				[_openKeys] = ActionType.Open,
				[_cancelKeys] = ActionType.Cancel,
				[_upKeys] = ActionType.Up,
				[_downKeys] = ActionType.Down
			};
		}
	}
}
