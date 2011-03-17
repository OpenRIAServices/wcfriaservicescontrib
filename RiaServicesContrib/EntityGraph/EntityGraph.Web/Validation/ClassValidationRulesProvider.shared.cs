using System;
using System.Collections.Generic;
using System.Linq;

namespace RiaServicesContrib.Validation
{
    /// <summary>
    /// Class that provides a collection of validation rules that are annotated
    /// to the properties of a class using ValidationAttribute attributes.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class ClassValidationRulesProvider<TResult> : IValidationRulesProvider<TResult>
        where TResult : class
    {
        /// <summary>
        /// Gets the type this validation rules provider operates on.
        /// </summary>
        public Type Type { get; private set; }
        /// <summary>
        /// Initializes a new instance of the ClassValidationRulesProvider class.
        /// </summary>
        /// <param name="type"></param>
        public ClassValidationRulesProvider(Type type)
        {
            this.Type = type;
        }
        private IEnumerable<ValidationRule<TResult>> _validationRules;
        /// <summary>
        /// <summary>
        /// Gets the collection of Validation rules that are provided by this validation rules provider.
        /// </summary>
        /// </summary>
        public IEnumerable<ValidationRule<TResult>> ValidationRules
        {
            get
            {
                if(_validationRules == null)
                {
                    _validationRules = (
                    from prop in Type.GetProperties()
                    from attr in prop.GetCustomAttributes(true).OfType<IValidationAttribute<TResult>>()
                    select attr.BindTo(prop)).ToList();
                }

                return _validationRules;
            }
        }
    }
}
