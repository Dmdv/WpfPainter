﻿using System;
using System.Windows.Input;

namespace WpfPainter.Commands
{
	/// <summary>
	/// 	Simplistic implementation of command.
	/// </summary>
	public class DelegateSimpleCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public DelegateSimpleCommand(Action<object> executeAction, Func<object, bool> canExecute)
		{
			_executeAction = executeAction;
			_canExecute = canExecute;
		}

		/// <summary>
		/// 	Checks if a command should be executed.
		/// </summary>
		/// <param name="parameter"> </param>
		/// <returns> </returns>
		public bool CanExecute(object parameter)
		{
			var temp = _canExecute(parameter);

			if (_canExecuteCache != temp)
			{
				_canExecuteCache = temp;

				if (CanExecuteChanged != null)
				{
					CanExecuteChanged(this, new EventArgs());
				}
			}

			return _canExecuteCache;
		}

		/// <summary>
		/// 	Executes the command.
		/// </summary>
		/// <param name="parameter"> </param>
		public void Execute(object parameter)
		{
			_executeAction(parameter);
		}

		private readonly Func<object, bool> _canExecute;

		private readonly Action<object> _executeAction;

		private bool _canExecuteCache;
	}
}