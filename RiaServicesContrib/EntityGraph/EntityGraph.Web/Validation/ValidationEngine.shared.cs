using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityGraph.Validation
{
    /// <summary>
    /// Class that implements cross-entity validation.
    /// 
    /// There are two alternative ways fro cross-entity validation
    /// - Validation for all objects and validation rules. In this variant, the engine computes all possible
    ///   bindings for a given set of entities of type TEntity and a collection of validation rules provided by 
    ///   the given IValidationRulesProvider. For each binding it invokes the corresponding validation rule.
    /// - Validation given an object and a property name. In this variant, the permutations of entity bindings 
    ///   are restricted to include the given object. The set of validation rules is restricted to those rules
    ///   that have the given property in their signature.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class ValidationEngine<TEntity, TResult> : IDisposable
        where TResult : class
    {
        /// <summary>
        /// Initializes a new instance of the ValidationEngine class.
        /// </summary>
        /// <param name="rulesProvider"></param>
        public ValidationEngine(IValidationRulesProvider<TResult> rulesProvider)
        {
            this.rulesProvider = rulesProvider;
            ValidationRules = rulesProvider.ValidationRules;

            foreach(var rule in ValidationRules)
            {
                rule.ResultChanged += ValidationResultChangedCallback;
            }
        }
        /// <summary>
        /// Method that invokes all matching validation rules for the given object and 
        /// property name.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        public void Validate(TEntity obj, string propertyName)
        {
            Validate(obj, propertyName, new List<TEntity> { obj });
        }
        /// <summary>
        /// Method that invokes all matching validation rules for all possible bindings given
        /// a collection of objects, an object 'obj' that should be presentin any bindings, and a 
        /// (changed) property with name 'propertyName' that should be part in any signature.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="objects"></param>
        public void Validate(object obj, string propertyName, IEnumerable<TEntity> objects)
        {
            var type = obj.GetType();
            var rules = from rule in GetRulesByPropertyName(propertyName)
                        where rule.Signature.Any(dep => dep.TargetPropertyOwnerType.IsAssignableFrom(type))
                        select rule;
            ValidateRules(rules, objects, obj);
        }
        /// <summary>
        /// Method that invokes all matching validation rules for all possible bindings given a 
        /// collection of validation rules.
        /// </summary>
        /// <param name="objects"></param>
        public void ValidateAll(IEnumerable<TEntity> objects)
        {
            ValidateRules(ValidationRules, objects, null);
        }
        /// <summary>
        /// Frees allocated resources for this validation engine.
        /// </summary>
        public void Dispose()
        {
            foreach(var rule in ValidationRules)
            {
                rule.ResultChanged -= ValidationResultChangedCallback;
            }
        }
        /// <summary>
        /// Event handler that is called when the parameterObjectBindings of any of the validation rules changes.
        /// </summary>
        internal event EventHandler<ValidationResultChangedEventArgs<TResult>> ValidationResultChanged;
        /// <summary>
        /// Returns a collection of validation rules that have 'propertyName' as one of the target properties in
        /// any of their validation rule dependencies.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private IEnumerable<ValidationRule<TResult>> GetRulesByPropertyName(string propertyName)
        {
            return from rule in ValidationRules
                   where rule.Signature.Any(dep => dep.TargetProperty.Name == propertyName)
                   select rule;
        }

        private IValidationRulesProvider<TResult> rulesProvider;

        /// <summary>
        /// Gets or sets the collection of validation rules for this validation engine.
        /// </summary>
        private IEnumerable<ValidationRule<TResult>> ValidationRules { get; set; }

        /// <summary>
        /// Given a validation rule with dependency expressions and a collection of objects
        /// return a list of tuples (represented as lists) with all permutations of matching objects.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        private static IEnumerable<IEnumerable<ParameterObjectBinding>> GetRuleArgumentObjectBindings(ValidationRule<TResult> rule, IEnumerable<TEntity> objects)
        {
            List<IEnumerable<ParameterObjectBinding>> bindings = new List<IEnumerable<ParameterObjectBinding>>();

            var dependencyParameters = rule.GetValidationRuleDependencyParameters();

            var parameterObjectBindings =
                (from obj in objects
                 from parameter in dependencyParameters
                 where parameter.ParameterObjectType.IsAssignableFrom(obj.GetType())
                 select new ParameterObjectBinding { ParameterName = parameter.ParameterName, ParameterObjectType = parameter.ParameterObjectType, ParameterObject = obj })
                .GroupBy(x => x.ParameterName);
            foreach(var group in parameterObjectBindings.ToList())
            {
                bindings.Add(group.ToList());
            }

            return GetPermutations(bindings);
        }
        /// <summary>
        /// Given a validation rule and a collection of tuples of parameter to object bindings (represented as list),
        /// create corresponding RuleBindings.
        /// We create a separate rule binding for each parameter to object binding tuple.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="objectBindings"></param>
        /// <returns></returns>
        private static IEnumerable<RuleBinding<TResult>> GetRuleBindings(ValidationRule<TResult> rule, IEnumerable<IEnumerable<ParameterObjectBinding>> objectBindings)
        {
            var result = new List<RuleBinding<TResult>>();
            foreach(var objectBinding in objectBindings)
            {
                var bindings = (from dependency in rule.Signature
                                select new ValidationRuleDependencyBinding
                                {
                                    ValidationRuleDependency = dependency,
                                    ParameterObjectBinding = objectBinding.Single(b => b.ParameterName == dependency.ParameterExpression.Name)
                                }).ToList();
                result.Add(new RuleBinding<TResult>
                {
                    DependencyBindings = bindings.ToArray(),
                    ValidationRule = rule
                });
            }
            return result;
        }
        /// <summary>
        /// Synthesizes all possible permutations for each collection in the provided list.
        /// That is, for a the collection
        /// { {a,b}, {c,d} }
        /// The following permutation is calculated:
        /// { {a, c}, {a, d}, {b, c}, {c, d} }
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<IEnumerable<T>> GetPermutations<T>(List<IEnumerable<T>> list)
        {
            if(list.Count() == 0)
            {
                return list;
            }
            return GetPermutations(list.First(), list.Skip(1).ToList());
        }
        /// <summary>
        /// This method does the actual work for computating the collection of permutations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Filter rules for which the collection of dependency bindings does not contain obj
        /// and rules for which any TargetOwnerObject of an dependency binings is null.
        /// </summary>
        /// <param name="bindings"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static IEnumerable<RuleBinding<TResult>> FilterRuleBindings(IEnumerable<RuleBinding<TResult>> bindings, object obj)
        {
            return (from binding in bindings
                    where
                        binding.DependencyBindings.All(b => b.TargetOwnerObject != null)
                        &&
                        (obj == null || binding.DependencyBindings.Any(b => b.TargetOwnerObject == obj))
                    select binding).ToList();
        }
        /// <summary>
        /// This is the actual validation method that invokes a collection of validation rules for a collection of validation
        /// rule bindings.
        /// For each rule, this amounts to:
        /// 1) Synthesizing the bindings of objects from the provided objects to parameters of validation rule dependencies 
        ///    defined in the signature of the validation rule.
        /// 2) Synthesizing all possible bindings for the signature of the validation rule
        /// 3) Filtering this collection to remove invalid bindings
        /// 4) For each resulting binding, invoke the validaton rule.
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="objects"></param>
        /// <param name="obj"></param>
        private void ValidateRules(IEnumerable<ValidationRule<TResult>> rules, IEnumerable<TEntity> objects, object obj)
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
        /// <summary>
        /// Callback method that is called when the result of a validation rule has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidationResultChangedCallback(object sender, ValidationResultChangedEventArgs<TResult> e)
        {
            if(ValidationResultChanged != null)
            {
                ValidationResultChanged(sender, e);
            }
        }
    }
}
