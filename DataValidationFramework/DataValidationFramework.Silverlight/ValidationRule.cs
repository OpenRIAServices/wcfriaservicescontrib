using System.ComponentModel.DataAnnotations;
using RiaServicesContrib.DataValidation;

namespace RiaServicesContrib.DomainServices.Client.DataValidation
{
    /// <summary>
    /// Class that forms the abstract base for all cross-entity validation rules.
    /// This is a WCF DomainServices.Client services specific instantiation of the 
    /// RiaServicesContrib.DataValidation.ValidationRule class.
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
