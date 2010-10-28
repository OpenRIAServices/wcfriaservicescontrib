using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using System.Reflection;

namespace RIA.EntityValidator
{
    /// <summary>
    /// Class that holds the singleton AggregateCatalog for storing entity validation rules 
    /// </summary>
    public static class MEFValidationRules
    {
        private static AggregateCatalog _catalog = new AggregateCatalog();

        public static AggregateCatalog Catalog { get { return _catalog; } }
        /// <summary>
        /// Registers given entity validation rule 
        /// </summary>
        /// <param name="type"></param>
        public static void RegisterType(Type type) {
            Catalog.Catalogs.Add(new TypeCatalog(type));
        }
        /// <summary>
        /// Registers collection of entity validation rules in given assembly.
        /// </summary>
        /// <param name="assembly"></param>
        public static void RegisterAssembly(Assembly assembly) {
            Catalog.Catalogs.Add(new AssemblyCatalog(assembly));
        }
    }

    public class MEFValidationRulesProvider<TEntity, TResult> : 
        IValidationRulesProvider<TEntity, TResult> where TResult : class
    {

        public MEFValidationRulesProvider() {
            //Adds all the parts found in the same assembly as the Program class
            var assembly = this.GetType().Assembly;
            MEFValidationRules.RegisterAssembly(assembly);

            //Create the CompositionContainer with the parts in the catalog
            container = new CompositionContainer(MEFValidationRules.Catalog);

            //Fill the imports of this object
            container.ComposeParts(this);
        }

        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IValidationRule<TEntity, TResult>> EntityValidators { get; set; }

        public IEnumerable<Tuple<Tuple<object, string>, IValidationRule<TEntity, TResult>>> GetValidators(TEntity root) {
            foreach(var validator in EntityValidators)
            {
                var signature = validator.Signature;
                foreach(Expression<Func<TEntity, object>> method in signature)
                {
                    var key = ResolveDependency(method, root);
                    yield return new Tuple<Tuple<object, string>, IValidationRule<TEntity, TResult>>(key, validator);
                }
            }
        }

        private CompositionContainer container;

        static private object GetValueFromExpression(Expression expr, object o) {
            if(expr is ParameterExpression)
            {
                return o;
            }
            var memberExpression = expr as MemberExpression;
            var propValue = GetValueFromExpression(memberExpression.Expression, o);
            return ((PropertyInfo)memberExpression.Member).GetValue(propValue, null);
        }

        static private Tuple<object, string> ResolveDependency(Expression<Func<TEntity, object>> propertyExpression, object arg) {

            MemberExpression body = null;
            if(propertyExpression.Body is UnaryExpression)
            {
                var unary = propertyExpression.Body as UnaryExpression;
                if(unary.Operand is MemberExpression)
                    body = unary.Operand as MemberExpression;
            }
            else if(propertyExpression.Body is MemberExpression)
            {
                body = propertyExpression.Body as MemberExpression;
            }
            if(body == null)
                throw new ArgumentException("propertyExpression' should be a member expression");
            var propertyPath = body.Member;
            if(body.Expression is ParameterExpression)
            {
                return new Tuple<object, string>(arg, propertyPath.Name);
            }
            else
            {
                var objectExpression = body.Expression as MemberExpression;
                if(objectExpression == null)
                    throw new ArgumentException("propertyExpression' should be a member expression");
                var objectPath = objectExpression.Member;
                return new Tuple<object, string>(GetValueFromExpression(body.Expression, arg), propertyPath.Name);
            }
        }
    }
}
