using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Class that maps a given type 'TFrom' to an equally named type defined in the given assemblies.
    /// </summary>
    public class AssembliesTypeMapper : ITypeMapper
    {
        private readonly IEnumerable<Assembly> _assemblies;
        /// <summary>
        /// Initializes a new instance of the AssemblyTypeMapper class.
        /// </summary>
        /// <param name="types">a list of types of which the containing assemblies are searched for the type mapping.</param>
        public AssembliesTypeMapper(params Type[] types)
        {
            _assemblies = types.Select(x => x.Assembly).ToList();
        }

        #region Implementation of ITypeMapper

        public Type Map(Type fromType)
        {
            foreach(var assembly in _assemblies)
            {
                var types = assembly.GetExportedTypes().Where(t => t != fromType);
                var toType = types.SingleOrDefault(type => type.Name.Equals(fromType.Name));
                if(toType != null)
                {
                    return toType;                    
                }
            }
            throw new Exception("Can't map type " + fromType.FullName);
        }

        #endregion
    }
    /// <summary>
    /// Class that maps a given type 'TFrom' to an equally named type defined in the assembly that contains type 'TTo'.
    /// </summary>
    /// <typeparam name="TTo"></typeparam>
    public class AssemblyTypeMapper<TTo> : AssembliesTypeMapper
        where TTo : class
    {
        /// <summary>
        /// Initializes a new instance of the AssemblyTypeMapper class.
        /// </summary>
        public AssemblyTypeMapper()
            : base(typeof(TTo))
        {
        }
    }

    /// <summary>
    /// Class that maps a given type 'TFrom' to a type defined in the assembly that contains type 'TTo1' or 'TTo2'.
    /// </summary>
    /// <typeparam name="TTo1"></typeparam>
    /// <typeparam name="TTo2"></typeparam>
    public class AssemblyTypeMapper<TTo1, TTo2> : AssembliesTypeMapper
        where TTo1 : class
        where TTo2 : class
    {
        /// <summary>
        /// Initializes a new instance of the AssemblyTypeMapper class.
        /// </summary>
        public AssemblyTypeMapper()
            : base(typeof(TTo1), typeof(TTo2))
        {
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
            var outEdges = shape.OutEdges(fromEntity).ToList();
            if(!outEdges.Any())
            {
                outEdges = shape.OutEdges(toEntity).ToList();
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

                    var toList = (IEnumerable)toPropInfo.GetValue(toEntity, null);
                    // If the IEnumerable is null, lets try to allocate one
                    if(toList == null)
                    {
                        var constr = toPropInfo.PropertyType.GetConstructor(new Type[] {});
                        toList = (IEnumerable)constr.Invoke(new object[] {});
                        toPropInfo.SetValue(toEntity, toList, null);
                    }
                    var addMethod = toPropInfo.PropertyType.GetMethod("Add");
                    foreach(var fromChild in fromChildren)
                    {
                        if(visited.Contains(fromChild) == false)
                        {
                            var toChild = shape.CopyTo<TFrom, TTo>((TFrom)fromChild, typeMapper, visited);
                            addMethod.Invoke(toList, new object[] {toChild});
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
            const BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
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

            if(toEntityType == null)
            {
                throw new Exception(String.Format("EntityGraphShape.Copy: Can't find a mapping for type {0}.",
                                                  fromEntityType.FullName));
            }
            var toEntity = (TTo)Activator.CreateInstance(toEntityType);

            CopyDataMembers(fromEntity, toEntity);
            return toEntity;
        }
        /// <summary>
        /// Generic Cast method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        private static T Cast<T>(object o)
        {
            return (T)o;
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
            foreach(var toProperty in GetDataMembers(fromObjectType, toObject.GetType(), true))
            {
                var fromProperty = fromObjectType.GetProperty(toProperty.Name);
                var fromValue = fromProperty.GetValue(fromObject, null);
                if(fromValue == null)
                {
                    continue;
                }
                var toValue = CopyDataMember(toProperty.PropertyType, fromProperty.PropertyType, fromValue);
                toProperty.SetValue(toObject, toValue, null);
            }
        }

        private static object CopyDataMember(Type toType, Type fromType, object fromValue)
        {
            if(toType.IsAssignableFrom(fromType))
            {
                return fromValue;
                //                toProperty.SetValue(toObject, fromValue, null);
            }
            if(typeof(Enum).IsAssignableFrom(toType) &&
               typeof(Enum).IsAssignableFrom(fromType))
            {
                var toEnumPropertyType = Enum.GetUnderlyingType(toType);
                var fromEnumPropertyType = Enum.GetUnderlyingType(fromType);
                if(toEnumPropertyType.IsAssignableFrom(fromEnumPropertyType) == false)
                {
                    throw new Exception("Incompatible enum types encountered: " + toType.Name + " " +
                                        fromType.Name);
                }
                // Cast the enum value fromValue to its target enum value using the generic Cast method.
                var castMethod = typeof(EntityGraphShapeExtensions).GetMethod("Cast",
                                                                              BindingFlags.Static |
                                                                              BindingFlags.NonPublic);
                var castMethodGeneric = castMethod.MakeGenericMethod(toEnumPropertyType);
                var convertedValue = castMethodGeneric.Invoke(null, new[] {fromValue});
                return convertedValue;
            }
            if(IsGenericCollection(typeof(ICollection<>), toType) &&
               IsGenericCollection(typeof(IEnumerable<>), fromType))
            {
                var fromEnumValues = (IEnumerable)fromValue;
                var fromElementType = fromType.GetGenericArguments()[0];
                var toElementType = toType.GetGenericArguments()[0];
                var constr = toType.GetConstructor(new Type[] {});
                if(constr == null)
                {
                    throw new InvalidCastException("No parameterless constructor defined for type: " +
                                                   toType.FullName);
                }
                var toList = (IEnumerable)constr.Invoke(new object[] {});
                var addMethod = toType.GetMethod("Add");
                foreach(var fromEnumValue in fromEnumValues)
                {
                    var value = CopyDataMember(toElementType, fromElementType, fromEnumValue);
                    addMethod.Invoke(toList, new[] {value});
                }
                return toList;
            }
            var propType = toType;
            var propValue = Activator.CreateInstance(propType);
            CopyDataMembers(fromValue, propValue);
            return propValue;
        }

        private static bool IsGenericCollection(Type genericType, Type type)
        {
            return type.IsGenericType && (
                                             type.GetGenericTypeDefinition().IsAssignableFrom(genericType) ||
                                             type.GetInterfaces()
                                                 .Where(x => x.IsGenericType)
                                                 .Any(x => x.GetGenericTypeDefinition().IsAssignableFrom(genericType))
                                         );
        }

        #endregion
    }
}
