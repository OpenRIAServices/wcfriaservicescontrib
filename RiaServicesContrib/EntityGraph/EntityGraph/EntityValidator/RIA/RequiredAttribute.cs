using System;
using System.ComponentModel.DataAnnotations;

namespace RIA.EntityValidator.RIA
{
    /// <summary>
    /// This class is similar (but simpler) to System.ComponentModel.DataAnnotations.RequiredAttribute.
    /// Its purpose is to demonstrate that the validation mechanism in RIA.EntityValidator
    /// can support similar property-level validation as with the RIA validation mechanism.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class RequiredAttribute : ValidationAttribute
    {
        [ValidateMethod]
        public void Validate(object value)
        {
            if(value == null)
            {
                string msg = String.Format(ErrorMessage, PropertyInfo.Name);
                Result = new ValidationResult(msg, new string[] { PropertyInfo.Name });
            }
            else
            {
                Result = ValidationResult.Success;
            }
        }
    }
}
