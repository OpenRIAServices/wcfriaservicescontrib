using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;

namespace EntityGraph.Validation
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
        /// Registers a validation rule by its type.
        /// </summary>
        /// <param name="type"></param>
        public static void RegisterType(Type type)
        {
            if(GetTypeCatalog(type) == null)
                Catalog.Catalogs.Add(new TypeCatalog(type));
        }

        /// <summary>
        /// Unregisters a validation rule given its type.
        /// </summary>
        /// <param name="type"></param>
        public static void UnregisterType(Type type)
        {
            var typeCatalog = GetTypeCatalog(type);
            if(typeCatalog != null)
                Catalog.Catalogs.Remove(typeCatalog);
        }
        /// <summary>
        /// Registers a collection of entity validation rules in given assembly.
        /// </summary>
        /// <param name="assembly"></param>
        public static void RegisterAssembly(Assembly assembly)
        {
            if(Catalog.Catalogs.OfType<AssemblyCatalog>().Any(ac => ac.Assembly == assembly) == false)
                Catalog.Catalogs.Add(new AssemblyCatalog(assembly));
        }
        /// <summary>
        /// Unregisters the given assembly with validation rules.
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

    /// <summary>
    /// Class that provides a collection of validation rules which are loaded
    /// using MEF.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class MEFValidationRulesProvider<TResult> : IValidationRulesProvider<TResult>
        where TResult : class
    {
        /// <summary>
        /// Creates a new instance of the MEFValidationRulesProvider
        /// </summary>
        public MEFValidationRulesProvider() {
            //Create the CompositionContainer with the parts in the catalog
            container = new CompositionContainer(MEFValidationRules.Catalog);

            //Fill the imports of this object
            container.ComposeParts(this);
        }

        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<ValidationRule<TResult>> ValidationRules { get; set; }

        private CompositionContainer container;
    }
}
