using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace RIA.EntityValidator
{
    [InheritedExport]
    public interface IValidationRulesProvider<TEntity, TResult> where TResult : class
    {
        IEnumerable<Tuple<Tuple<object, string>, IValidationRule<TEntity, TResult>>> GetValidators(TEntity root);
    }
}
