using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntityGraph.Validation;

namespace EntityGraph.RIA.Validation
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
        protected override EntityGraph.Validation.ValidationRule<ValidationResult> Create(EntityGraph.Validation.Signature signature)
        {
            return new NoDuplicatesValidator(this, signature);
        }
    }
    /// <summary>
    /// The actual implementation of the NoDuplicates validation.
    /// </summary>
    public class NoDuplicatesValidator : AttributeValidator
    {
        NoDuplicatesAttribute attribute;
        /// <summary>
        /// Initializes a new instance of the NoDuplicatesValidator class.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="signature"></param>
        public NoDuplicatesValidator(NoDuplicatesAttribute attribute, Signature signature)
            : base(signature)
        {
            this.attribute = attribute;
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
                    string msg = String.Format(attribute.ErrorMessage, attribute.PropertyInfo.Name);
                    Result = new ValidationResult(msg);
                    return;
                }
                list.Add(element);
            }
            Result = ValidationResult.Success;
        }
    }
}

