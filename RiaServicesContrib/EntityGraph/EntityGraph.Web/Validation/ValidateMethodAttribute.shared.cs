using System;

namespace EntityGraph.Validation
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
