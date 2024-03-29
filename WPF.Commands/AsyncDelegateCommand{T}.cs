﻿using System;
using System.Threading.Tasks;

namespace WPF.Commands
{
    /// <summary>
    /// ICommand implementation which force execution on separate thread. Takes arbitrary paremeter as input.
    /// </summary>
    /// <typeparam name="T">Command parameter type.</typeparam>
    public class AsyncDelegateCommand<T> : AsyncDelegateCommandBase
    {
        private readonly Func<T, bool>? canExecute;
        private readonly Func<T, Task> execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDelegateCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">Function to execute on separate thread.</param>
        /// <param name="canExecute">Function to determine if <see cref="execute"/> can be executed.</param>
        /// <param name="executeOnce">Execute function only once, after that <see cref="canExecute"/> would return false regardless.</param>
        /// <param name="freezeWhenBusy">UI not blocked when function executed, so user can trigger function multiple times at once. This will prevent it: during execution <see cref="canExecute"/> would return false.</param>
        /// <param name="refreshAutomatically">A value indicating whether CanExecute should be refreshed automatically or manually.</param>
        /// <param name="forceExecutionOnSeparateThread">A value indicating whether Execute must be forced to run on a non-UI thread.</param>
        /// <param name="exceptionHandler">An exception handler function.</param>
        public AsyncDelegateCommand(Func<T, Task> execute,
                                    Func<T, bool>? canExecute = null,
                                    bool executeOnce = false,
                                    bool freezeWhenBusy = true,
                                    bool refreshAutomatically = true,
                                    Func<Exception, bool>? exceptionHandler = null)
            : base(freezeWhenBusy, executeOnce, refreshAutomatically, exceptionHandler)
        {
            this.execute = execute;
            this.canExecute = canExecute;

            CanExecuteSpecified = canExecute != null || FreezeWhenBusy || ExecuteOnce;
        }

        /// <inheritdoc/>
        protected override bool CanExecuteAsyncInternal(object? parameter)
        {
            if (canExecute != null)
            {
                if (parameter is T tParameter)
                {
                    return canExecute(tParameter);
                }
                else if (parameter == null && typeof(T).IsClass)
                {
                    return canExecute(default);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsyncInternal(object? parameter)
        {
            if (parameter is T tParameter)
            {
                await execute(tParameter).ConfigureAwait(false);
            }
            else if (parameter == null)
            {
                var type = typeof(T);
                if (type.IsClass || type.Name.StartsWith("Nullable"))
                {
                    await execute(default).ConfigureAwait(false);
                }
            }
            else
            {
                throw new InvalidCastException($"Command parameter is {parameter?.GetType().FullName}, command expected {typeof(T).FullName}");
            }
        }

        public static implicit operator AsyncDelegateCommand<T>(Func<T, Task> a) => new AsyncDelegateCommand<T>(a);
    }
}
