using System;
using System.Collections.Generic;
using System.Linq;

namespace RIA.EntityValidator
{
    public class ValidationEngine<TRoot, TEntity, TResult>
        where TEntity : class
        where TResult : class
    {
        private IValidationRulesProvider<TRoot, TEntity, TResult> RulesProvider;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="root">The data element for which to create this EntityValidator</param>
        public ValidationEngine(IValidationRulesProvider<TRoot, TEntity, TResult> rulesProvider, TRoot root)
        {
            this.Root = root;
            this.RulesProvider = rulesProvider;
        }

        /// <summary>
        /// Refreshes the set of validation rules
        /// </summary>
        public void Refresh()
        {
            foreach(var validationRule in ValidationRules.Values.SelectMany(rules => rules))
            {
                validationRule.ValidationResultChanged -= validationRule_ValidationResultChanged;
            }

            ValidationRules = RulesProvider.GetValidationRules(Root);

            foreach(var validationRule in ValidationRules.Values.SelectMany(rules => rules))
            {
                validationRule.ValidationResultChanged += validationRule_ValidationResultChanged;
            }
        }

        public event EventHandler<ValidationResultChangedEventArgs<TResult>> ValidationResultChanged;
        /// <summary>
        /// Returns the collection of objects that are involved in any validation rule of the current validation engine
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> ObjectsInvolved()
        {
            return ValidationRules.Keys.Select(key => key.Item1).Distinct();
        }

        /// <summary>
        /// Returns the collection of objects that are involved in the given validation rule
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> ObjectsInvolved(IValidationRule<TRoot, TResult> rule)
        {
            return (from r in ValidationRules.Keys
                    where
                        ValidationRules[r].Contains(rule)
                    select r.Item1).Distinct();
        }
        /// <summary>
        /// Returns the collection of properties that are involved in any validation rule for the given entity
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> PropertiesInvolved(TEntity entity)
        {
            return ValidationRules.Keys.Where(key => key.Item1 == entity).Select(key => key.Item2).Distinct();
        }
        /// <summary>
        /// Returns the collection of properties of the given entity that are involved in the given validation rule.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public IEnumerable<string> PropertiesInvolved(TEntity entity, IValidationRule<TRoot, TResult> rule)
        {
            return ValidationRules.Where(r => r.Key.Item1 == entity && r.Value.Contains(rule)).Select(r => r.Key.Item2).Distinct();
        }
        /// <summary>
        /// Validates the given property of the given object according to the current set of validation rules
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        public void Validate(TEntity entity, string property)
        {
            var key = new Tuple<TEntity, string>(entity, property);
            var list = ValidationRules.Keys.ToList();
            if(ValidationRules.ContainsKey(key))
            {
                foreach(var rule in ValidationRules[key])
                {
                    rule.InvokeValidate(Root);
                }
            }
        }
        /// <summary>
        /// Validates all registered objects/properties
        /// </summary>
        public void ValidateAll()
        {
            var rules = ValidationRules.Values.SelectMany(id => id).Distinct().ToList();
            rules.ForEach(rule => rule.InvokeValidate(Root));
        }

        private TRoot Root { get; set; }

        private Dictionary<Tuple<TEntity, string>, List<IValidationRule<TRoot, TResult>>> _validationRules;
        private Dictionary<Tuple<TEntity, string>, List<IValidationRule<TRoot, TResult>>> ValidationRules
        {
            get
            {
                if(_validationRules == null)
                {
                    _validationRules = new Dictionary<Tuple<TEntity, string>, List<IValidationRule<TRoot, TResult>>>();
                    Refresh();
                }
                return _validationRules;
            }
            set
            {
                _validationRules = value;
            }
        }

        private void validationRule_ValidationResultChanged(object sender, ValidationResultChangedEventArgs<TResult> e)
        {
            if(ValidationResultChanged != null)
            {
                ValidationResultChanged(sender, e);
            }
        }
    }
}