using System;
using System.ComponentModel.Composition;

namespace RIA.EntityValidator
{
    [InheritedExport]
    public abstract class ValidationRule<TRoot, TResult> : IValidationRule<TRoot, TResult> where TResult : class
    {
        public event EventHandler<ValidationResultChangedEventArgs<TResult>> ValidationResultChanged;

        private TResult _result;
        public TResult Result
        {
            get { return _result; }
            protected set
            {
                if(_result != value)
                {
                    TResult old = _result;
                    _result = value;
                    if(ValidationResultChanged != null)
                    {
                        ValidationResultChanged(this, new ValidationResultChangedEventArgs<TResult>(old, value));
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of mappings from T to members reachable from T that play a role in the validation rule
        /// </summary>
        public abstract ValidationRuleDependencies<TRoot> Signature { get; }

        /// <summary>
        /// Validates entity by invoking the validation method of this EntityValidationRule 
        /// </summary>
        /// <param name="entity"></param>
        public void InvokeValidate(TRoot entity)
        {
            ValidationRuleInvokeHelper<TRoot, TResult>.InvokeValidator(this, entity);
        }
    }
}
