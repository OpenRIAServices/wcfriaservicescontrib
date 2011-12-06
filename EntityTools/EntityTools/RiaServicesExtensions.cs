using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Client;
using System.Collections;

namespace RiaServicesContrib.Extensions
{
    /// <summary>
    /// Extension methods for WCF RIA Services related objects
    /// </summary>
    public static class RiaServicesExtensions
    {
        /// <summary>
        /// Extracts all DataMember properties of an Entity
        /// </summary>
        /// <param name="entity">Target of extraction</param>
        /// <param name="extractType">Type of extraction</param>
        /// <returns>IDictionary of DataMember values keyed by DataMember name</returns>
        public static IDictionary<string, object> ExtractState(this Entity entity, ExtractType extractType)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            if (extractType == ExtractType.ChangesOnlyState) return ExtractChangedState(entity);

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
        /// Extracts modified DataMember properties of an Entity
        /// </summary>
        /// <param name="entity">Target of extraction</param>
        private static IDictionary<string, object> ExtractChangedState(this Entity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (!entity.HasChanges) return new Dictionary<string, object>();

            Entity originalEntity = entity.GetOriginal();

            Dictionary<string, object> returnDictionary = new Dictionary<string, object>();
            foreach (PropertyInfo currentPropertyInfo in GetDataMembers(entity))
            {
                object originalObject = originalEntity != null ? currentPropertyInfo.GetValue(originalEntity, null) : null;
                object currentObject = currentPropertyInfo.GetValue(entity, null);

                if (originalObject == null && currentObject == null) continue;
                if (currentObject != null && currentObject.Equals(originalObject)) continue;

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
        /// <param name="originalState">IDictionary of OriginalState DataMembers keyed by DataMember name </param>
        /// <param name="modifiedState">IDictionary of modified (aka current) state DataMembers keyed by DataMember name</param>
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
        /// Applies extracted state of data members to entity
        /// </summary>
        /// <param name="entity">Target entity</param>
        /// <param name="state">IDictionary of DataMembers keyed by DataMember name </param>
        /// <param name="stateType">EntityStateType of state to be applied</param>
        public static void ApplyState(this Entity entity, IDictionary<string, object> state, ExtractType stateType)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            StreamingContext dummy = new StreamingContext();
            //Call OnDeserializing to temporarily disable validation
            entity.OnDeserializing(dummy);

            List<PropertyInfo> dataMembers = GetDataMembers(entity);

            ApplyState(entity, state, dataMembers);

            if (stateType == ExtractType.OriginalState)
            {
                ((IChangeTracking)entity).AcceptChanges();
            }

            //Call OnDeserializaed to enable validation
            entity.OnDeserialized(dummy);
        }
        /// <summary>
        /// Extracts the changed state for all changed entities in the DomainContext
        /// </summary>
        /// <param name="context">The DomainContext to query for changes</param>
        /// <returns>The changes for all context controlled entities, in suitable format for ApplyChangedState</returns>
        /// <remarks>
        /// WARNING:  Changes to the DomainContext where Key() values are changed will NOT be
        /// accurately reflected when ApplyChangedState is called.  In fact, changes to key
        /// values will result in phantom additions to the domain context.  This is a known
        /// limitation, so DO NOT use these routines unless key values are assigned at the
        /// client, or where you expect to change primary key values for the Entities.
        /// </remarks>
        public static List<EntityStateSet> ExtractChangedState(this DomainContext context)
        {
            List<EntityStateSet> contextChanges = new List<EntityStateSet>();

            EntityChangeSet changeSet = context.EntityContainer.GetChanges();

            foreach (Entity entity in changeSet.AddedEntities)
            {
                contextChanges.Add(entity.Export());
            }

            foreach (Entity entity in changeSet.ModifiedEntities)
            {
                contextChanges.Add(entity.Export());
            }

            foreach (Entity entity in changeSet.RemovedEntities)
            {
                contextChanges.Add(entity.Export());
            }

            return contextChanges;
        }
        /// <summary>
        /// Applies previously extracted state to the specified context
        /// </summary>
        /// <param name="context">The context to apply the changed state to</param>
        /// <param name="changedState">The changed state to apply [retrieved from ExtractChangedState]</param>
        /// <remarks>
        /// NOTE: This routine will also 'fixup' any EntityCollection(s) that are referenced
        /// by added Entity derivatives where the added child has a bidirectional reference
        /// with the parent Entity.  Other non-directly referenced collections may need manual fixup.
        /// 
        /// For Example: 
        /// Parent -> EntityCollection Child
        /// Child -> Parent
        /// </remarks>
        public static void ApplyChangedState(this DomainContext context, List<EntityStateSet> changedState)
        {
            ApplyChangedState(context, changedState, true);
        }
        /// <summary>
        /// Applies previously extracted state to the specified context
        /// </summary>
        /// <param name="context">The context to apply the changed state to</param>
        /// <param name="changedState">The changed state to apply [retrieved from ExtractChangedState]</param>
        /// <param name="doBasicFixup">
        /// Performs basic fixup of any bi-directional EntityCollections for
        /// newly added entities.
        /// </param>
        /// <remarks>
        /// NOTE: This routine will also 'fixup' any EntityCollection(s) that are referenced
        /// by added Entity derivatives where the added child has a bidirectional reference
        /// with the parent Entity.  Other non-directly referenced collections may need manual fixup.
        /// 
        /// For Example: 
        /// Parent -> EntityCollection Child
        /// Child -> Parent
        /// </remarks>
        public static void ApplyChangedState(this DomainContext context, List<EntityStateSet> changedState, bool doBasicFixup)
        {
            if (changedState == null) throw new ArgumentException("Changed State is required");

            Dictionary<string, EntityStateSet> identityMap = new Dictionary<string, EntityStateSet>();

            changedState.ForEach(change => identityMap.Add((change.ModifiedKey ?? change.OriginalKey).ToString(), change));

            // process update/delete entries
            foreach (EntitySet set in context.EntityContainer.EntitySets)
            {
                foreach (Entity entity in set.OfType<Entity>().ToList())
                {
                    string identity = entity.GetIdentity().ToString();
                    if (identityMap.ContainsKey(identity))
                    {
                        EntityStateSet theSet = identityMap[identity];

                        if (theSet.IsDelete)
                        {
                            set.Remove(entity);
                        }
                        else
                        {
                            ApplyState(entity, theSet.ModifiedState, ExtractType.ModifiedState);
                        }

                        identityMap.Remove(identity);
                    }
                }
            }

            // now do adds
            List<Entity> addedEntities = new List<Entity>();
            foreach (string key in identityMap.Keys)
            {
                EntityStateSet theState = identityMap[key];
                if (theState.IsDelete) continue; // handle this 'just in case'

                Entity newEntity = Activator.CreateInstance(Type.GetType(theState.EntityType)) as Entity;
                ApplyState(newEntity, theState.ModifiedState, ExtractType.ModifiedState);

                // find an appropriate EntitySet to stick the entity into
                EntitySet theSet = null;
                if (context.EntityContainer.TryGetEntitySet(newEntity.GetType(), out theSet))
                {
                    theSet.Add(newEntity);
                    addedEntities.Add(newEntity);
                }

                // do we care if we couldn't find a place to put the add?
                //if (!added)
                //{
                //    throw new InvalidOperationException("Could not find destination EntitySet for new Entity");
                //}
            }

            if (doBasicFixup)
            {
                DoBasicAssociationFixup(addedEntities);
            }
        }
        /// <summary>
        /// Performs 'basic' fixup of EntityCollection references for newly
        /// added entities in the DomainContext.
        /// </summary>
        /// <param name="addedEntities">The entities that have been added to DomainContext.</param>
        private static void DoBasicAssociationFixup(List<Entity> addedEntities)
        {
            // now, need to fixup collection references by manually adding entity to that collection
            Type entityColType = typeof(EntityCollection<>);
            Dictionary<Type, PropertyInfo[]> reflectionCache = new Dictionary<Type, PropertyInfo[]>();
            Dictionary<Type, MethodInfo> collectionAddMethods = new Dictionary<Type, MethodInfo>();
            Dictionary<PropertyInfo, AssociationAttribute> associationAttributes = new Dictionary<PropertyInfo, AssociationAttribute>();

            foreach (Entity newEntity in addedEntities)
            {
                Type entityType = newEntity.GetType();
                if (!reflectionCache.ContainsKey(entityType))
                {
                    reflectionCache[entityType] = entityType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
                }

                PropertyInfo[] props = reflectionCache[entityType];
                foreach (PropertyInfo prop in props.Where(p => typeof(Entity).IsAssignableFrom(p.PropertyType)))
                {
                    if (!associationAttributes.ContainsKey(prop))
                    {
                        associationAttributes[prop] = prop.GetCustomAttributes(typeof(AssociationAttribute), false).OfType<AssociationAttribute>().FirstOrDefault();
                    }

                    AssociationAttribute assocAttr = associationAttributes[prop];
                    if (assocAttr != null)
                    {
                        Entity pkEntity = prop.GetValue(newEntity, null) as Entity;

                        if (pkEntity != null)
                        {
                            // look through the referenced entity for properties that are EntityCollection<newEntity.GetType()>
                            if (!reflectionCache.ContainsKey(prop.PropertyType))
                            {
                                reflectionCache[prop.PropertyType] = prop.PropertyType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
                            }

                            PropertyInfo[] refProps = reflectionCache[prop.PropertyType];
                            foreach (PropertyInfo refProp in refProps)
                            {
                                if (refProp.PropertyType.IsGenericType &&
                                    refProp.PropertyType.GetGenericTypeDefinition() == entityColType &&
                                    refProp.PropertyType.GetGenericArguments().First().IsAssignableFrom(entityType))
                                {
                                    PropertyInfo pkProp = refProps.Where(p => p.Name == assocAttr.OtherKey).First();
                                    PropertyInfo fkProp = props.Where(p => p.Name == assocAttr.ThisKey).First();

                                    object pkValue = pkProp.GetValue(pkEntity, null);
                                    object fkValue = fkProp.GetValue(newEntity, null);

                                    if (pkValue != null && fkValue != null && pkValue.Equals(fkValue))
                                    {
                                        if (!collectionAddMethods.ContainsKey(entityType))
                                        {
                                            collectionAddMethods[entityType] = entityColType.MakeGenericType(entityType).GetMethod("Add");
                                        }

                                        // we have a match, and should add this to the collection
                                        MethodInfo addMethod = collectionAddMethods[entityType];
                                        if (addMethod != null)
                                        {
                                            addMethod.Invoke(refProp.GetValue(pkEntity, null), new object[] { newEntity });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
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
        /// <param name="sourceEntity"></param>
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
        /// Exports the entity into an EntityStateSet
        /// </summary>
        /// <param name="entity">The entity to extract</param>
        /// <returns>NOTE:  For modified entities, only changed state is exported, not all state</returns>
        public static EntityStateSet Export(this Entity entity)
        {
            if (entity == null) throw new ArgumentException("entity");

            EntityStateSet theSet = new EntityStateSet()
            {
                EntityType = entity.GetType().AssemblyQualifiedName
            };

            object entityKey = entity.GetIdentity();
            switch (entity.EntityState)
            {
                case EntityState.Deleted:
                    theSet.IsDelete = true;
                    theSet.OriginalKey = entityKey;
                    theSet.ModifiedKey = entityKey;
                    break;

                case EntityState.New:
                    theSet.ModifiedKey = entityKey;
                    theSet.ModifiedState = ExtractState(entity, ExtractType.ModifiedState);
                    break;

                case EntityState.Modified:
                    theSet.OriginalKey = entityKey;
                    theSet.OriginalState = ExtractState(entity, ExtractType.OriginalState);
                    theSet.ModifiedKey = entityKey;
                    theSet.ModifiedState = ExtractChangedState(entity);
                    break;

                case EntityState.Unmodified:
                    theSet.OriginalKey = entityKey;
                    // yes, it's modified state, but that's purely for performance reasons, 
                    // since the entity is unchanged
                    theSet.OriginalState = ExtractState(entity, ExtractType.ModifiedState);
                    break;

                default:
                    throw new ArgumentException("Cannot export detached entities");
            }

            return theSet;
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
        /// <param name="targetEntity">Entity having state copied to</param>
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
        /// <returns>true if duplicate</returns>
        private static T GetOriginalForced<T>(this T entity) where T : Entity
        {
            T returnEntity = entity.GetOriginal() as T;
            if (returnEntity == null)
                return entity;
            else
                return returnEntity;
        }


        /// <summary>
        /// Extension method which yields all child entities from a certain parent entity.  Set includeDirectChildrenOfEntityBaseType
        /// to true to include direct Entity-inheriting properties (instead of just the EntityCollections)
        /// </summary>
        /// <param name="parent">The parent</param>
        /// <param name="changeSet">The changeset in which to look for entities</param>
        /// <param name="includeDirectChildrenOfEntityBaseType">Defines if direct entity-inheriting properties should be included</param>
        /// <returns></returns>
        public static IEnumerable YieldChildEntities(this Entity parent, EntityChangeSet changeSet,
         bool includeDirectChildrenOfEntityBaseType = false)
        {
            IEnumerable entityCollectionProperties = from p in parent.GetType().GetProperties()
                                                     where
                                                         p.PropertyType.IsGenericType
                                                         && (p.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>)
                                                         )
                                                     select p;

            if (includeDirectChildrenOfEntityBaseType)
            {
                IEnumerable entityProperties = from p in parent.GetType().GetProperties()
                                               where p.PropertyType.BaseType == typeof(Entity)
                                               select p;

                // Get the entities that inherit from "Entity"
                foreach (PropertyInfo property in entityProperties)
                {
                    var entity = (Entity)property.GetValue(parent, null);
                    if (entity != null)
                    {
                        // is this entity in the changeset?
                        if (changeSet.GetEntitiesOfType<Entity>(true, true, true).Contains(entity))
                            yield return entity;
                    }
                }
            }

            // Get the entities from each child collection
            foreach (PropertyInfo property in entityCollectionProperties)
            {
                var collectionType = property.PropertyType.GetGenericArguments()[0];

                var entityCollection = (IEnumerable)property.GetValue(parent, null);
                foreach (Entity entity in entityCollection)
                {
                    foreach (Entity childEntity in entity.YieldChildEntities(changeSet))
                    {
                        yield return childEntity;
                    }

                    if (entity != null)
                    {
                        // is this entity in the changeset?
                        if (changeSet.GetEntitiesOfType<Entity>(true, true, true).Contains(entity))
                            yield return entity;
                    }
                }
            }

        }

        
        /// <summary>
        /// Extension method which yields all child entities from a certain parent entity.  Set includeDirectChildrenOfEntityBaseType
        /// to true to include direct Entity-inheriting properties (instead of just the EntityCollections)
        /// </summary>
        /// <param name="parent">The parent</param>
        /// <param name="container">The container in which to look for entities</param>
        /// <param name="includeDirectChildrenOfEntityBaseType">Defines if direct entity-inheriting properties should be included</param>
        /// <returns></returns>
        public static IEnumerable YieldChildEntities(this Entity parent, EntityContainer container,
            bool includeDirectChildrenOfEntityBaseType = false)
        {
            IEnumerable entityCollectionProperties = from p in parent.GetType().GetProperties()
                                                     where
                                                         p.PropertyType.IsGenericType
                                                         && (p.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>)
                                                         )
                                                     select p;

            if (includeDirectChildrenOfEntityBaseType)
            {
                IEnumerable entityProperties = from p in parent.GetType().GetProperties()
                                               where p.PropertyType.BaseType == typeof(Entity)
                                               select p;

                // Get the entities that inherit from "Entity"
                foreach (PropertyInfo property in entityProperties)
                {
                    // check if an entityset of this type exists in the current container 
                    // (if not, ignore by continuing with the next property)
                    EntitySet set;
                    if (!container.TryGetEntitySet(property.PropertyType, out set))
                    {
                        continue;
                    }

                    var entity = (Entity)property.GetValue(parent, null);
                    if (entity != null)
                        yield return entity;
                }
            }

            // Get the entities from each child collection
            foreach (PropertyInfo property in entityCollectionProperties)
            {
                var collectionType = property.PropertyType.GetGenericArguments()[0];
                EntitySet set;
                if (!container.TryGetEntitySet(collectionType, out set))
                {
                    continue;
                }
                var entityCollection = (IEnumerable)property.GetValue(parent, null);
                foreach (Entity entity in entityCollection)
                {
                    foreach (Entity childEntity in entity.YieldChildEntities(container))
                    {
                        yield return childEntity;
                    }

                    if (entity != null)
                        yield return entity;
                }
            }

        }

        /// <summary>
        /// Extension method which yields all child entities from a certain parent IExtendedEntity.  Set includeDirectChildrenOfEntityBaseType
        /// to true to include direct Entity-inheriting properties (instead of just the EntityCollections)
        /// </summary>
        /// <param name="parent">The parent</param>
        /// <param name="includeDirectChildrenOfEntityBaseType">Defines if direct entity-inheriting properties should be included</param>
        /// <returns></returns>
        public static IEnumerable YieldChildEntities(this IExtendedEntity parent,
            bool includeDirectChildrenOfEntityBaseType = false)
        {
            Entity entityParent = parent as Entity;
            return entityParent.YieldChildEntities(parent.EntitySet.EntityContainer, includeDirectChildrenOfEntityBaseType);            
        }

        /// <summary>
        /// Extension method which deletes all child entities of a certain parent entity from the provided EntityContainer.  
        /// Set includeDirectChildrenOfEntityBaseType to true to include direct Entity-inheriting properties 
        /// (instead of just the children in EntityCollections)
        /// </summary>
        /// <param name="parent">The parent</param>
        /// <param name="container">The container in which to look for entities</param>
        /// <param name="includeDirectChildrenOfEntityBaseType">Defines if direct entity-inheriting properties should be included</param>
        public static void DeleteChildEntities(this Entity parent, EntityContainer container,
            bool includeDirectChildrenOfEntityBaseType = false)
        {
            foreach (var child in parent.YieldChildEntities(container, includeDirectChildrenOfEntityBaseType))
            {
                container.GetEntitySet(child.GetType()).Remove((Entity)child);
            }
        }

        /// <summary>
        /// Extension method which deletes all child entities of a certain parent IExtendedEntity from the provided EntityContainer.  
        /// Set includeDirectChildrenOfEntityBaseType to true to include direct Entity-inheriting properties 
        /// (instead of just the children in EntityCollections)
        /// </summary>
        /// <param name="parent">The parent</param>
        /// <param name="includeDirectChildrenOfEntityBaseType">Defines if direct entity-inheriting properties should be included</param>
        public static void DeleteChildEntities(this IExtendedEntity parent,
            bool includeDirectChildrenOfEntityBaseType = false)
        {
            Entity parentEntity = parent as Entity;
            parentEntity.DeleteChildEntities(parent.EntitySet.EntityContainer, includeDirectChildrenOfEntityBaseType);
            
        }

        /// <summary>
        /// Extension method which detaches all child entities of a certain parent entity from the provided EntityContainer.  
        /// Set includeDirectChildrenOfEntityBaseType to true to include direct Entity-inheriting properties 
        /// (instead of just the children in EntityCollections)
        /// </summary>
        /// <param name="parent">The parent</param>
        /// <param name="container">The container in which to look for entities</param>
        /// <param name="includeDirectChildrenOfEntityBaseType">Defines if direct entity-inheriting properties should be included</param>
        public static void DetachChildEntities(this Entity parent, EntityContainer container,
            bool includeDirectChildrenOfEntityBaseType = false)
        {
            foreach (var child in parent.YieldChildEntities(container, includeDirectChildrenOfEntityBaseType))
            {
                container.GetEntitySet(child.GetType()).Detach((Entity)child);
            }
        }

        /// <summary>
        /// Extension method which detaches all child entities of a certain parent IExtendedEntity from the provided EntityContainer.  
        /// Set includeDirectChildrenOfEntityBaseType to true to include direct Entity-inheriting properties 
        /// (instead of just the children in EntityCollections)
        /// </summary>
        /// <param name="parent">The parent</param>
        /// <param name="container">The container in which to look for entities</param>
        /// <param name="includeDirectChildrenOfEntityBaseType">Defines if direct entity-inheriting properties should be included</param>
        public static void DetachChildEntities(this IExtendedEntity parent, EntityContainer container,
            bool includeDirectChildrenOfEntityBaseType = false)
        {
            Entity parentEntity = parent as Entity;
            parentEntity.DetachChildEntities(parent.EntitySet.EntityContainer, includeDirectChildrenOfEntityBaseType);
            
        }


        /// <summary>
        /// Extension method which rejects all changes on all child entities of a certain parent entity from the provided EntityContainer.  
        /// Set includeDirectChildrenOfEntityBaseType to true to include direct Entity-inheriting properties 
        /// (instead of just the children in EntityCollections)
        /// </summary>
        /// <param name="parent">The parent</param>
        /// <param name="container">The container in which to look for entities</param>
        /// <param name="includeDirectChildrenOfEntityBaseType">Defines if direct entity-inheriting properties should be included</param>
        public static void RejectChangesOnChildEntities(this Entity parent, EntityContainer container,
            bool includeDirectChildrenOfEntityBaseType = false)
        {
            foreach (var child in parent.YieldChildEntities(container, includeDirectChildrenOfEntityBaseType))
            {
                if (child is IRevertibleChangeTracking)
                {
                    ((IRevertibleChangeTracking)child).RejectChanges();
                }
            }
        }

        /// <summary>
        /// Extension method which rejects all changes on all child entities of a certain parent IExtendedEntity from the provided EntityContainer.  
        /// Set includeDirectChildrenOfEntityBaseType to true to include direct Entity-inheriting properties 
        /// (instead of just the children in EntityCollections)
        /// </summary>
        /// <param name="parent">The parent</param>
        /// <param name="container">The container in which to look for entities</param>
        /// <param name="includeDirectChildrenOfEntityBaseType">Defines if direct entity-inheriting properties should be included</param>
        public static void RejectChangesOnChildEntities(this IExtendedEntity parent, EntityContainer container,
            bool includeDirectChildrenOfEntityBaseType = false)
        {
            Entity entityParent = parent as Entity;
            entityParent.RejectChangesOnChildEntities(parent.EntitySet.EntityContainer, includeDirectChildrenOfEntityBaseType);
        }


        /// <summary>
        /// Extension method which rejects all changes on all child entities of a certain parent entity which are available in the provided ChangeSet.  
        /// Set includeDirectChildrenOfEntityBaseType to true to include direct Entity-inheriting properties 
        /// (instead of just the children in EntityCollections)
        /// </summary>
        /// <param name="parent">The parent</param>
        /// <param name="changeSet">The container in which to look for entities</param>
        /// <param name="includeDirectChildrenOfEntityBaseType">Defines if direct entity-inheriting properties should be included</param>
        public static void RejectChangesOnChildEntities(this Entity parent, EntityChangeSet changeSet,
            bool includeDirectChildrenOfEntityBaseType = false)
        {
            foreach (var child in parent.YieldChildEntities(changeSet, includeDirectChildrenOfEntityBaseType))
            {
                if (child is IRevertibleChangeTracking)
                {
                    ((IRevertibleChangeTracking)child).RejectChanges();
                }
            }
        }



        /// <summary>
        /// Extension method to attach a range of entities to an EntitySet
        /// </summary>
        /// <typeparam name="T">The type of Entity to attach</typeparam>
        /// <param name="entitySet">The EntitySet to attach the entities to</param>
        /// <param name="entitiesToAttach">The entities to attach</param>
        public static void AttachRange<T>(this EntitySet<T> entitySet, IEnumerable<T> entitiesToAttach) where T : Entity
        {
            entitiesToAttach.ToList<T>().ForEach(e => entitySet.Attach(e));
        }

        /// <summary>
        /// Extension method to get entities of a certain type from an EntityChangeSet, across added, modified
        /// and/or deleted entities
        /// </summary>
        /// <typeparam name="T">The type of the entities to get</typeparam>
        /// <param name="changeSet">The EntityChangeSet to get the entities from</param>
        /// <param name="includeAddedEntites">Defines if added entities should be included</param>
        /// <param name="includeModifiedEntities">Defines if modified entities should be included</param>
        /// <param name="includeDeletedEntities">Defines if deleted entities should be included</param>
        /// <returns></returns>
        public static IEnumerable<T> GetEntitiesOfType<T>(this EntityChangeSet changeSet, bool includeAddedEntites = true,
            bool includeModifiedEntities = true, bool includeDeletedEntities = false) where T : Entity
        {
            List<T> returnList = new List<T>();

            if (includeAddedEntites)
            {
                var subList = changeSet.AddedEntities.OfType<T>();
                returnList.AddRange(subList);
            }

            if (includeModifiedEntities)
            {
                var subList = changeSet.ModifiedEntities.OfType<T>();
                returnList.AddRange(subList);
            }

            if (includeDeletedEntities)
            {
                var subList = changeSet.RemovedEntities.OfType<T>();
                returnList.AddRange(subList);
            }

            return returnList;
        }

        /// <summary>
        /// Extension method to check for validation errors on a submit operation (can be used instead of looping
        /// through the EntitiesInError collection when you need to execute certain actions when validation errors occur)
        /// </summary>
        /// <param name="submitOperation">The SubmitOperation</param>
        /// <returns></returns>
        public static bool HasValidationErrors(this System.ServiceModel.DomainServices.Client.SubmitOperation submitOperation)
        {
            if (submitOperation.HasError)
            {
                foreach (var entityInError in submitOperation.EntitiesInError)
                {
                    if (entityInError.HasValidationErrors)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Extension method to check for validation errors on an entity of a provided type T on a submit operation (can be used instead of looping
        /// through the EntitiesInError collection when you need to execute certain actions when validation errors occur)
        /// </summary>
        /// <param name="submitOperation">The SubmitOperation</param>
        /// <returns></returns>
        public static bool HasValidationErrors<T>(this System.ServiceModel.DomainServices.Client.SubmitOperation submitOperation) where T:Entity
        {
            if (submitOperation.HasError)
            {
                foreach (var entityInError in submitOperation.EntitiesInError.OfType<T>())
                {
                    if (entityInError.HasValidationErrors)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Extension method to get all validation errors (can be used instead of looping
        /// through the EntitiesInError collection)
        /// </summary>
        /// <param name="submitOperation">The SubmitOperation</param>
        /// <returns></returns>
        public static IEnumerable<ValidationResult> GetAllValidationErrors(this System.ServiceModel.DomainServices.Client.SubmitOperation submitOperation)
        {
            List<ValidationResult> valResults = new List<ValidationResult>();

            if (submitOperation.HasError)
            {
                foreach (var entityInError in submitOperation.EntitiesInError)
                {
                    if (entityInError.HasValidationErrors)
                        (entityInError.ValidationErrors as List<ValidationResult>).ForEach((vr) => valResults.Add(vr));
                }
            }

            return valResults;
        }
                
        /// <summary>
        /// Extension method to get all validation errors that occurred on entities of a provided type T (can be used instead of looping
        /// through the EntitiesInError collection)
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <param name="submitOperation">The SubmitOperation</param>
        /// <returns></returns>
        public static IEnumerable<ValidationResult> GetAllValidationErrors<T>(this System.ServiceModel.DomainServices.Client.SubmitOperation submitOperation) where T:Entity
        {
            List<ValidationResult> valResults = new List<ValidationResult>();

            if (submitOperation.HasError)
            {
                foreach (var entityInError in submitOperation.EntitiesInError.OfType<T>())
                {
                    if (entityInError.HasValidationErrors)
                        (entityInError.ValidationErrors as List<ValidationResult>).ForEach((vr) => valResults.Add(vr));
                }
            }

            return valResults;
        }

    }
}
