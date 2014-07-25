using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using WpfPainter.Properties;

namespace WpfPainter.Commands
{
	/// <summary>
	/// 	The CompositeCommand composites one or more ICommands.
	/// </summary>
	public class CompositeCommand : ICommand
	{
		/// <summary>
		/// Occurs when any of the registered commands raise <see cref="ICommand.CanExecuteChanged" /> . 
		/// You must keep a hard reference to the handler to avoid garbage collection and unexpected results. 
		/// See remarks for more information.
		/// </summary>
		/// <remarks>
		/// 	When subscribing to the <see cref="ICommand.CanExecuteChanged" /> 
		/// event using code (not when binding using XAML) will need to keep a hard reference to the event handler. 
		/// This is to prevent garbage collection of the event handler because the command implements the Weak Event 
		/// pattern so it does not have a hard reference to this handler. An example implementation can be seen in 
		/// the CompositeCommand and CommandBehaviorBase classes. In most scenarios, there is no reason to sign up 
		/// to the CanExecuteChanged event directly, but if you do, you are responsible for maintaining the reference.
		/// </remarks>
		/// <example>
		/// The following code holds a reference to the event handler. 
		/// The myEventHandlerReference value should be stored in an instance member to avoid 
		/// it from being garbage collected. 
		/// <code>EventHandler myEventHandlerReference = new EventHandler(this.OnCanExecuteChanged);
		/// command.CanExecuteChanged += myEventHandlerReference;</code>
		/// </example>
		public event EventHandler CanExecuteChanged
		{
			add { WeakEventHandlerManager.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2); }
			remove { WeakEventHandlerManager.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value); }
		}

		/// <summary>
		/// 	Initializes a new instance of <see cref="CompositeCommand" /> .
		/// </summary>
		public CompositeCommand()
		{
			_onRegisteredCommandCanExecuteChangedHandler = OnRegisteredCommandCanExecuteChanged;
		}

		/// <summary>
		/// 	Initializes a new instance of <see cref="CompositeCommand" /> .
		/// </summary>
		/// <param name="monitorCommandActivity"> Indicates when the command activity is going to be monitored. </param>
		public CompositeCommand(bool monitorCommandActivity)
			: this()
		{
			_monitorCommandActivity = monitorCommandActivity;
		}

		/// <summary>
		/// 	Gets the list of all the registered commands.
		/// </summary>
		/// <value> A list of registered commands. </value>
		/// <remarks>
		/// 	This returns a copy of the commands subscribed to the CompositeCommand.
		/// </remarks>
		public IList<ICommand> RegisteredCommands
		{
			get
			{
				IList<ICommand> commandList;
				lock (_registeredCommands)
				{
					commandList = _registeredCommands.ToList();
				}

				return commandList;
			}
		}

		/// <summary>
		/// Forwards <see cref="ICommand.CanExecute" /> to the registered commands and returns 
		/// <see langword="true" /> if all of the commands return <see
		/// langword="true" /> .
		/// </summary>
		/// <param name="parameter"> Data used by the command. 
		/// If the command does not require data to be passed, this object can be set to 
		/// <see langword="null" /> . </param>
		/// <returns> <see langword="true" /> if all of the commands return <see langword="true" /> ; otherwise, 
		/// <see langword="false" /> . </returns>
		public virtual bool CanExecute(object parameter)
		{
			var commandsThatShouldBeExecuted = false;

			ICommand[] commandList;
			lock (_registeredCommands)
			{
				commandList = _registeredCommands.ToArray();
			}

			foreach (var command in commandList.Where(ShouldExecute))
			{
				if (!command.CanExecute(parameter))
				{
					return false;
				}

				commandsThatShouldBeExecuted = true;
			}

			return commandsThatShouldBeExecuted;
		}

		/// <summary>
		/// 	Forwards <see cref="ICommand.Execute" /> to the registered commands.
		/// </summary>
		/// <param name="parameter"> Data used by the command. If the command does not require data to be passed, 
		/// this object can be set to <see langword="null" /> . </param>
		public virtual void Execute(object parameter)
		{
			Queue<ICommand> commands;
			lock (_registeredCommands)
			{
				commands = new Queue<ICommand>(_registeredCommands.Where(ShouldExecute).ToList());
			}

			while (commands.Count > 0)
			{
				var command = commands.Dequeue();
				command.Execute(parameter);
			}
		}

		/// <summary>
		/// Adds a command to the collection and signs up for the 
		/// <see cref="ICommand.CanExecuteChanged" /> event of it.
		/// </summary>
		/// <remarks>
		/// If this command is set to monitor command activity, and <paramref name="command" /> implements the 
		/// <see cref="IActiveAware" /> interface, this method will subscribe to its 
		/// <see cref="IActiveAware.IsActiveChanged" /> event.
		/// </remarks>
		/// <param name="command"> The command to register. </param>
		public virtual void RegisterCommand(ICommand command)
		{
			if (command == this)
			{
				throw new ArgumentException(Resources.CannotRegisterCompositeCommandInItself);
			}

			lock (_registeredCommands)
			{
				if (_registeredCommands.Contains(command))
				{
					throw new InvalidOperationException(Resources.CannotRegisterSameCommandTwice);
				}
				_registeredCommands.Add(command);
			}

			command.CanExecuteChanged += _onRegisteredCommandCanExecuteChangedHandler;
			OnCanExecuteChanged();

			if (_monitorCommandActivity)
			{
				var activeAwareCommand = command as IActiveAware;
				if (activeAwareCommand != null)
				{
					activeAwareCommand.IsActiveChanged += CommandIsActiveChanged;
				}
			}
		}

		/// <summary>
		/// Removes a command from the collection and removes itself from the 
		/// <see cref="ICommand.CanExecuteChanged" /> event of it.
		/// </summary>
		/// <param name="command"> The command to unregister. </param>
		public virtual void UnregisterCommand(ICommand command)
		{
			bool removed;
			lock (_registeredCommands)
			{
				removed = _registeredCommands.Remove(command);
			}

			if (removed)
			{
				command.CanExecuteChanged -= _onRegisteredCommandCanExecuteChangedHandler;
				OnCanExecuteChanged();

				if (_monitorCommandActivity)
				{
					var activeAwareCommand = command as IActiveAware;
					if (activeAwareCommand != null)
					{
						activeAwareCommand.IsActiveChanged -= CommandIsActiveChanged;
					}
				}
			}
		}

		/// <summary>
		/// 	Raises <see cref="ICommand.CanExecuteChanged" /> on the UI thread so every command invoker can requery
		/// <see cref="ICommand.CanExecute" /> to check if the <see cref="CompositeCommand" /> can execute.
		/// </summary>
		protected virtual void OnCanExecuteChanged()
		{
			WeakEventHandlerManager.CallWeakReferenceHandlers(this, _canExecuteChangedHandlers);
		}

		/// <summary>
		/// 	Evaluates if a command should execute.
		/// </summary>
		/// <param name="command"> The command to evaluate. </param>
		/// <returns> A <see cref="bool" /> value indicating whether the command should be used when evaluating 
		/// <see cref="CompositeCommand.CanExecute" /> and <see cref="CompositeCommand.Execute" /> . </returns>
		/// <remarks> If this command is set to monitor command activity, and <paramref name="command" /> 
		/// implements the <see cref="IActiveAware" /> interface, this method will return <see langword="false" /> 
		/// if the command's <see cref="IActiveAware.IsActive" /> property is <see langword="false" /> ; 
		/// otherwise it always returns <see langword="true" /> .
		/// </remarks>
		protected virtual bool ShouldExecute(ICommand command)
		{
			var activeAwareCommand = command as IActiveAware;

			if (_monitorCommandActivity && activeAwareCommand != null)
			{
				return activeAwareCommand.IsActive;
			}

			return true;
		}

		/// <summary>
		/// 	Handler for IsActiveChanged events of registered commands.
		/// </summary>
		/// <param name="sender"> The sender. </param>
		/// <param name="e"> EventArgs to pass to the event. </param>
		private void CommandIsActiveChanged(object sender, EventArgs e)
		{
			OnCanExecuteChanged();
		}

		private void OnRegisteredCommandCanExecuteChanged(object sender, EventArgs e)
		{
			OnCanExecuteChanged();
		}

		private readonly bool _monitorCommandActivity;
		private readonly EventHandler _onRegisteredCommandCanExecuteChangedHandler;
		private readonly List<ICommand> _registeredCommands = new List<ICommand>();
		private List<WeakReference> _canExecuteChangedHandlers;
	}
}