using System.ComponentModel.DataAnnotations;
using RiaServicesContrib.DataValidation;

namespace RiaServicesContrib.DomainServices.Client.DataValidation
{
    /// <summary>
    /// WCF DomainServices.Client services specific instantiation of the RiaServicesContrib.EntityGraph.Validation.ValidationRule class.
    /// </summary>
    public abstract class ValidationRule : ValidationRule<ValidationResult>
    {
        /// <summary>
        /// Initializes a new instance of the ValidationRule class.
        /// </summary>
        /// <param name="signature"></param>
        public ValidationRule(params ValidationRuleDependency[] signature)
            : base(signature)
        {
        }
    }
}
