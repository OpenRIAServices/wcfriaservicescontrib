using System;
using System.Collections.Generic;

namespace RIA.EntityValidator
{
    public interface IValidationRulesProvider<TEntity, TResult> where TResult : class
    {
        IEnumerable<Tuple<Tuple<object, string>, IValidationRule<TEntity, TResult>>> GetValidationRules(TEntity root);
    }
}
