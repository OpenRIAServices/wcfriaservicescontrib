using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace RiaServicesContrib.DataValidation
{
    /// <summary>
    /// Represents a collection of validation rules.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    [InheritedExport]
    public interface IValidationRulesProvider<TResult> where TResult : class
    {
        /// <summary>
        /// Gets the collection of Validation rules that are provided by this validation rules provider.
        /// </summary>
        IEnumerable<ValidationRule<TResult>> ValidationRules { get; }
    }
}
