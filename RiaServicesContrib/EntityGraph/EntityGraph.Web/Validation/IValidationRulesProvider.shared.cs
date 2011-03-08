using System.Collections.Generic;

namespace EntityGraph.Validation
{
    public interface IValidationRulesProvider<TResult> where TResult : class
    {
        IEnumerable<ValidationRule<TResult>> ValidationRules { get; }
    }
}
