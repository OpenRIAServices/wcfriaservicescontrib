using System;

namespace RIA.EntityValidator
{
    public class ValidationResultChangedEventArgs<TResult> : EventArgs
    {
        public TResult OldResult { get; private set; }
        public TResult Result { get; private set; }

        public ValidationResultChangedEventArgs(TResult old, TResult result) {
            this.OldResult = old;
            this.Result = result;
        }
    }
}
