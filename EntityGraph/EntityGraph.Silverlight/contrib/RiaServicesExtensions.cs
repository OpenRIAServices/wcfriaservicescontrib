namespace RiaServicesContrib.DomainServices.Client.contrib
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.ServiceModel.DomainServices.Client;
    using System.Collections.ObjectModel;

    public static class RiaServicesExtensions
    {


        public static IDictionary<string, object> ExtractState(this Entity entity, ExtractType extractType)
        {
            Entity extractEntity;
            if (extractType == ExtractType.OriginalState && entity.HasChanges)
                extractEntity = entity.GetOriginal();
            else
                extractEntity = entity;

            Dictionary<string, object> returnDictionary = new Dictionary<string, object>();
            foreach (PropertyInfo currentPropertyInfo in GetDataMembers(extractEntity))
            {
                //if(currentPropertyInfo.IsDefined(typeof(KeyAttribute), true))
                //    continue;
                object currentObject = currentPropertyInfo.GetValue(extractEntity, null);
                returnDictionary[currentPropertyInfo.Name] = currentObject;
            }
            return returnDictionary;
        }

        public static void ApplyState(this Entity entity, IDictionary<string, object> originalState, IDictionary<string, object> modifiedState)
        {
            
            StreamingContext dummy = new StreamingContext();
            //Call OnDeserializing to temporarily disable validation
            entity.OnDeserializing(dummy);
            if (entity.EntityState != EntityState.New && originalState == null)
                throw new InvalidOperationException("Entity must be in New state if no originalState was supplied.");
            PropertyInfo[] dataMembers = GetDataMembers(entity);
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

        public static IEnumerable<T> Import<T>(this EntitySet<T> collection, IList<EntityStateSet> stateSet) where T : Entity, new()
        {
            return collection.Import(stateSet, LoadBehavior.RefreshCurrent);
        }
        public static IEnumerable<T> Import<T>(this EntitySet<T> collection, IList<EntityStateSet> stateSet, LoadBehavior loadBehavior) where T : Entity, new()
        {
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
                    if (currentStateSet.ModifiedState != null)
                        collection.Attach(newEntity);
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


        public static IEnumerable<T> ToEntities<T>(this IList<EntityStateSet> stateSet) where T : Entity, new()
        {
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


        public static IList<EntityStateSet> Export<T>(this IEnumerable<T> collection) where T : Entity, new()
        {
            List<EntityStateSet> stateList = new List<EntityStateSet>();
            foreach (Entity currentEntity in collection)
            {
                EntityStateSet newStateSet = new EntityStateSet();

                if (currentEntity.HasChanges)
                {
                    newStateSet.OriginalKey = currentEntity.GetOriginal().GetIdentity();
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

        private static void ApplyState(Entity entity, IDictionary<string, object> state, PropertyInfo[] dataMembers)
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

        private static PropertyInfo[] GetDataMembers(Entity entity)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qry = from p in entity.GetType().GetProperties(bindingAttr)
                      where p.GetCustomAttributes(typeof(DataMemberAttribute), true).Length > 0
                      && p.GetSetMethod() != null
                      select p;
            return qry.ToArray();
        }
    }
}
