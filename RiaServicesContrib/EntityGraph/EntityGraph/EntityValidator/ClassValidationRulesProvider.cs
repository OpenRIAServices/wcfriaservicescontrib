using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RIA.EntityValidator
{
    public class ClassValidationRulesProvider<TEntity, TResult> :
        IValidationRulesProvider<TEntity, TResult> where TResult : class
    {
        public event EventHandler<ValidationRulesChangedEventArgs> ValidationRulesChanged;

        public IEnumerable<Tuple<Tuple<object, string>, IValidationRule<TEntity, TResult>>> GetValidationRules(TEntity root)
        {
            var type = root.GetType();

            // Sets the propertyinfo of the ValidationAttribute, such that the validation attribute knwos
            // about the property it is attached to.
            Func<PropertyInfo, IValidationRule<TEntity, TResult>, IValidationRule<TEntity, TResult>> SetPropInfo =
                (pInfo, rule) => { 
                    ((IValidationAttribute<TEntity, TResult>)rule).SetPropertyInfo(pInfo); return rule; 
                };
            var validatorEntries = from property in type.GetProperties()
                                   from validator in property.GetCustomAttributes(typeof(IValidationAttribute<TEntity, TResult>), true)
                                        .Cast<IValidationRule<TEntity, TResult>>()
                                   select
                                    new Tuple<Tuple<object, string>, IValidationRule<TEntity, TResult>>(
                                        new Tuple<object, string>(root, property.Name), SetPropInfo(property, validator));
            return validatorEntries.ToList();
        }
    }
}
