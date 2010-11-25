using System;
using System.Collections.Generic;

namespace RIA.EntityValidator
{
    public interface IValidationRulesProvider<TRoot, TEntity, TResult> where TResult : class
    {
        Dictionary<Tuple<TEntity, string>, List<IValidationRule<TRoot,TResult>>> GetValidationRules(TRoot root);
    }
}
