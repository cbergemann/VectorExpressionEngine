using System;
using System.Windows.Input;

namespace SimpleCalculator.Helper
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _executeMethod;
        private readonly Func<object, bool> _canExecuteMethod;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod = null)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        public bool CanExecute(object parameter) => _canExecuteMethod == null || _canExecuteMethod(parameter);

        public void Execute(object parameter) => _executeMethod(parameter);
    }
}
