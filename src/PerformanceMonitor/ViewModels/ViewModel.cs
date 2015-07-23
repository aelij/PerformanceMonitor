using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PerformanceMonitor.Annotations;

namespace PerformanceMonitor.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotifyPropertyChangedInvocator]
        protected void RaisePropertyChangedByCallerName([CallerMemberName] string propertyName = null)
        {
            RaisePropertyChanged(propertyName);
        }

        protected bool RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                RaisePropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            RaisePropertyChanged(((MemberExpression)expression.Body).Member.Name);
        }

        protected static ICommandEx CreateCommand(Action execute, Func<bool> canExecute = null)
        {
            return new Command(o => execute(), canExecute != null ? o => canExecute() : (Func<object, bool>)null);
        }

        protected static ICommandEx CreateCommand<T>(Action<T> execute, Func<T, bool> canExecute = null)
        {
            return new Command(o => execute((T)o), canExecute != null ? o => canExecute((T)o) : (Func<object, bool>)null);
        }

        private class Command : ICommandEx
        {
            private readonly Action<object> _execute;
            private readonly Func<object, bool> _canExecute;

            public Command(Action<object> execute, Func<object, bool> canExecute)
            {
                if (execute == null) throw new ArgumentNullException(nameof(execute));
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute(parameter);
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }

            public event EventHandler CanExecuteChanged;

            public void RaiseCanExecuteChanged()
            {
                var handler = CanExecuteChanged;
                handler?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public interface ICommandEx : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
