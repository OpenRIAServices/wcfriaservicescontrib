namespace EntityGraph.Validation
{
    /// <summary>
    /// Abstract base class for attribute validators.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public abstract class AttributeValidator<TResult> : ValidationRule<TResult>, IAttributeValidator
        where TResult : class
    {
        /// <summary>
        /// Initializes a new instance of the AttributeValidator class.
        /// </summary>
        /// <param name="signature"></param>
        protected AttributeValidator(params ValidationRuleDependency[] signature) : base(signature) { }
        /// <summary>
        /// Invokes this validator on the given object
        /// </summary>
        /// <param name="value"></param>
        public abstract void Validate(object value);
    }
}
