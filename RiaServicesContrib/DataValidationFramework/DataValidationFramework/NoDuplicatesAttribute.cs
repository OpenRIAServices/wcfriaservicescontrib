using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RiaServicesContrib.DataValidation;

namespace RiaServicesContrib.DomainServices.Client.DataValidation
{
    /// <summary>
    /// This validation attribute verifies that a collection does not contain duplicate elements
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NoDuplicatesAttribute : ValidationAttribute
    {
        /// <summary>
        /// Creates a new instance of the NoDuplicatesValidator class.
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        protected override ValidationRule<ValidationResult> Create(params ValidationRuleDependency[] signature)
        {
            var validator = new NoDuplicatesValidator(signature)
            {
                Message = String.Format(ErrorMessage, PropertyInfo.Name),
            };
            return validator;
        }
    }
    /// <summary>
    /// The actual implementation of the NoDuplicates validation.
    /// </summary>
    public class NoDuplicatesValidator : AttributeValidator
    {
        /// <summary>
        /// Gets or sets the error message of this validator.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Initializes a new instance of the NoDuplicatesValidator class using an array of ValidationRuleDependencies.
        /// </summary>
        /// <param name="signature"></param>
        public NoDuplicatesValidator(params ValidationRuleDependency[] signature)
            : base(signature)
        {
        }
        /// <summary>
        /// Initializes a new instance of the NoDuplicatesValidator class using a givenSignature.
        /// </summary>
        /// <param name="signature"></param>
        public NoDuplicatesValidator(Signature signature)
            : base(signature.ToArray())
        {
        }
        /// <summary>
        /// Validation method for the class.
        /// </summary>
        /// <param name="value"></param>
        public override void Validate(object value)
        {
            var collection = (IEnumerable)value;
            if(collection == null)
            {
                return;
            }
            List<object> list = new List<object>();
            foreach(var element in collection)
            {
                if(list.Contains(element))
                {
                    Result = new ValidationResult(Message);
                    return;
                }
                list.Add(element);
            }
            Result = ValidationResult.Success;
        }
    }
}

