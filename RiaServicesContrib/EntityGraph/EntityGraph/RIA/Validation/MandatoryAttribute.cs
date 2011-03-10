using System;
using EntityGraph.RIA.Validation;
using EntityGraph.Validation;
using System.ComponentModel.DataAnnotations;

namespace EntityGraph.RIA.Validation
{
    /// <summary>
    /// This class is similar (but simpler) to System.ComponentModel.DataAnnotations.RequiredAttribute.
    /// Its purpose is to demonstrate that the validation mechanism in RIA.EntityValidator
    /// can support similar property-level validation as with the RIA validation mechanism.
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
            return new MandatoryValidator(this, signature);
        }
    }
    /// <summary>
    /// The actual implementation of the Mandatory validation.
    /// </summary>
    public class MandatoryValidator : AttributeValidator
    {
        private MandatoryAttribute attribute;
        /// <summary>
        /// Initializes a new instance of the MandatoryValidator class.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="signature"></param>
        public MandatoryValidator(MandatoryAttribute attribute, params ValidationRuleDependency[] signature)
            : base(signature)
        {
            this.attribute = attribute;
        }
        /// <summary>
        /// The validation method.
        /// </summary>
        /// <param name="value"></param>
        public override void Validate(object value)
        {
            if(value == null)
            {
                string msg = String.Format(attribute.ErrorMessage, attribute.PropertyInfo.Name);
                Result = new ValidationResult(msg);
            }
            else
            {
                Result = ValidationResult.Success;
            }
        }
    }
}
