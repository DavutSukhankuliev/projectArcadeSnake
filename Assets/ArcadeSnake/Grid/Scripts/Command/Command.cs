using System;
using System.Windows.Input;

namespace ArcadeSnake
{
    public abstract class Command : ICommand
    {
        public abstract void Execute();
        public event EventHandler DoneEvent;

        protected virtual void OnDone()
        {
            DoneEvent?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler CanExecuteChanged;
    }
}
