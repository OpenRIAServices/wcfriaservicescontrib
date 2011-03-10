using System.ComponentModel.DataAnnotations;
using EntityGraph.Validation;

namespace EntityGraph.RIA.Validation
{
    /// <summary>
    /// Abstract base class for attribute validators.
    /// This is a WCF RIA services-specific instantation of the 
    /// EntityGraph.Validation.AttributeValidator class.
    /// </summary>
    public abstract class AttributeValidator : AttributeValidator<ValidationResult>
    {
        /// <summary>
        /// Initializes a new instance of the AttributeValidator class.
        /// </summary>
        /// <param name="signature"></param>
        protected AttributeValidator(params ValidationRuleDependency[] signature) : base(signature) { }
    }
}
