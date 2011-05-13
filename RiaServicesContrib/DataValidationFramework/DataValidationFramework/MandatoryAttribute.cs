using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RiaServicesContrib;
using RiaServicesContrib.DataValidation;

namespace RiaServicesContrib.DomainServices.Client.DataValidation
{
    /// <summary>
    /// This class is similar (but simpler) to System.ComponentModel.DataAnnotations.RequiredAttribute.
    /// Its purpose is to demonstrate that the validation mechanism in DomainServices.Client.EntityValidator
    /// can support similar property-level validation as with the DomainServices.Client validation mechanism.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class MandatoryAttribute : ValidationAttribute
    {
        /// <summary>
        /// Creates a new instance of the MandatoryValidator class.
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        protected override ValidationRule<ValidationResult> Create(params ValidationRuleDependency[] signature)
        {
            var validator = new MandatoryValidator(signature)
            {
                Message = String.Format(ErrorMessage, PropertyInfo.Name)
            };
            return validator;
        }
    }
    /// <summary>
    /// The actual implementation of the Mandatory validation.
    /// </summary>
    public class MandatoryValidator : AttributeValidator
    {
        /// <summary>
        /// Gets or sets the error message of this validator.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Initializes a new instance of the MandatoryValidator class using an array of ValidationRuleDependencies.
        /// </summary>
        /// <param name="signature"></param>
        public MandatoryValidator(params ValidationRuleDependency[] signature)
            : base(signature)
        {
        }
        /// <summary>
        /// Initializes a new instance of the MandatoryValidator class using a givenSignature.
        /// </summary>
        /// <param name="signature"></param>
        public MandatoryValidator(Signature signature)
            : base(signature.ToArray())
        {
        }
        /// <summary>
        /// The validation method.
        /// </summary>
        /// <param name="value"></param>
        public override void Validate(object value)
        {
            if(value == null)
            {
                Result = new ValidationResult(Message);
            }
            else
            {
                Result = ValidationResult.Success;
            }
        }
    }
}
