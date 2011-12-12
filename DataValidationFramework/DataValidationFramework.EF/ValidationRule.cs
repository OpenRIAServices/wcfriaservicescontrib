using System.Data.Entity.Validation;
using RiaServicesContrib.DataValidation;

namespace DataValidationFramework.EF
{
    public abstract class ValidationRule : ValidationRule<DbValidationError>
    {
        protected ValidationRule(params ValidationRuleDependency[] signature)
            : base(signature)
        {
        }
    }
}
