using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

namespace EntityGraph.Validation
{
    [InheritedExport]
    public abstract class ValidationRule<TValidationResult> where TValidationResult : class
    {
        public Signature Signature { get; private set; }
        public event EventHandler<ValidationResultChangedEventArgs<TValidationResult>> ValidationResultChanged;

        internal MethodInfo ValidationMethod { get; set; }
        internal RuleBinding<TValidationResult> RuleBinding { get; set; }
        protected ValidationRule(Signature signature)
        {
            this.Signature = signature;
            ValidationMethod = ValidationRuleEvaluator<TValidationResult>.GetValidateMethod(this);
        }
        internal IEnumerable<ParameterObjectBinding> GetValidationRuleDependencyParameters()
        {
            var oBindings = from dependency in Signature
                            select new ParameterObjectBinding
                            {
                                ParameterName = dependency.ParameterExpression.Name,
                                ParameterObjectType = dependency.ParameterExpression.Type
                            };
            return oBindings.Distinct();
        }

        internal void Evaluate(RuleBinding<TValidationResult> binding)
        {
            ValidationRuleEvaluator<TValidationResult>.Evaluate(binding);
        }
        private TValidationResult _validationResult;
        public TValidationResult ValidationResult
        {
            get
            {
                return _validationResult;
            }
            protected set
            {
                if(_validationResult != value)
                {
                    TValidationResult old = _validationResult;
                    _validationResult = value;
                    if(ValidationResultChanged != null)
                    {
                        ValidationResultChanged(this, new ValidationResultChangedEventArgs<TValidationResult>(old, value));
                    }
                }
            }
        }
    }
}
