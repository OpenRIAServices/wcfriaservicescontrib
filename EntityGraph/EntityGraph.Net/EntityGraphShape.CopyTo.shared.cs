using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

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
            if(fromEntity == null)
            {
                throw new ArgumentNullException("fromEntity");
            }
            if(typeMapper == null)
            {
                throw new ArgumentNullException("typeMapper");
            }
            return CopyTo<TFrom, TTo>(shape, fromEntity, typeMapper, new List<TFrom>());
        }


        private static TTo CopyTo<TFrom, TTo>(this IEntityGraphShape shape, TFrom fromEntity, ITypeMapper typeMapper, IList visited)
            where TFrom : class
            where TTo : class
        {
            var toEntity = CopyFromTo<TFrom, TTo>(fromEntity, typeMapper);
            Debug.Assert(visited.Contains(fromEntity) == false);
            visited.Add(fromEntity);

            // Determine if provided shape is defined for fromEntity or for toEntity.
            var outEdges = shape.OutEdges(fromEntity);
            if(outEdges.Count() == 0)
            {
                outEdges = shape.OutEdges(toEntity);
            }
            var fromType = fromEntity.GetType();
            var toType = toEntity.GetType();

            foreach(var edge in outEdges)
            {
                var fromPropInfo = fromType.GetProperty(edge.Name);
                var toPropInfo = toType.GetProperty(edge.Name);
                if(fromPropInfo == null || toPropInfo == null)
                {
                    continue;
                }
                var fromPropvalue = fromPropInfo.GetValue(fromEntity, null);
                if(fromPropvalue == null)
                {
                    continue;
                }
                if(typeof(IEnumerable).IsAssignableFrom(fromPropInfo.PropertyType))
                {
                    var fromChildren = (IEnumerable)fromPropvalue;

                    IEnumerable toList = (IEnumerable)toPropInfo.GetValue(toEntity, null);
                    // If the IEnumerable is null, lets try to allocate one
                    if(toList == null)
                    {
                        var constr = toPropInfo.PropertyType.GetConstructor(new Type[] { });
                        toList = (IEnumerable)constr.Invoke(new object[] { });
                        toPropInfo.SetValue(toEntity, toList, null);
                    }
                    var addMethod = toPropInfo.PropertyType.GetMethod("Add");
                    foreach(var fromChild in fromChildren)
                    {
                        if(visited.Contains(fromChild) == false)
                        {
                            var toChild = shape.CopyTo<TFrom, TTo>((TFrom)fromChild, typeMapper, visited);
                            addMethod.Invoke(toList, new object[] { toChild });
                        }
                    }
                }
                else
                {
                    var fromChild = (TFrom)fromPropvalue;
                    if(visited.Contains(fromChild) == false)
                    {
                        var toChild = shape.CopyTo<TFrom, TTo>(fromChild, typeMapper, visited);
                        toPropInfo.SetValue(toEntity, toChild, null);
                    }
                }
            }
            return toEntity;
        }
        #region Helper functions
        /// <summary>
        /// This method returns an array of ProperTypeInfo objects which from the data member (I.e., non-naviagion) properties 
        /// that must be copied.
        /// The list is determined as follows:
        /// 1) A property must have the DataMemberAttribute defined on both the fromObjectType and the toObjectType.
        /// 2) If includeKeys==false, the KeyAttribute should not be defined on both types
        /// 3) The property must be readable on fromObjectType
        /// 4) The property must be writable on toObjectType
        /// The returnd list consists of ProperTypeInfo objects for type toObjectType.
        /// </summary>
        /// <param name="fromObjectType"></param>
        /// <param name="toObjectType"></param>
        /// <param name="includeKeys"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetDataMembers(Type fromObjectType, Type toObjectType, bool includeKeys)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qryToObject = from p in toObjectType.GetProperties(bindingAttr)
                              where
                              p.IsDefined(typeof(DataMemberAttribute), true)
                              && (includeKeys || p.IsDefined(typeof(KeyAttribute), true) == false)
                              && p.CanWrite
                              select p;
            var qryFromObject = from p in fromObjectType.GetProperties(bindingAttr)
                                where
                                 p.IsDefined(typeof(DataMemberAttribute), true)
                                && (includeKeys || p.IsDefined(typeof(KeyAttribute), true) == false)
                                && p.CanRead
                                select p;
            var result = from propToObject in qryToObject
                         from propFromObject in qryFromObject
                         where propToObject.Name == propFromObject.Name
                         select propToObject;
            return result.ToArray();
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
            foreach(var prop in GetDataMembers(fromObjectType, toObject.GetType(), true))
            {
                var propInfo = fromObjectType.GetProperty(prop.Name);
                var value = propInfo.GetValue(fromObject, null);
                if(value == null)
                {
                    continue;
                }
                if(prop.PropertyType.IsAssignableFrom(propInfo.PropertyType))
                {
                    prop.SetValue(toObject, value, null);
                }
                else // Complex type
                {
                    var propValue = prop.GetValue(toObject, null);
                    var propType = prop.PropertyType;
                    if(propValue == null)
                    {
                        propValue = Activator.CreateInstance(propType);
                    }
                    prop.SetValue(toObject, propValue, null);
                    CopyDataMembers(value, propValue);
                }
            }
        }
        #endregion
    }
}
