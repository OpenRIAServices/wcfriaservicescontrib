using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace RIA.EntityValidator.RIA
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RegularExpressionAttribute : ValidationAttribute
    {
        private Regex Regex;
        private string _pattern;
        public string Pattern
        {
            get
            {
                return _pattern;
            }
            private set
            {
                if(_pattern != value)
                {
                    _pattern = value;
                    Regex = new Regex(_pattern);
                }
            }
        }

        public RegularExpressionAttribute(string pattern)
        {
            this.Pattern = pattern;
        }

        [ValidateMethod]
        public void Validate(object value)
        {
            if(value is string)
            {
                string input = (string)value;
                Match m = Regex.Match(input);
                if(m.Length != input.Length)
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
}
