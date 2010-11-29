using System.ComponentModel.DataAnnotations;
using RIA.EntityValidator;

namespace EntityGraph.EntityValidator.RIA
{
    public abstract class ValidationRule<TRoot> : ValidationRule<TRoot, ValidationResult>
    {
    }
}
