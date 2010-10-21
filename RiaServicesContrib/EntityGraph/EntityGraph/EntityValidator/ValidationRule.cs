using System;

namespace RIA.EntityValidator
{
    public abstract class ValidationRule<TEntity, TResult> : IValidationRule<TEntity, TResult> where TResult : class
    {
        public event EventHandler<ValidationResultChangedEventArgs<TResult>> ValidationResultChanged;

        private TResult _result;
        public TResult Result {
            get { return _result; }
            protected set {
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
        public abstract ValidationRuleDependencies<TEntity> Signature { get; }

        /// <summary>
        /// Validates entity by invoking the validation method of this EntityValidationRule 
        /// </summary>
        /// <param name="entity"></param>
        public void InvokeValidate(TEntity entity) {
            ValidationRuleInvokeHelper<TEntity, TResult>.InvokeValidator(this, entity);
        }
    }
}
