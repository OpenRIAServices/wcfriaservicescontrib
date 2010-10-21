using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RIA.EntityValidator
{
    internal static class ValidationRuleInvokeHelper<TEntity, TResult> where TResult : class
    {
        public static void InvokeValidator(IValidationRule<TEntity, TResult> validator, TEntity entity) {
            var signature = validator.Signature;
            var type = validator.GetType();

            Type[] parameterTypes = new Type[signature.Count()];
            for(int i = 0; i < signature.Count(); i++)
            {
                parameterTypes[i] = GetExpressionType(signature[i].Body);
            }
            var method = FindValidateMethod(validator, parameterTypes);

            object[] parameters = new object[signature.Count];
            for(int i = 0; i < signature.Count; i++)
            {
                var func = signature[i].Compile();
                parameters[i] = func(entity);
            }
            method.Invoke(validator, parameters);
        }
        /// <summary>
        /// This method tries to find a method which has the name "Validate" (or is annotated
        /// with the "Validate" attribute) and for which the signature matches the given signature.
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static MethodInfo FindValidateMethod(IValidationRule<TEntity, TResult> validator, Type[] signature) {
            Type type = validator.GetType();
            MethodInfo[] methods = null;

            var validators = from m in type.GetMethods()
                             where
                                 m.IsDefined(typeof(ValidateMethodAttribute), true)
                             select m;
            if(validators.Count() == 0)
            {
                validators = type.GetMethods().Where(m => m.Name.StartsWith("Validate"));
            }
            methods = GetMatchesValidationMethods(validators.ToArray(), signature);
            if(methods.Count() > 1)
            {
                string msg = String.Format("Only one method in class {0} can be decorated with the 'Validator' attribute", type.Name);
                throw new Exception(msg);
            }
            if(methods.Count() == 0)
            {
                string msg = String.Format("No validation method could be found in {0} that matches the signature", type.Name);
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
        public static MethodInfo[] GetMatchesValidationMethods(MethodInfo[] methods, Type[] signature) {
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
                if(tuples.All(t => t.ParameterType.IsAssignableFrom(t.ArgumentType)))
                {
                    matchingMethods.Add(method);
                }
            }
            return matchingMethods.ToArray();
        }
        public static Type GetExpressionType(Expression expr) {
            if(expr is UnaryExpression)
            {
                var unary = expr as UnaryExpression;
                return GetExpressionType(unary.Operand);
            }
            if(expr is ParameterExpression)
            {
                return ((ParameterExpression)expr).Type;
            }
            else
            {
                return ((MemberExpression)expr).Type;
            }
        }
    }
}
