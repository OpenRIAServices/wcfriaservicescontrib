using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
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
        /// Query to obtain the type catalog that holds the given type
        /// </summary>
        private static Func<Type, TypeCatalog> GetTypeCatalog =
            type => (from tc in Catalog.Catalogs.OfType<TypeCatalog>()
                     from part in tc.Parts
                     where
                     ReflectionModelServices.GetPartType(part).Value == type
                     select tc).SingleOrDefault();

        /// <summary>
        /// Registers given entity validation rule
        /// </summary>
        /// <param name="type"></param>
        public static void RegisterType(Type type)
        {
            if(GetTypeCatalog(type) == null)
                Catalog.Catalogs.Add(new TypeCatalog(type));
        }

        /// <summary>
        /// Unregisters give type 
        /// </summary>
        /// <param name="type"></param>
        public static void UnregisterType(Type type)
        {
            var typeCatalog = GetTypeCatalog(type);
            if(typeCatalog != null)
                Catalog.Catalogs.Remove(typeCatalog);
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

    public class MEFValidationRulesProvider<TRoot, TEntity, TResult> : 
        IValidationRulesProvider<TRoot, TEntity, TResult>
        where TRoot : TEntity
        where TResult : class
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
        public IEnumerable<ValidationRule<TRoot, TResult>> EntityValidators { get; set; }

        public Dictionary<Tuple<TEntity, string>, List<IValidationRule<TRoot, TResult>>> GetValidationRules(TRoot root)
        {
            var rules = new Dictionary<Tuple<TEntity, string>, List<IValidationRule<TRoot, TResult>>>();

            foreach(var validator in EntityValidators)
            {
                var tmpRules = new Dictionary<Tuple<TEntity, string>, List<IValidationRule<TRoot, TResult>>>();
                var signature = validator.Signature;

                foreach(Expression<Func<TRoot, object>> method in signature)
                {
                    var key = ResolveDependency(method, root);
                    if(key.Item1 == null)
                        break;
                    AddRule(tmpRules, key, validator);
                }
                // Only add members from signature if they are all reachable from 'root'. I.e.,
                // non of the paths include a null element
                if(tmpRules.Count == signature.Count)
                {
                    foreach(var key in tmpRules.Keys)
                    {
                        foreach(var v in tmpRules[key])
                            AddRule(rules, key, v);
                    }
                }
            }
            return rules;
        }
        private void AddRule(Dictionary<Tuple<TEntity, string>, List<IValidationRule<TRoot, TResult>>> rules,
            Tuple<TEntity, string> key, IValidationRule<TRoot, TResult> validator)
        {
            if(rules.ContainsKey(key))
            {
                rules[key].Add(validator);
            }
            else
            {
                rules.Add(key, new List<IValidationRule<TRoot, TResult>> { validator });
            }
        }
        private CompositionContainer container;

        static private TEntity GetValueFromExpression(Expression expr, TEntity entity)
        {
            if(expr is ParameterExpression)
            {
                return entity;
            }
            var memberExpression = expr as MemberExpression;
            var propValue = GetValueFromExpression(memberExpression.Expression, entity);
            var value =((PropertyInfo)memberExpression.Member).GetValue(propValue, null);
            return (TEntity)value;
        }
        static private Tuple<TEntity, string> ResolveDependency(Expression<Func<TRoot, object>> propertyExpression, TEntity arg)
        {
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
                throw new ArgumentException("propertyExpression should be a member expression");
            var propertyPath = body.Member;
            if(body.Expression is ParameterExpression)
            {
                return new Tuple<TEntity, string>(arg, propertyPath.Name);
            }
            else
            {
                var objectExpression = body.Expression as MemberExpression;
                if(objectExpression == null)
                    throw new ArgumentException("propertyExpression should be a member expression");
                var objectPath = objectExpression.Member;
                return new Tuple<TEntity, string>(GetValueFromExpression(body.Expression, arg), propertyPath.Name);
            }
        }
    }
}
