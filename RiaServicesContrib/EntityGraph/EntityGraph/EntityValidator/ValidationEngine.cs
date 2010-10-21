using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;

namespace RIA.EntityValidator
{
    public class ValidationEngine<TEntity, TResult> where TResult : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="root">The data element for which to create this EntityValidator</param>
        public ValidationEngine(TEntity root) {
            this.Root = root;
            this.ValidatorProviders = new List<IValidationRulesProvider<TEntity, TResult>>();

            // MEF doesn't support generics. Hence we explicitly add the MEFValidationRulesProvider here.
            var provider = new MEFValidationRulesProvider<TEntity, TResult>();
            MEFValidationRules.Catalog.Changed += (sender, args) => Refresh();
            var container = new CompositionContainer();
            container.ComposeParts(this, provider);
        }

        /// <summary>
        /// Refreshes the set of validation rules
        /// </summary>
        public void Refresh() {
            foreach(var validationRule in ValidationRules.Values.SelectMany(rules => rules))
            {
                validationRule.ValidationResultChanged -= validationRule_ValidationResultChanged;
            }
            ValidationRules.Clear();
            var list = ValidatorProviders.ToList();
            foreach(var validator in ValidatorProviders.SelectMany(provider => provider.GetValidators(Root)))
            {
                var key = validator.Item1;
                if(ValidationRules.ContainsKey(key) == false)
                {
                    ValidationRules.Add(key, new List<IValidationRule<TEntity, TResult>> { validator.Item2 });
                }
                else
                {
                    ValidationRules[key].Add(validator.Item2);
                }
                validator.Item2.ValidationResultChanged += validationRule_ValidationResultChanged;
            }
        }

        public event EventHandler<ValidationResultChangedEventArgs<TResult>> ValidationResultChanged;

        /// <summary>
        /// Collection of registered validator rule providers
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IValidationRulesProvider<TEntity, TResult>> ValidatorProviders;


        /// <summary>
        /// Returns the collection of objects that are involved in the given validation rule
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public IEnumerable<object> ObjectsInvolved(IValidationRule<TEntity, TResult> rule) {
            return from r in ValidationRules.Keys
                   where
                       ValidationRules[r].Contains(rule)
                   select r.Item1;
        }

        /// <summary>
        /// Validates the given property of the given object according to the current set of validation rules
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        public void Validate(object obj, string property) {
            var key = new Tuple<object, string>(obj, property);
            var list = ValidationRules.Keys.ToList();
            if(ValidationRules.ContainsKey(key))
            {
                foreach(var rule in ValidationRules[key])
                {
                    rule.InvokeValidate(Root);
                }
            }
        }

        private TEntity Root { get; set; }

        private Dictionary<Tuple<object, string>, List<IValidationRule<TEntity, TResult>>> ValidationRules =
            new Dictionary<Tuple<object, string>, List<IValidationRule<TEntity, TResult>>>();

        private void validationRule_ValidationResultChanged(object sender, ValidationResultChangedEventArgs<TResult> e) {
            if(ValidationResultChanged != null)
            {
                ValidationResultChanged(sender, e);
            }
        }
    }
}