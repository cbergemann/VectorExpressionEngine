using System;
using System.Windows.Input;

namespace SimpleCalculator.Helper
{
    public class RelayCommand : ICommand
    {
        Action<object> _executeMethod;
        Func<object, bool> _canexecuteMethod;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object> executeMethod, Func<object, bool> canexecuteMethod = null)
        {
            _executeMethod = executeMethod;
            _canexecuteMethod = canexecuteMethod;
        }

        public bool CanExecute(object parameter) => _canexecuteMethod == null || _canexecuteMethod(parameter);

        public void Execute(object parameter) => _executeMethod(parameter);
    }
}
