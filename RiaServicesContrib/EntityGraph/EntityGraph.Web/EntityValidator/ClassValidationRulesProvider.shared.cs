using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RIA.EntityValidator
{
    public class ClassValidationRulesProvider<TRoot, TEntity, TResult> :
        IValidationRulesProvider<TRoot, TEntity, TResult>
        where TRoot : TEntity
        where TEntity : class
        where TResult : class
    {
        Dictionary<Tuple<TEntity, string>, List<IValidationRule<TRoot,TResult>>> IValidationRulesProvider<TRoot, TEntity, TResult>.GetValidationRules(TRoot root)
        {
            var rules = new Dictionary<Tuple<TEntity, string>, List<IValidationRule<TRoot,TResult>>>();
            var type = root.GetType();

            // Sets the propertyinfo of the ValidationAttribute, such that the validation attribute knows
            // about the property it is attached to.
            Func<PropertyInfo, IValidationRule<TRoot, TResult>, IValidationRule<TRoot,TResult>> SetPropInfo =
                (pInfo, rule) => {
                    ((IValidationAttribute<TRoot,TResult>)rule).SetPropertyInfo(pInfo); return rule; 
                };
            var validatorEntries = from property in type.GetProperties()
                                   from validator in property.GetCustomAttributes(typeof(IValidationAttribute<TRoot,TResult>), true)
                                        .Cast<IValidationRule<TRoot,TResult>>()
                                   select
                                    new Tuple<Tuple<TEntity, string>, IValidationRule<TRoot,TResult>>(
                                        new Tuple<TEntity, string>(root, property.Name), SetPropInfo(property, validator));
            foreach(var rule in validatorEntries)
            {
                if(rules.ContainsKey(rule.Item1))
                {
                    rules[rule.Item1].Add(rule.Item2);
                }
                else
                {
                    rules.Add(rule.Item1, new List<IValidationRule<TRoot,TResult>> { rule.Item2 });
                }
            }
            return rules;
        }
    }
}
