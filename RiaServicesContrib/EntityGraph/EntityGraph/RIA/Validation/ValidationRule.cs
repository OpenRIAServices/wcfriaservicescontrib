using System.ComponentModel.DataAnnotations;
using EntityGraph.Validation;

namespace EntityGraph.RIA.EntityValidator
{
    public abstract class ValidationRule : ValidationRule<ValidationResult>
    {
        public ValidationRule(Signature signature) :
            base(signature)
        {
        }
    }
}
