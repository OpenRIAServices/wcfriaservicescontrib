using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityGraph.Validation
{
    internal static class ValidationRuleEvaluator<TValidationResult> where TValidationResult : class
    {
        public static void Evaluate(RuleBinding<TValidationResult> binding) {
            var signature = binding.ValidationRule.Signature;
            var type = binding.ValidationRule.GetType();
            var method = binding.ValidationRule.ValidationMethod;

            if(signature.Count != binding.DependencyBindings.Count())
            {
                string msg = String.Format(@"Argument count mismatch between Signature and rule method ""{0}"" in class {1}.", method.Name, type.Name);
                throw new Exception(msg);
            }

            var bindings = new object[signature.Count];
            for(int i = 0; i < signature.Count; i++)
            {
                var ibinding = binding.DependencyBindings[i];
                bindings[i] = ibinding.ValidationRuleDependency.TargetProperty.GetValue(ibinding.TargetOwnerObject, null);
            }
            binding.ValidationRule.RuleBinding = binding;
            method.Invoke(binding.ValidationRule, bindings);
        }
        /// <summary>
        /// This method tries to find a method which has the name "Validate" (or is annotated
        /// with the "Validate" attribute) and for which the signature matches the given signature.
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal static MethodInfo GetValidateMethod(ValidationRule<TValidationResult> rule) {
            Type type = rule.GetType();
            MethodInfo[] methods = null;

            var validators = from m in type.GetMethods()
                             where
                                 m.IsDefined(typeof(ValidateMethodAttribute), true)
                             select m;
            if(validators.Count() == 0)
            {
                validators = type.GetMethods().Where(m => m.Name.StartsWith("Validate"));
            }
            methods = GetMatchesValidationMethods(validators.ToArray(), rule.Signature);
            if(methods.Count() > 1)
            {
                string msg = String.Format("Only one method in class {0} can be decorated with the 'Validator' attribute.", type.Name);
                throw new Exception(msg);
            }
            if(methods.Count() == 0)
            {
                string msg = String.Format("No validation method could be found in {0} that matches the signature.", type.Name);
                throw new Exception(msg);
            }
            return methods.Single();
        }
        /// <summary>
        /// This method filters the array of methods for those methods tha match the given signature.
        /// </summary>
        /// <param name="methods"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        private static MethodInfo[] GetMatchesValidationMethods(MethodInfo[] methods, Signature signature) {
            List<MethodInfo> matchingMethods = new List<MethodInfo>();
            foreach(var method in methods)
            {
                var parameters = method.GetParameters();
                if(parameters.Count() != signature.Count())
                {
                    continue;
                }
                if(method.ReturnType != typeof(void))
                {
                    continue;
                }
                var tuples = parameters.Zip(signature, (p, a) => new { p.ParameterType, ArgumentType = a });
                if(tuples.All(t => t.ParameterType.IsAssignableFrom(t.ArgumentType.TargetProperty.PropertyType)))
                {
                    matchingMethods.Add(method);
                }
            }
            return matchingMethods.ToArray();
        }
    }
}
