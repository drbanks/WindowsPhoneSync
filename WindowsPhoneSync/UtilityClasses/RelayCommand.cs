using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WindowsPhoneSync
{
    /// <summary>
    /// Bog standard Josh Smith RelayCommand implementation
    /// </summary>
    public class RelayCommand : ICommand
    { 
        #region Fields 

        /// <summary>
        /// The execute action
        /// </summary>
        readonly Action<object> execute;
 
        /// <summary>
        /// The can execute predicate
        /// </summary>
        readonly Predicate<object> canExecute;
        
        #endregion 
        
        #region Constructors
        
        /// <summary>
        /// Constructor, execute handler only
        /// </summary>
        /// <param name="execute"></param>
        public RelayCommand(Action<object> execute) : this(execute, null) { } 
        
        /// <summary>
        /// Main constructor, supplying both execute handler and can execute handler
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute) 
        {
            if (execute == null) 
                throw new ArgumentNullException("execute");
            this.execute = execute;
            this.canExecute = canExecute; 
        } 
        
        #endregion 
        
        #region ICommand Members
        
        /// <summary>
        /// Returns true if the command can be executed.  Calls the CanExecute handler if there
        /// is one.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [DebuggerStepThrough] 
        public bool CanExecute(object parameter) 
        { 
            return canExecute == null ? true : canExecute(parameter); 
        }
        
        /// <summary>
        /// Raised when the value of CanExecute has potentially changed
        /// </summary>
        public event EventHandler CanExecuteChanged 
        { 
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; } 
        } 
        
        /// <summary>
        /// Main method to execute the command handler.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) 
        {
            execute(parameter);
        } 
        
        #endregion 
    }
}
