using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RIA.EntityValidator
{
    public class ClassValidationRulesProvider<TEntity, TResult> :
        IValidationRulesProvider<TEntity, TResult> where TResult : class
    {
        Dictionary<Tuple<object, string>, List<IValidationRule<TEntity, TResult>>> IValidationRulesProvider<TEntity, TResult>.GetValidationRules(TEntity root)
        {
            var rules = new Dictionary<Tuple<object, string>, List<IValidationRule<TEntity, TResult>>>();
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
            foreach(var rule in validatorEntries)
            {
                if(rules.ContainsKey(rule.Item1))
                {
                    rules[rule.Item1].Add(rule.Item2);
                }
                else
                {
                    rules.Add(rule.Item1, new List<IValidationRule<TEntity, TResult>> { rule.Item2 });
                }
            }
            return rules;
        }
    }
}
