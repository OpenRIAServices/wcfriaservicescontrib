
namespace EntityGraph.Validation
{
    /// <summary>
    /// Class that represents a collection of dependency bindings for a validation rule
    /// </summary>
    /// <typeparam name="TValidationResult"></typeparam>
    internal class RuleBinding<TValidationResult> where TValidationResult : class
    {
        public ValidationRuleDependencyBinding[] DependencyBindings { get; set; }
        public ValidationRule<TValidationResult> ValidationRule { get; set; }
    }
}
