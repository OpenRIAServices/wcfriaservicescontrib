using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using EntityGraph.Validation;

namespace EntityGraph.RIA.Validation
{
    /// <summary>
    /// This class is similar to System.ComponentModel.DataAnnotations.RegularExpressionAttribute.
    /// This attribute checks if a property value matches the provided regular expression .
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PatternAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the PatternAttribute class.
        /// </summary>
        /// <param name="pattern"></param>
        public PatternAttribute(string pattern)
        {
            this.Pattern = pattern;
        }
        /// <summary>
        /// Gets the regular expression pattern of this RegularExpressionAttribute instance.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Creates a new instance of the PatternValidator class.
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        protected override EntityGraph.Validation.ValidationRule<ValidationResult> Create(EntityGraph.Validation.Signature signature)
        {
            return new PatternValidator(this, signature);
        }
    }
    /// <summary>
    /// The actual implementation of the PatternValidator validation.
    /// </summary>
    public class PatternValidator : AttributeValidator
    {
        private Regex Regex;

        private PatternAttribute attribute;
        /// <summary>
        /// Initializes a new instance of the PatternValidator class.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="signature"></param>
        public PatternValidator(PatternAttribute attribute, Signature signature)
            : base(signature)
        {
            this.attribute = attribute;
            Regex = new Regex(attribute.Pattern);

        }
        /// <summary>
        /// The validation method.
        /// </summary>
        /// <param name="value"></param>
        public override void Validate(object value)
        {
            {
                if(value is string)
                {
                    string input = (string)value;
                    Match m = Regex.Match(input);
                    if(m.Length != input.Length)
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
    }
}
