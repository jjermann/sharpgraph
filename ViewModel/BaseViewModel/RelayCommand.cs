using System;
using System.Diagnostics;
using System.Windows.Input;

namespace SharpGraph {
    public class RelayCommand : ICommand {
        private readonly Predicate<object> m_canExecute;
        private readonly Action<object> m_execute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null) {
            if (execute == null) {
                throw new ArgumentNullException(nameof(execute));
            }
            m_execute = execute;
            m_canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter) {
            return m_canExecute?.Invoke(parameter) ?? true;
        }

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter) {
            m_execute(parameter);
        }
    }
}