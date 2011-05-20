using System.ComponentModel.DataAnnotations;
using RiaServicesContrib.DataValidation;

namespace RiaServicesContrib.DomainServices.Client.DataValidation
{
    /// <summary>
    /// Class that forms the abstract base for all asynchronous cross-entity validation rules.
    /// This is a WCF DomainServices.Client services specific instantiation of the 
    /// RiaServicesContrib.DataValidation.AsyncValidationRule class.
    /// </summary>
    public class AsyncValidationRule : AsyncValidationRule<ValidationResult>
    {
        /// <summary>
        /// Initializes a new instance of the ValidationRule class.
        /// </summary>
        /// <param name="signature"></param>
        public AsyncValidationRule(params ValidationRuleDependency[] signature)
            : base(signature)
        {
        }
    }
}
