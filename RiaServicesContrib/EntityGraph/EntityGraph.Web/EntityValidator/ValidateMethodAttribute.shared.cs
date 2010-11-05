using System;

namespace RIA.EntityValidator
{
    /// <summary>
    /// Attribute that marks a method as a validation method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited=true)]
    public class ValidateMethodAttribute : Attribute
    {
        public ValidateMethodAttribute() { }
    }
}
