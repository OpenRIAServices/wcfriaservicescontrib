using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
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
        /// Don't know how fidn out if the aggregate catalog contains a type catalog for the given type.
        /// Therefore, we do our own book keeping using a dictionary.
        /// </summary>
        private static Dictionary<Type, TypeCatalog> RegisteredTypes = new Dictionary<Type, TypeCatalog>();

        /// <summary>
        /// Registers given entity validation rule
        /// </summary>
        /// <param name="type"></param>
        public static void RegisterType(Type type)
        {
            if(RegisteredTypes.ContainsKey(type))
                return;
            var tc = new TypeCatalog(type);
            RegisteredTypes.Add(type, tc);
            Catalog.Catalogs.Add(tc);
        }

        /// <summary>
        /// Unregisters give type 
        /// </summary>
        /// <param name="type"></param>
        public static void UnregisterType(Type type)
        {
            if(RegisteredTypes.ContainsKey(type) == false)
                return;
            var tc = RegisteredTypes[type];
            Catalog.Catalogs.Remove(tc);
            RegisteredTypes.Remove(type);
        }
        /// <summary>
        /// Registers collection of entity validation rules in given assembly.
        /// </summary>
        /// <param name="assembly"></param>
        public static void RegisterAssembly(Assembly assembly)
        {
            if(Catalog.Catalogs.OfType<AssemblyCatalog>().Any(ac => ac.Assembly == assembly) == false)
                Catalog.Catalogs.Add(new AssemblyCatalog(assembly));
        }
        /// <summary>
        /// Unregisters the given assembly with validatio rules
        /// </summary>
        /// <param name="assembly"></param>
        public static void UnregisterAssembly(Assembly assembly)
        {
            var catalog = Catalog.Catalogs.OfType<AssemblyCatalog>().FirstOrDefault(ac => ac.Assembly == assembly);
            if(catalog != null)
            {
                Catalog.Catalogs.Remove(catalog);
            }
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
        public IEnumerable<ValidationRule<TEntity, TResult>> EntityValidators { get; set; }

        public Dictionary<Tuple<object, string>, List<IValidationRule<TResult>>> GetValidationRules(TEntity root)
        {
            var rules = new Dictionary<Tuple<object, string>, List<IValidationRule<TResult>>>();

            foreach(var validator in EntityValidators)
            {
                var signature = validator.Signature;
                foreach(Expression<Func<TEntity, object>> method in signature)
                {
                    var key = ResolveDependency(method, root);
                    if(rules.ContainsKey(key))
                    {
                        rules[key].Add(validator);
                    }
                    else
                    {
                        rules.Add(key, new List<IValidationRule<TResult>> { validator });
                    }
                }
            }
            return rules;
        }

        private CompositionContainer container;

        static private object GetValueFromExpression(Expression expr, object o)
        {
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
