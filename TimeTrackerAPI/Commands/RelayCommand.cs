﻿using System;
using System.Windows.Input;

namespace Ficksworkshop.TimeTracker.Commands
{
    public class RelayCommand : ICommand
    {
        #region Fields
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        
        #endregion
        
        #region Constructors
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }
        
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            
            _execute = execute;
            _canExecute = canExecute;
        }
        
        #endregion
        
        #region ICommand Members
        
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        
        #endregion
    }
}
