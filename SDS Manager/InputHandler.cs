using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDSManager
{
	//enum ActionType
	//{
	//	Open,
	//	Cancel,
	//	Up,
	//	Down,
	//	None
	//}

	class InputHandler
	{
		private readonly OpenAction _openAction;
		private readonly CancelAction _cancelAction;
		private readonly UpAction _upAction;
		private readonly DownAction _downAction;
		private readonly NoneAction _noneAction;


		public Action Read()
		{
			ConsoleKey pressedKey = Console.ReadKey().Key;

			Action[] actionArray = new Action[] {_openAction, _cancelAction, _downAction, _upAction, _noneAction};

			foreach (var action in actionArray)
			{
				if(action.HasAKey(pressedKey)) return action;
			}

			return _noneAction;
		}

		public InputHandler(ConsoleKey[] OpenKeys, ConsoleKey[] CancelKeys, ConsoleKey[] UpKeys, ConsoleKey[] DownKeys)
		{
			_openAction = new OpenAction(OpenKeys);
			_cancelAction = new CancelAction(CancelKeys);
			_upAction = new UpAction(UpKeys);
			_downAction = new DownAction(DownKeys);
			_noneAction = new NoneAction();
		}
	}
}
