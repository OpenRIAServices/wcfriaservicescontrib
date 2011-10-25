using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;

namespace RiaServicesContrib
{
    /// <summary>
    /// Interface used by EntityGraphShape.CopyTo. It contains a single method 'Map' that maps one type into another.
    /// </summary>
    public interface ITypeMapper
    {
        /// <summary>
        /// Method that maps fromType to another type. 
        /// </summary>
        /// <param name="fromType"></param>
        /// <returns></returns>
        Type Map(Type fromType);
    }
    /// <summary>
    /// Class that maps a given type 'TFrom' to a type 'TTo' where 'TTo' has the same name as `TFrom' but is
    /// defined in the namespace/assembly of the generic type parameter 'ToType'.
    /// from the assembly and namespace 
    /// </summary>
    /// <typeparam name="ToType"></typeparam>
    public class AssemblyTypeMapper<ToType> : ITypeMapper
        where ToType : class
    {
        /// <summary>
        /// Initializes a new instance of the AssemblyTypeMapper class.
        /// </summary>
        public AssemblyTypeMapper()
        {
        }
        public Type Map(Type fromType)
        {
            var assembly = typeof(ToType).Assembly;
            var nameSpaceName = typeof(ToType).Namespace;
            return assembly.GetType(nameSpaceName + "." + fromType.Name);
        }
    }

    public static partial class EntityGraphShapeExtensions
    {
        /// <summary>
        /// Copies a graph of entities of base type TFrom to a similar entity graph with base type TTo given the
        /// provided entity graph shape and type mapper.
        /// </summary>
        /// <typeparam name="TFrom">The base type of the source entity graph</typeparam>
        /// <typeparam name="TTo">The base type of the target entity graph</typeparam>
        /// <param name="shape">The entity graph shape defining the structure of the graph</param>
        /// <param name="fromEntity">The original entity</param>
        /// <param name="typeMapper">A class implementing ITypeMapper that maps subtypes of TFrom to sub types of TTo</param>
        /// <returns></returns>
        public static TTo CopyTo<TFrom, TTo>(this IEntityGraphShape shape, TFrom fromEntity, ITypeMapper typeMapper)
            where TFrom : class
            where TTo : class
        {
            var toEntity = CopyFromTo<TFrom, TTo>(fromEntity, typeMapper);

            // Determine if provided shape is defined for fromEntity or for toEntity.
            var outEdges = shape.OutEdges(fromEntity);
            if (outEdges.Count() == 0)
            {
                outEdges = shape.OutEdges(toEntity);
            }
            var fromType = fromEntity.GetType();
            var toType = toEntity.GetType();

            foreach (var edge in outEdges)
            {
                var fromPropInfo = fromType.GetProperty(edge.Name);
                var toPropInfo = toType.GetProperty(edge.Name);
                var fromPropvalue = fromPropInfo.GetValue(fromEntity, null);
                if (fromPropvalue == null)
                {
                    continue;
                }
                if (typeof(IEnumerable).IsAssignableFrom(edge.PropertyType))
                {
                    var fromChildren = (IEnumerable)fromPropvalue;

                    IEnumerable toList = (IEnumerable)toPropInfo.GetValue(toEntity, null);
                    // If the IEnumerable is null, lets try to allocate one
                    if (toList == null)
                    {
                        var constr = toPropInfo.PropertyType.GetConstructor(new Type[] { });
                        toList = (IEnumerable)constr.Invoke(new object[] { });
                        toPropInfo.SetValue(toEntity, toList, null);
                    }
                    var addMethod = toPropInfo.PropertyType.GetMethod("Add");
                    foreach (var fromChild in fromChildren)
                    {
                        var toChild = shape.CopyTo<TFrom, TTo>((TFrom)fromChild, typeMapper);
                        addMethod.Invoke(toList, new object[] { toChild });
                    }
                }
                else
                {
                    var fromChild = (TFrom)fromPropvalue;
                    var toChild = shape.CopyTo<TFrom, TTo>(fromChild, typeMapper);
                    edge.SetValue(toEntity, toChild, null);
                }
            }
            return toEntity;
        }
        #region Helper functions
        private static PropertyInfo[] GetDataMembers(object obj, bool includeKeys)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qry = from p in obj.GetType().GetProperties(bindingAttr)
                      where
                        p.IsDefined(typeof(DataMemberAttribute), true)
                      && (includeKeys || p.IsDefined(typeof(KeyAttribute), true) == false)
                      && p.CanWrite
                      select p;
            return qry.ToArray();
        }

        private static TTo CopyFromTo<TFrom, TTo>(TFrom fromEntity, ITypeMapper typeMapper)
            where TFrom : class
            where TTo : class
        {
            var fromEntityType = fromEntity.GetType();
            var toEntityType = typeMapper.Map(fromEntityType);
            var toEntity = (TTo)Activator.CreateInstance(toEntityType);
            
            CopyDataMembers(fromEntity, toEntity);
            return toEntity;
        }
        /// <summary>
        /// Copies properties annotated with the DataMemberAttribute from fromObject to toObject.
        /// If property types are not assignable, consider them as complex types and call CopyDataMembers
        /// recursively on their values.
        /// </summary>
        /// <param name="fromObject"></param>
        /// <param name="toObject"></param>
        private static void CopyDataMembers(object fromObject, object toObject)
        {
            var fromObjectType = fromObject.GetType();
            foreach (var prop in GetDataMembers(toObject, true))
            {
                var propInfo = fromObjectType.GetProperty(prop.Name);
                var value = propInfo.GetValue(fromObject, null);
                if (value == null)
                {
                    continue;
                }
                if (prop.PropertyType.IsAssignableFrom(propInfo.PropertyType))
                {
                    prop.SetValue(toObject, value, null);
                }
                else
                {
                    var obj = prop.GetValue(toObject, null);
                    if (obj == null)
                    {
                        obj = Activator.CreateInstance(prop.PropertyType);
                    } 
                    prop.SetValue(toObject, obj, null);
                    CopyDataMembers(value, obj);
                }
            }
        }
        #endregion
    }
}
