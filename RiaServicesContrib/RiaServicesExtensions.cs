using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Client;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace RiaServicesContrib.Extensions
{
    public static class RiaServicesExtensions
    {
        /// <summary>
        /// Extracts all DataMember properties of an Entity
        /// </summary>
        /// <param name="entity">Target of extraction</param>
        /// <param name="extractType">Type of extraction</param>
        /// <returns>IDictionary<string,object> of DataMember values keyed by DataMember name</returns>
        public static IDictionary<string, object> ExtractState(this Entity entity, ExtractType extractType)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Entity extractEntity;
            if (extractType == ExtractType.OriginalState && entity.HasChanges)
                extractEntity = entity.GetOriginalForced();
            else
                extractEntity = entity;

            Dictionary<string, object> returnDictionary = new Dictionary<string, object>();
            foreach (PropertyInfo currentPropertyInfo in GetDataMembers(extractEntity))
            {
                object currentObject = currentPropertyInfo.GetValue(extractEntity, null);
                returnDictionary[currentPropertyInfo.Name] = currentObject;
            }
            return returnDictionary;
        }
        /// <summary>
        /// Applies state of sourceEntity to current state of entity
        /// </summary>
        /// <remarks>Original EntityStates are not checked. </remarks>
        /// <param name="targetEntity">Entity having state copied to</param>
        /// <param name="sourceEntity">Entity being copied from</param>
        public static void ApplyState<T>(this T targetEntity, T sourceEntity) where T : Entity
        {
            if (targetEntity == null) throw new ArgumentNullException("targetEntity");
            if (sourceEntity == null) throw new ArgumentNullException("sourceEntity");
            StreamingContext dummy = new StreamingContext();
            //Call OnDeserializing to temporarily disable validation
            targetEntity.OnDeserializing(dummy);
            
            List<PropertyInfo> dataMembers = GetDataMembers(targetEntity);
            if (sourceEntity.HasChanges)
            {
                ApplyState(targetEntity, sourceEntity.GetOriginalForced(), dataMembers);
                ((IChangeTracking)targetEntity).AcceptChanges();
            }
            
                ApplyState(targetEntity, sourceEntity, dataMembers);
            
            //Call OnDeserializaed to enable validation
            targetEntity.OnDeserialized(dummy);
        }
        /// <summary>
        /// Applies extracted state of data members to entity
        /// </summary>
        /// <param name="entity">Target entity</param>
        /// <param name="originalState">IDictionary<string,object> of OriginalState DataMembers keyed by DataMember name </param>
        /// <param name="modifiedState">IDictionary<string,object> of modified (aka current) state DataMembers keyed by DataMember name</param>
        public static void ApplyState(this Entity entity, IDictionary<string, object> originalState, IDictionary<string, object> modifiedState)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            
            StreamingContext dummy = new StreamingContext();
            //Call OnDeserializing to temporarily disable validation
            entity.OnDeserializing(dummy);
            if (entity.EntityState != EntityState.New && originalState == null)
                throw new InvalidOperationException("Entity must be in New state if no originalState was supplied.");
            List<PropertyInfo> dataMembers = GetDataMembers(entity);
            if (originalState != null)
            {
                ApplyState(entity, originalState, dataMembers);
                ((IChangeTracking)entity).AcceptChanges();
            }
            if (modifiedState != null)
            {
                ApplyState(entity, modifiedState, dataMembers);
            }
            //Call OnDeserializaed to enable validation
            entity.OnDeserialized(dummy);
        }
        /// <summary>
        /// Imports an a list of EntityStateSet objects into the target
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">Target EntitySet</param>
        /// <param name="stateSet">Source list of EntityStateSet objects</param>
        /// <returns>IEnumerable of imported entities</returns>
        /// <remarks>Uses a LoadBehavior of RefreshCurrent</remarks>
        public static IEnumerable<T> Import<T>(this EntitySet<T> collection, IList<EntityStateSet> stateSet) where T : Entity, new()
        {
            return collection.Import(stateSet, LoadBehavior.RefreshCurrent);
        }
        /// <summary>
        /// Imports an a list of EntityStateSet objects into the target
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">Target EntitySet</param>
        /// <param name="stateSet">Source list of EntityStateSet objects</param>
        /// <param name="loadBehavior">Load behavior used during load</param>
        /// <returns>IEnumerable of imported entities</returns>
        public static IEnumerable<T> Import<T>(this EntitySet<T> collection, IList<EntityStateSet> stateSet, LoadBehavior loadBehavior) where T : Entity, new()
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (stateSet == null) throw new ArgumentNullException("stateSet");
            List<T> loadedEntities = new List<T>();
            Dictionary<object, T> identityCache = new Dictionary<object, T>();
            foreach (T currentEntity in collection)
            {
                identityCache.Add(currentEntity.GetIdentity(), currentEntity);
            }
            foreach (EntityStateSet currentStateSet in stateSet)
            {
                T existingEntity;
                identityCache.TryGetValue(currentStateSet.OriginalKey, out existingEntity);
                if (existingEntity == null && currentStateSet.ModifiedKey != null)
                    identityCache.TryGetValue(currentStateSet.ModifiedKey, out existingEntity);
                if (existingEntity == null)
                {
                    T newEntity = new T();
                    if (currentStateSet.ModifiedState != null && currentStateSet.OriginalState != null)
                        collection.Attach(newEntity);
                    else if (currentStateSet.ModifiedState != null)
                        collection.Add(newEntity);

                    newEntity.ApplyState(currentStateSet.OriginalState, currentStateSet.ModifiedState);
                    if (currentStateSet.ModifiedState == null)
                        collection.Attach(newEntity);
                    if (currentStateSet.IsDelete)
                    {
                        collection.Attach(newEntity);
                        collection.Remove(newEntity);
                    }
                    else
                        loadedEntities.Add(newEntity);
                }
                else if (loadBehavior == LoadBehavior.RefreshCurrent)
                {
                    if (currentStateSet.IsDelete)
                        collection.Remove(existingEntity);
                    else
                    {
                        existingEntity.ApplyState(currentStateSet.OriginalState, currentStateSet.ModifiedState);
                        loadedEntities.Add(existingEntity);
                    }
                }
            }
            return new ReadOnlyCollection<T>(loadedEntities);
        }

        /// <summary>
        /// Does a direct clone of one entity to another.
        /// The resulting EntityState of the targetEntity is not guaranteed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="originalEntity"></param>
        /// <param name="targetEntity"></param>
        public static void Clone<T>(this T sourceEntity, T targetEntity) where T:Entity
        {
            StreamingContext dummy = new StreamingContext();
            //Call OnDeserializing to temporarily disable validation
            targetEntity.OnDeserializing(dummy);
            List<PropertyInfo> dataMembers = GetDataMembers(sourceEntity);
            if (sourceEntity.HasChanges)
            {
                ApplyState(targetEntity, sourceEntity.GetOriginalForced(), dataMembers);
                ((IChangeTracking)targetEntity).AcceptChanges();            
            }            
            ApplyState(targetEntity, sourceEntity, dataMembers);
           
            //Call OnDeserializaed to enable validation
            targetEntity.OnDeserialized(dummy);
        }

        /// <summary>
        /// Clones a single entity into an EntitySet.
        /// </summary>
        /// <remarks>Does not check for duplicate entities. Detached sources are not allowed.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="sourceEntity"></param>
        public static T Clone<T>(this EntitySet<T> collection, T sourceEntity) where T : Entity, new()
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (sourceEntity == null) throw new ArgumentNullException("sourceEntity");
            StreamingContext dummy = new StreamingContext();
            T targetEntity = new T();
            //Call OnDeserializing to temporarily disable validation
            targetEntity.OnDeserializing(dummy);
            List<PropertyInfo> dataMembers = GetDataMembers(sourceEntity);
            if (sourceEntity.HasChanges)
                ApplyState(targetEntity, sourceEntity.GetOriginalForced(), dataMembers);

            if (sourceEntity.EntityState == EntityState.Deleted)
            {
                collection.Add(targetEntity);
                collection.Remove(targetEntity);
            }
            else if (sourceEntity.EntityState == EntityState.Detached)
            {
                throw new ArgumentException("Detached entities cannot be cloned into an EntitySet", "sourceEntity");
            }
            else if (sourceEntity.EntityState == EntityState.Modified)
            {
                collection.Attach(targetEntity);
                ApplyState(targetEntity, sourceEntity, dataMembers);
            }
            else if (sourceEntity.EntityState == EntityState.New)
            {
                ApplyState(targetEntity, sourceEntity, dataMembers);
                collection.Add(targetEntity);
            }
            else if (sourceEntity.EntityState == EntityState.Unmodified)
            {
                ApplyState(targetEntity, sourceEntity, dataMembers);
                collection.Attach(targetEntity);
            }
            //Call OnDeserializaed to enable validation
            targetEntity.OnDeserialized(dummy);
            return targetEntity;
        }
        /// <summary>
        /// Clones an IEnumerable of entities into an EntitySet.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="collection">Target EntitySet</param>
        /// <param name="source">Source entities</param>
        /// <param name="loadBehavior">Load behavior used during clone</param>
        /// <returns></returns>
        public static IEnumerable<T> Clone<T>(this EntitySet<T> collection, IEnumerable<T> source, LoadBehavior loadBehavior) where T : Entity, new()
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (source == null) throw new ArgumentNullException("source");
            List<T> clonedEntities = new List<T>();
            Dictionary<object, T> identityCache = new Dictionary<object, T>();

            foreach (T currentEntity in collection)
            {
                identityCache.Add(currentEntity.GetIdentity(), currentEntity);
            }
            foreach (T currentSourceEntity in source)
            {
                T existingEntity = null;
                if (currentSourceEntity.HasChanges)
                    identityCache.TryGetValue(currentSourceEntity.GetOriginalForced().GetIdentity(), out existingEntity);
                if (existingEntity == null)
                    identityCache.TryGetValue(currentSourceEntity.GetIdentity(), out existingEntity);
                if (existingEntity == null)
                {
                    collection.Clone(currentSourceEntity);
                }
                else if (loadBehavior == LoadBehavior.RefreshCurrent)
                {
                    if (currentSourceEntity.EntityState == EntityState.Deleted)
                        collection.Remove(existingEntity);
                    else if (currentSourceEntity.EntityState == EntityState.New)
                    {
                        if (existingEntity.EntityState == EntityState.New)
                            ApplyState(existingEntity, currentSourceEntity);
                        else
                        {
                            collection.Remove(existingEntity);
                            collection.Clone(currentSourceEntity);
                        }
                    }
                    else if (currentSourceEntity.EntityState == EntityState.Modified)
                    {
                        ApplyState(existingEntity, currentSourceEntity.GetOriginalForced());
                        ((IChangeTracking)existingEntity).AcceptChanges();
                        ApplyState(existingEntity, currentSourceEntity);
                    }
                    else if (currentSourceEntity.EntityState == EntityState.Unmodified)
                    {                    
                        ApplyState(existingEntity, currentSourceEntity);
                        ((IChangeTracking)existingEntity).AcceptChanges();
                    }
                }
            }
            return new ReadOnlyCollection<T>(clonedEntities);
        }

        /// <summary>
        /// Converts list of EntityStateSets into Detached entities.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="stateSet">Source IList of EntityStateSets</param>
        /// <returns>IEnumerable of T</returns>
        /// <remarks>Any EntityStateSet which has a ModifiedState will ignore the OriginalState</remarks>
        public static IEnumerable<T> ToEntities<T>(this IList<EntityStateSet> stateSet) where T : Entity, new()
        {
            if (stateSet == null) throw new ArgumentNullException("stateSet");
            List<T> returnList = new List<T>();

            foreach (EntityStateSet currentStateSet in stateSet)
            {
                T newEntity = new T();
                if (currentStateSet.ModifiedState == null)
                    newEntity.ApplyState(currentStateSet.OriginalState, null);
                else
                    newEntity.ApplyState(currentStateSet.ModifiedState, null);
                returnList.Add(newEntity);
            }
            return returnList;
        }

        /// <summary>
        /// Creates an IList of EntityStateSet based on a source IEnumerable of entities
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="collection">Source collection of entities</param>
        /// <returns>IList of EntityStateSet</returns>
        public static IList<EntityStateSet> Export<T>(this IEnumerable<T> collection) where T : Entity, new()
        {
            if (collection == null) throw new ArgumentNullException("collection");
            List<EntityStateSet> stateList = new List<EntityStateSet>();
            foreach (Entity currentEntity in collection)
            {
                EntityStateSet newStateSet = new EntityStateSet();

                if (currentEntity.HasChanges)
                {
                    newStateSet.OriginalKey = currentEntity.GetOriginalForced().GetIdentity();
                    object modifiedKey = currentEntity.GetIdentity();
                    if (newStateSet.OriginalKey != modifiedKey)
                        newStateSet.ModifiedKey = modifiedKey;
                }
                else
                    newStateSet.OriginalKey = currentEntity.GetIdentity();

                if (currentEntity.EntityState == EntityState.New)
                {
                    newStateSet.ModifiedState = currentEntity.ExtractState(ExtractType.ModifiedState);
                }
                else
                {
                    newStateSet.OriginalState = currentEntity.ExtractState(ExtractType.OriginalState);
                    if (currentEntity.HasChanges)
                        newStateSet.ModifiedState = currentEntity.ExtractState(ExtractType.ModifiedState);
                }

                newStateSet.IsDelete = currentEntity.EntityState == EntityState.Deleted;
                stateList.Add(newStateSet);
            }
            return stateList;
        }
        /// <summary>
        /// Applies IDictionary of state to an enity
        /// </summary>
        /// <param name="entity">Entity having state copied to</param>
        /// <param name="state">IDictionary of property values</param>
        /// <param name="dataMembers">List of properties being copied </param>
        private static void ApplyState(Entity entity, IDictionary<string, object> state, List<PropertyInfo> dataMembers)
        {
            foreach (PropertyInfo currentPropertyInfo in dataMembers)
            {
                object currentProperty;
                if (state.TryGetValue(currentPropertyInfo.Name, out currentProperty))
                {
                    currentPropertyInfo.SetValue(entity, currentProperty, null);
                }
            }
        }
        /// <summary>
        /// Applies current state of sourceEntity to current state of entity
        /// </summary>
        /// <param name="entity">Entity having state copied to</param>
        /// <param name="sourceEntity">Entity being copied from</param>
        /// <param name="dataMembers">List of properties being copied</param>
        private static void ApplyState<T>(T targetEntity, T sourceEntity, List<PropertyInfo> dataMembers) where T:Entity
        {
            foreach (PropertyInfo currentPropertyInfo in dataMembers)
            {
                currentPropertyInfo.SetValue(targetEntity, currentPropertyInfo.GetValue(sourceEntity, null), null);                
            }
        }

        //This caches the reflected information about the entities for performance
        private static Dictionary<Type, List<PropertyInfo>> reflectionCache = new Dictionary<Type, List<PropertyInfo>>();
        //This gets the DataMembers
        private static List<PropertyInfo> GetDataMembers(Entity entity)
        {
            if (!reflectionCache.ContainsKey(entity.GetType()))
            {
                BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
                var qry = from p in entity.GetType().GetProperties(bindingAttr)
                          where p.GetCustomAttributes(typeof(DataMemberAttribute), true).Length > 0
                          && p.GetSetMethod() != null
                          select p;
                reflectionCache.Add(entity.GetType(), qry.ToList());
            }
            return reflectionCache[entity.GetType()];
        }

        /// <summary>
        /// Returns true if the non Key and Timestamp properties of two entities are the same
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="compareEntity"></param>
        /// <returns>true if duplicate</returns>
        public static bool IsDuplicate<T>(this T entity, T compareEntity) where T : Entity
        {
            foreach (PropertyInfo currentPropertyInfo in GetDataMembers(entity))
            {
                if (!(currentPropertyInfo.GetCustomAttributes(typeof(KeyAttribute), true).Any() ||
                    currentPropertyInfo.GetCustomAttributes(typeof(TimestampAttribute), true).Any() ||
                    currentPropertyInfo.Name == "Version"))
                {
                    object entityObject = currentPropertyInfo.GetValue(entity, null);
                    object compareObject = currentPropertyInfo.GetValue(compareEntity, null);
                    if (!((entityObject == null && compareObject == null) || (entityObject != null && compareObject != null && entityObject.Equals(compareObject))))
                        return false;
                }
            }
            return true;
        }
        /// <summary>
        /// If the original entity doesn't have an original state, returns the current entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="compareEntity"></param>
        /// <returns>true if duplicate</returns>
        private static T GetOriginalForced<T>(this T entity) where T : Entity
        {
            T returnEntity = entity.GetOriginal() as T;
            if (returnEntity == null)
                return entity;
            else
                return returnEntity;
        }
 
    }
}
