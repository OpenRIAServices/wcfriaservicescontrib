using System;
using System.Collections.Generic;

namespace RIA.EntityValidator
{
    public class ValidationRulesChangedEventArgs : EventArgs
    {
    }
    public interface IValidationRulesProvider<TEntity, TResult> where TResult : class
    {
        event EventHandler<ValidationRulesChangedEventArgs> ValidationRulesChanged;
        IEnumerable<Tuple<Tuple<object, string>, IValidationRule<TEntity, TResult>>> GetValidationRules(TEntity root);
    }
}
