using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace EntityGraph.Validation
{
    public class ValidationEngine<TEntity, TValidationResult> : IDisposable
        where TValidationResult : class
    {
        public event EventHandler<ValidationResultChangedEventArgs<TValidationResult>> ValidationResultChanged;

        private IEnumerable<ValidationRule<TValidationResult>> GetRulesByPropertyName(string propertyName)
        {
            return from rule in ValidationRules
                   where rule.Signature.Any(dep => dep.TargetProperty.Name == propertyName)
                   select rule;
        }
        private IEnumerable<ValidationRule<TValidationResult>> FilterByObjectType(IEnumerable<ValidationRule<TValidationResult>> rules, object obj)
        {
            var type = obj.GetType();
            return from rule in rules
                   where rule.Signature.Any(dep => dep.TargetPropertyOwnerType == type)
                   select rule;
        }

        private IValidationRulesProvider<TValidationResult> rulesProvider;
        public ValidationEngine(IValidationRulesProvider<TValidationResult> rulesProvider)
        {
            this.rulesProvider = rulesProvider;
            ValidationRules = rulesProvider.ValidationRules;

            foreach(var rule in ValidationRules)
            {
                rule.ValidationResultChanged += ValidationResultChangedCallback;
            }
        }

        public IEnumerable<ValidationRule<TValidationResult>> ValidationRules { get; private set; }

        /// <summary>
        /// Given a validation rule with dependency expressions and a collection of objects
        /// return a list of tuples (represented as lists) with all permutations of matching objects.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        private static IEnumerable<IEnumerable<ParameterObjectBinding>> GetRuleArgumentObjectBindings(ValidationRule<TValidationResult> rule, IEnumerable<TEntity> objects)
        {
            List<IEnumerable<ParameterObjectBinding>> bindings = new List<IEnumerable<ParameterObjectBinding>>();

            var dependencyParameters = rule.GetValidationRuleDependencyParameters();

            var result = (from obj in objects
                          from parameter in dependencyParameters
                          where parameter.ParameterObjectType.IsAssignableFrom(obj.GetType())
                          select new ParameterObjectBinding { ParameterName = parameter.ParameterName, ParameterObjectType = parameter.ParameterObjectType, ParameterObject = obj })
                          .Distinct()
                          .GroupBy(x => x.ParameterName);
            foreach(var group in result.ToList())
            {
                bindings.Add(group.Select(x => x).ToList());
            }

            return GetPermutations(bindings);
        }
        private static IEnumerable<RuleBinding<TValidationResult>> GetRuleBindings(ValidationRule<TValidationResult> rule, IEnumerable<IEnumerable<ParameterObjectBinding>> objectBindings)
        {
            var result = new List<RuleBinding<TValidationResult>>();
            foreach(var objectBinding in objectBindings)
            {
                var bindings = (from dependency in rule.Signature
                                select new ValidationRuleDependencyBinding
                                {
                                    ValidationRuleDependency = dependency,
                                    ParameterObjectBinding = objectBinding.Single(b => b.ParameterName == dependency.ParameterExpression.Name)
                                }).ToList();
                result.Add(new RuleBinding<TValidationResult>
                {
                    DependencyBindings = bindings.ToArray(),
                    ValidationRule = rule
                });
            }
            return result;
        }
        private static List<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> head, List<IEnumerable<T>> tail)
        {
            if(tail.Count() == 0)
            {
                var newList = new List<IEnumerable<T>>();
                foreach(var e in head)
                {
                    newList.Add(new List<T> { e });
                }
                return newList;
            }
            var newhead = tail.First();
            var result = GetPermutations(newhead, tail.Skip(1).ToList());
            var list = new List<IEnumerable<T>>();
            foreach(var e in head)
            {
                foreach(var le in result)
                {
                    var tmp = le.ToList();
                    tmp.Add(e);
                    list.Add(tmp);
                }
            }
            return list;
        }
        private static List<IEnumerable<T>> GetPermutations<T>(List<IEnumerable<T>> list)
        {
            if(list.Count() == 0)
            {
                return list;
            }
            return GetPermutations(list.First(), list.Skip(1).ToList());
        }
        /// <summary>
        /// Filter rules for which the collections of dependency bindings does not contain obj
        /// and rules for which any TargetOwnerObject of an   dependency binings is null.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static IEnumerable<RuleBinding<TValidationResult>> FilterRuleBindings(IEnumerable<RuleBinding<TValidationResult>> bindings, object obj)
        {
            return (from binding in bindings
                    where
                        binding.DependencyBindings.All(b => b.TargetOwnerObject != null)
                        &&
                        (obj == null || binding.DependencyBindings.Any(b => b.TargetOwnerObject == obj))
                    select binding).ToList();
        }
        public void Validate(object obj, string propertyName, IEnumerable<TEntity> objects)
        {
            var type = obj.GetType();
            var rules = from rule in GetRulesByPropertyName(propertyName)
                        where rule.Signature.Any(dep => dep.TargetPropertyOwnerType == type)
                        select rule;
            ValidateRules(rules, objects, obj);
        }
        private void ValidateRules(IEnumerable<ValidationRule<TValidationResult>> rules, IEnumerable<TEntity> objects, object obj)
        {
            foreach(var rule in rules)
            {
                var objectBindings = GetRuleArgumentObjectBindings(rule, objects);
                var ruleBindings = GetRuleBindings(rule, objectBindings);
                var filteredBindings = FilterRuleBindings(ruleBindings, obj);
                foreach(var binding in filteredBindings)
                {
                    rule.Evaluate(binding);
                }
            }
        }
        internal void ValidateAll(IEnumerable<TEntity> objects)
        {
            ValidateRules(ValidationRules, objects, null);
        }
        public void Dispose()
        {
            foreach(var rule in ValidationRules)
            {
                rule.ValidationResultChanged -= ValidationResultChangedCallback;
            }
        }

        void ValidationResultChangedCallback(object sender, ValidationResultChangedEventArgs<TValidationResult> e)
        {
            if(ValidationResultChanged != null)
            {
                ValidationResultChanged(sender, e);
            }
        }
        internal IEnumerable<object> ObjectsInvolved(ValidationRule<TValidationResult> rule)
        {
            return rule.RuleBinding.DependencyBindings.Select(binding => binding.TargetOwnerObject).Distinct().ToList();
        }
    }
}
