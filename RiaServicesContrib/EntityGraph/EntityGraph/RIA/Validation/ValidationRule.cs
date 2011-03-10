using System.ComponentModel.DataAnnotations;
using EntityGraph.Validation;

namespace EntityGraph.RIA.Validation
{
    /// <summary>
    /// WCF RIA services specific instantiation of the EntityGraph.Validation.ValidationRule class.
    /// </summary>
    public abstract class ValidationRule : ValidationRule<ValidationResult>
    {
        /// <summary>
        /// Initializes a new instance of the ValidationRule class.
        /// </summary>
        /// <param name="signature"></param>
        public ValidationRule(Signature signature) :
            base(signature)
        {
        }
    }
}
