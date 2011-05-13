
namespace RiaServicesContrib.DataValidation
{
    /// <summary>
    /// Class that represents a collection of dependency bindings for a validation rule.
    /// It holds references to all bindings for its validation rule dependencies,
    /// and a reference to the validation rule itself.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    internal class RuleBinding<TResult> where TResult : class
    {
        /// <summary>
        /// Gets or sets the collection of validation rule dependency bindings for this rule binding.
        /// </summary>
        public ValidationRuleDependencyBinding[] DependencyBindings { get; set; }
        /// <summary>
        /// Gets or sets the validation rule for this validation rule binding.
        /// </summary>
        public ValidationRule<TResult> ValidationRule { get; set; }
    }
}
