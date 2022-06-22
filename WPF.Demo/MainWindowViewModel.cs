using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WPF.Commands;

namespace WPF.Demo
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel
    {
        public int Count { get; set; }
        private void IncrementCount() => Count++;
        private async Task AsyncIncrementCount() => Count++;

        public MainWindowViewModel()
        {
            DelegateCommand.GlobalExceptionHandler += exception =>
            {
                MessageBox.Show($"Exception handled globally: {exception.Message}");
                return true;
            };

            DelegateSetAndWaitCommand = (Action)DelegateSetAndWait;
            DelegateWaitAndSetCommand = (Action)DelegateWaitAndSet;
            DelegateExecuteOnceCommand = new DelegateCommand(IncrementCount, executeOnce: true);
            DelegateCannotExecuteCommand = new DelegateCommand(null, canExecute: () => false);
            DelegateCanExecuteManuallySetAutomaticallyUpdatedCommand = new DelegateCommand(IncrementCount, canExecute: () => CanExecuteFirst);
            DelegateCanExecuteManuallySetManuallyUpdatedCommand = new DelegateCommand(IncrementCount, canExecute: () => CanExecuteSecond, refreshAutomatically: false);
            UpdateCanExecuteCommand = (Action)UpdateCanExecute;
            DelegateGenericCommand = (Action<string>)DelegateGeneric;

            DelegateExceptionHandledCommand = new DelegateCommand(DelegateExceptionHandled, exceptionHandler: exception =>
            {
                MessageBox.Show($"Exception handled locally: {exception.Message}");
                return true;
            });
            DelegateExceptionCommand = (Action)DelegateExceptionHandled;

            AsyncDelegateSetAndWaitCommand = (Func<Task>)AsyncDelegateSetAndWait;
            AsyncDelegateSetAndWaitDisabledWhenBusyCommand = new AsyncDelegateCommand(AsyncDelegateSetAndWait);
            AsyncDelegateSetAndWaitDisabledWhenBusyManualCommand = new AsyncDelegateCommand(AsyncDelegateSetAndWait, refreshAutomatically: false);
            AsyncDelegateWaitAndSetCommand = new AsyncDelegateCommand(AsyncDelegateWaitAndSet, freezeWhenBusy: false);
            AsyncDelegateWaitAndSetDisabledWhenBusyCommand = new AsyncDelegateCommand(AsyncDelegateWaitAndSet);
            AsyncDelegateExecuteOnceCommand = new AsyncDelegateCommand(AsyncIncrementCount, executeOnce: true);
            AsyncDelegateCannotExecuteCommand = new AsyncDelegateCommand(null, canExecute: () => false);
            AsyncDelegateCanExecuteManuallySetAutomaticallyUpdatedCommand = new AsyncDelegateCommand(AsyncIncrementCount, canExecute: () => AsyncCanExecuteFirst);
            AsyncDelegateCanExecuteManuallySetManuallyUpdatedCommand = new AsyncDelegateCommand(AsyncIncrementCount, canExecute: () => AsyncCanExecuteSecond, refreshAutomatically: false);
            AsyncUpdateCanExecuteCommand = new AsyncDelegateCommand(AsyncUpdateCanExecute);
            AsyncDelegateGenericCommand = new AsyncDelegateCommand<string>(AsyncDelegateGeneric);

            AsyncDelegateExceptionHandledCommand = new AsyncDelegateCommand(AsyncDelegateExceptionHandled, exceptionHandler: exception =>
            {
                MessageBox.Show($"Exception handled locally: {exception.Message}");
                return true;
            });
            AsyncDelegateExceptionCommand = new AsyncDelegateCommand(AsyncDelegateExceptionHandled);
        }

        public DelegateCommand DelegateSetAndWaitCommand { get; }
        private void DelegateSetAndWait()
        {
            IncrementCount();
            Thread.Sleep(2000);
        }

        public DelegateCommand DelegateWaitAndSetCommand { get; }
        private void DelegateWaitAndSet()
        {
            Thread.Sleep(2000);
            IncrementCount();
        }

        public DelegateCommand DelegateExecuteOnceCommand { get; }
        public DelegateCommand DelegateCannotExecuteCommand { get; }

        public bool CanExecuteFirst { get; set; }
        public DelegateCommand DelegateCanExecuteManuallySetAutomaticallyUpdatedCommand { get; }

        public bool CanExecuteSecond { get; set; }
        // Manually update CanExecute for a command
        private void UpdateCanExecute()
        {
            DelegateCanExecuteManuallySetManuallyUpdatedCommand.RaiseCanExecuteChanged();
        }

        public DelegateCommand DelegateCanExecuteManuallySetManuallyUpdatedCommand { get; }
        public DelegateCommand UpdateCanExecuteCommand { get; }

        public DelegateCommand<string> DelegateGenericCommand { get; }
        private void DelegateGeneric(string value)
        {
            Count += int.Parse(value);
        }
        
        public AsyncDelegateCommand AsyncDelegateSetAndWaitDisabledWhenBusyCommand { get; }
        public AsyncDelegateCommand AsyncDelegateSetAndWaitDisabledWhenBusyManualCommand { get; }
        
        public AsyncDelegateCommand AsyncDelegateSetAndWaitCommand { get; }
        private async Task AsyncDelegateSetAndWait()
        {
            await AsyncIncrementCount();
            await Task.Delay(2000);
        }
        
        public AsyncDelegateCommand AsyncDelegateWaitAndSetDisabledWhenBusyCommand { get; }
        public AsyncDelegateCommand AsyncDelegateWaitAndSetCommand { get; }
        private async Task AsyncDelegateWaitAndSet()
        {
            await Task.Delay(2000);
            await AsyncIncrementCount();
        }

        public AsyncDelegateCommand AsyncDelegateExecuteOnceCommand { get; }
        public AsyncDelegateCommand AsyncDelegateCannotExecuteCommand { get; }

        public bool AsyncCanExecuteFirst { get; set; }
        public AsyncDelegateCommand AsyncDelegateCanExecuteManuallySetAutomaticallyUpdatedCommand { get; }

        public bool AsyncCanExecuteSecond { get; set; }
        // Manually update CanExecute for a command
        private async Task AsyncUpdateCanExecute()
        {
            // By default, command executed on separate thread, so we need a dispatcher to address it
            AsyncDelegateCanExecuteManuallySetManuallyUpdatedCommand.RaiseCanExecuteChanged();
        }

        public AsyncDelegateCommand AsyncDelegateCanExecuteManuallySetManuallyUpdatedCommand { get; }
        public AsyncDelegateCommand AsyncUpdateCanExecuteCommand { get; }

        public AsyncDelegateCommand<string> AsyncDelegateGenericCommand { get; }
        private async Task AsyncDelegateGeneric(string value)
        {
            Count += int.Parse(value);
        }

        public DelegateCommand DelegateExceptionCommand { get; }
        public DelegateCommand DelegateExceptionHandledCommand { get; }
        private void DelegateExceptionHandled()
        {
            throw new Exception("Some exception");
        }

        public AsyncDelegateCommand AsyncDelegateExceptionCommand { get; }
        public AsyncDelegateCommand AsyncDelegateExceptionHandledCommand { get; }
        private async Task AsyncDelegateExceptionHandled()
        {
            throw new Exception("Some exception");
        }
    }
}
