using System;
using System.ComponentModel;

namespace RiaServicesContrib.DataValidation
{
    /// <summary>
    /// Class that that represents an asynchronous validation operation.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class ValidationOperation<TResult> : INotifyPropertyChanged
        where TResult : class
    {
        private TResult _result;
        /// <summary>
        /// Gets or sets the valiation result for this ValidationOperation.
        /// </summary>
        public TResult Result
        {
            get
            {
                return _result;
            }
            set
            {
                if(_result != value)
                {
                    _result = value;
                    if(PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Result"));
                    }
                    if(Completed != null)
                    {
                        Completed(this, new EventArgs());
                    }
                }
            }
        }
        /// <summary>
        /// Event raised when the validation operation completes.
        /// </summary>
        public event EventHandler Completed;
        /// <summary>
        /// Event raised whenever a ValidationOperation property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
