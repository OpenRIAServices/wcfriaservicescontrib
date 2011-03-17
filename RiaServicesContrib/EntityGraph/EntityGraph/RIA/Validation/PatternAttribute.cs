using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using RiaServicesContrib.Validation;

namespace RiaServicesContrib.DomainServices.Client.Validation
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
        protected override ValidationRule<ValidationResult> Create(params ValidationRuleDependency[] signature)
        {
            var validator = new PatternValidator(signature)
            {
                Message = String.Format(ErrorMessage, PropertyInfo.Name),
                Pattern = Pattern
            };
            return validator;
        }
    }
    /// <summary>
    /// The actual implementation of the PatternValidator validation.
    /// </summary>
    public class PatternValidator : AttributeValidator
    {
        /// <summary>
        /// Gets or sets the error message of this validator.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the pattern of the validator.
        /// </summary>
        public string Pattern
        {
            get
            {
                return _pattern;
            }
            set
            {
                if(_pattern != value)
                {
                    _pattern = value;
                    _regex = new Regex(value);
                }
            }
        }
        private string _pattern;

        private Regex _regex;

        /// <summary>
        /// Initializes a new instance of the PatternValidator class using an array of ValidationRuleDependencies.
        /// </summary>
        /// <param name="signature"></param>
        public PatternValidator(params ValidationRuleDependency[] signature)
            : base(signature)
        {
        }
        /// <summary>
        /// Initializes a new instance of the PatternValidator class using a givenSignature.
        /// </summary>
        /// <param name="signature"></param>
        public PatternValidator(Signature signature)
            : base(signature.ToArray())
        {
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
                    Match m = _regex.Match(input);
                    if(m.Length != input.Length)
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
    }
}
