using System.Collections.Generic;

namespace EntityGraph.Validation
{
    /// <summary>
    /// Class that implements a validation rule provider as a list of validation rules.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class SimpleValidationRulesProvider<TResult> : List<ValidationRule<TResult>>, IValidationRulesProvider<TResult>
        where TResult : class
    {
        /// <summary>
        /// Gets the collection of Validation rules that are provided by this validation rules provider.
        /// </summary>
        public IEnumerable<ValidationRule<TResult>> ValidationRules
        {
            get { return this; }
        }
    }
}
