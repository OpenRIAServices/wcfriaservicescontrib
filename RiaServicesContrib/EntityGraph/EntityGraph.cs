using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Client;

namespace RIAEntity.Extensions
{
    /// <summary>
    /// This class implements functions for entity graphs. An entity graph is a collection of associated entities for 
    /// which the associations have been decorated with the "EntityGraphAttribute" attribute.
    /// 
    /// The following functions are supported:
    /// - Entity.GraphOperation(Func<Entity,Entity> action). This is a generic visitor method that executes 'action' on every node
    /// - Entity.Clone(). Given an entity "e", "e.Clone()" creates a clone entity and copies all properties of "e" which are marked with 
    ///   the "EntityGraphAttribute". Associations and association sets are copied or cloned depending on whether they are marked with the 
    ///   "EntityGraphAttribute". If an association "a" is not marked, then the value "e.a" is copied to the clone, othewise "a" is cloned 
    ///   itself and then assigned to the property "a" of the clone.
    /// - EntitySet.DetachEntityGraph(). Given an entity "e", the method "Es.DetachEntityGraph(e)" will detach the complete entity graph 
    ///   of "e" from the entity container who owns the entity set "Es".
    /// - EntitySet.DeleteEntityGraph(). Not implemented yet, but signature already defined
    /// 
    /// Each function can indicate a particular entity graph to operate on by either providing a type parameter that specifies a sub type 
    /// of EntityGraphAttribute, or by specifying a name of the entity graph. Both mechanisms can be combined.
    /// With these mechanism multiple (possibly) overlapping entity graphs can be defined, that can operated on individually.
    /// </summary>
    public static class EntityGraphOperations
    {
        #region GraphOperation
        /// <summary>
        /// CCC Extension method that implementes a generic traversal over an entity graph (defined by 
        /// associations marked with the 'EntityGraphAttribute' attribute) and applies 'action' to each visited node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T GraphOperation<T>(this T entity, Func<Entity, Entity> action) where T : Entity
        {
            return entity.GraphOperation<T, EntityGraphAttribute>(null, action);
        }
        /// <summary>
        /// CCC Extension method that implementes a generic traversal over an entity graph (defined by 
        /// associations marked with the 'EntityGraphAttribute' attribute and with name 'entityGraphName') and applies 'action' to each visited node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="entityGraphName">Only entity graphs with given name are considered</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T GraphOperation<T>(this T entity, string entityGraphName, Func<Entity, Entity> action) where T : Entity
        {
            return entity.GraphOperation<T, EntityGraphAttribute>(entityGraphName, action);
        }
        /// <summary>
        /// CCC Extension method that implementes a generic traversal over an entity graph (defined by 
        /// associations marked with an entity graph attibute of type 'GraphType') and applies 'action' to each visited node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="GraphType"></typeparam>
        /// <param name="entity"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T GraphOperation<T, GraphType>(this T entity, Func<Entity, Entity> action)
            where T : Entity
            where GraphType : EntityGraphAttribute
        {
            return entity.GraphOperation<T, GraphType>(null, action);
        }
        /// <summary>
        /// CCC Extension method that implementes a generic traversal over an entity graph (defined by 
        /// associations marked with an entity graph attibute of type 'GraphType' and with name 'entityGraphName') and applies 
        /// 'action' to each visited node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="GraphType"></typeparam>
        /// <param name="entity"></param>
        /// <param name="entityGraphName">Only entity graphs with given name are considered</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T GraphOperation<T, GraphType>(this T entity, string entityGraphName, Func<Entity, Entity> action)
            where T : Entity
            where GraphType : EntityGraphAttribute
        {
            EntityGraph<Entity> graph = new EntityGraph<Entity>();

            Func<EntityGraphAttribute, bool> match = entityGraph => entityGraph is GraphType && (entityGraphName == null || entityGraphName == entityGraph.Name);
            GetEntityGraph(entity, match, graph);
            var nodeMap = new Dictionary<Entity, Entity>();

            nodeMap = graph.Nodes.Aggregate(nodeMap, (nm, graphNode) =>
            {
                nm.Add(graphNode.Node, action(graphNode.Node));
                return nm;
            }
            );
            BuildEntityGraph(nodeMap, graph);
            return nodeMap[entity] as T;
        }
        #endregion

        #region DetachEntityGraph
        /// <summary>
        /// CCC Extension method that detaches a graph of associated entities from their respective entity sets.
        /// The graph of entities is defined by associations which are marked with an entity graph attribute of type 'GraphType'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitySet"></param>
        /// <param name="entity"></param>
        public static void DetachEntityGraph<T>(this EntitySet<T> entitySet, Entity entity) where T : Entity
        {
            entitySet.DetachEntityGraph<T, EntityGraphAttribute>(entity, null);
        }
        /// <summary>
        /// CCC Extension method that detaches a graph of associated entities from their respective entity sets.
        /// The graph of entities is defined by associations which are marked with an entity graph attribute of type 'GraphType' and 
        /// with name 'entityGraphName'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitySet"></param>
        /// <param name="entity"></param>
        /// <param name="entityGraphName">Only entity graphs with given name are considered</param>
        public static void DetachEntityGraph<T>(this EntitySet<T> entitySet, Entity entity, string entityGraphName) where T : Entity
        {
            entitySet.DetachEntityGraph<T, EntityGraphAttribute>(entity, entityGraphName);
        }
        /// <summary>
        /// CCC Extension method that detaches a graph of associated entities from their respective entity sets.
        /// The graph of entities is defined by associations which are marked with an entity graph attribute of type 'GraphType'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="GraphType"></typeparam>
        /// <param name="entitySet"></param>
        /// <param name="entity"></param>
        public static void DetachEntityGraph<T, GraphType>(this EntitySet<T> entitySet, Entity entity)
            where T : Entity
            where GraphType : EntityGraphAttribute
        {
            entitySet.DetachEntityGraph<T, GraphType>(entity, null);
        }
        /// <summary>
        /// CCC Extension method that detaches a graph of associated entities from their respective entity sets.
        /// The graph of entities is defined by associations which are marked with an entity graph attribute of type 'GraphType' and
        /// with name 'entityGraphName'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="GraphType"></typeparam>
        /// <param name="entitySet"></param>
        /// <param name="entity"></param>
        /// <param name="entityGraphName">Only entity graphs with given name are considered</param>
        public static void DetachEntityGraph<T, GraphType>(this EntitySet<T> entitySet, Entity entity, string entityGraphName)
            where T : Entity
            where GraphType : EntityGraphAttribute
        {
            var entityContainer = entitySet.EntityContainer;
            entity.GraphOperation<Entity, GraphType>(entityGraphName, e => DetachAction(e, entityContainer));
        }

        private static Entity DetachAction(Entity entity, EntityContainer entityContainer)
        {
            var entitySetToDetachFrom = entityContainer.GetEntitySet(entity.GetType());
            entitySetToDetachFrom.Detach(entity);
            return entity;
        }
        #endregion

        #region Clone
        /// <summary>
        /// CCC extension method that clones an entity and, recursively, all its associations that are marked with 
        /// the 'EntityGraphAttribute' attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T Clone<T>(this T entity) where T : Entity
        {
            return entity.Clone<T, EntityGraphAttribute>(null);
        }
        /// <summary>
        /// CCC extension method that clones an entity and, recursively, all its associations that are marked with 
        /// the 'EntityGraphAttribute' attribute and name 'entityGraphName'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="entityGraphName">Only entity graphs with given name are considered</param>
        /// <returns></returns>
        public static T Clone<T>(this T entity, string entityGraphName) where T : Entity
        {
            return entity.Clone<T, EntityGraphAttribute>(entityGraphName);
        }
        /// <summary>
        /// CCC extension method that clones an entity and, recursively, all its associations that are marked with 
        /// an entity graph attribute of type 'GraphType'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="GraphType"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T Clone<T, GraphType>(this T entity)
            where T : Entity
            where GraphType : EntityGraphAttribute
        {
            return entity.Clone<T, EntityGraphAttribute>(null);
        }
        /// <summary>
        /// CCC extension method that clones an entity and, recursively, all its associations that are marked with 
        /// an entity graph attribute of type 'GraphType' and name 'entityGraphName'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="GraphType"></typeparam>
        /// <param name="entity"></param>
        /// <param name="entityGraphName">Only entity graphs with given name are considered</param>
        /// <returns></returns>
        public static T Clone<T, GraphType>(this T entity, string entityGraphName)
            where T : Entity
            where GraphType : EntityGraphAttribute
        {
            return entity.GraphOperation<T, GraphType>(entityGraphName, CloneDataMembers);
        }

        private static T CloneDataMembers<T>(T entity) where T : Entity
        {
            // Create new object of type T (or subtype) using reflection and inspecting the concrete 
            // type of the entity to clone.
            T clone = (T)Activator.CreateInstance(entity.GetType());

            // Clone DataMember properties
            foreach (PropertyInfo currentPropertyInfo in GetDataMembers(entity))
            {
                object currentObject = currentPropertyInfo.GetValue(entity, null);
                currentPropertyInfo.SetValue(clone, currentObject, null);
            }
            return clone;
        }
        #endregion

        #region DeleteEntityGraph
        /// <summary>
        /// CCC Extension method that deletes a graph of associated entities from their respective entity sets.
        /// The graph of entities is defined by the 'EntityGraphAttribute' attribute.
        /// </summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitySet"></param>
        /// <param name="entity"></param>
        public static void DeleteEntityGraph<T>(this EntitySet<T> entitySet, Entity entity) where T : Entity
        {
            entitySet.DeleteEntityGraph<T, EntityGraphAttribute>(entity, null);
        }
        /// <summary>
        /// CCC Extension method that deletes a graph of associated entities from their respective entity sets.
        /// The graph of entities is defined by the 'EntityGraphAttribute' attribute and name 'entityGraphName'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitySet"></param>
        /// <param name="entity"></param>
        /// <param name="entityGraphName">Only entity graphs with given name are considered</param>
        public static void DeleteEntityGraph<T>(this EntitySet<T> entitySet, Entity entity, string entityGraphName) where T : Entity
        {
            entitySet.DeleteEntityGraph<T, EntityGraphAttribute>(entity, entityGraphName);
        }
        /// <summary>
        /// CCC Extension method that deletes a graph of associated entities from their respective entity sets.
        /// The graph of entities is defined by associations which are marked with an entity graph attribute of type 'GraphType'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="GraphType"></typeparam>
        /// <param name="entitySet"></param>
        /// <param name="entity"></param>
        public static void DeleteEntityGraph<T, GraphType>(this EntitySet<T> entitySet, Entity entity)
            where T : Entity
            where GraphType : EntityGraphAttribute
        {
            entitySet.DeleteEntityGraph<T, GraphType>(entity, null);
        }
        /// <summary>
        /// CCC Extension method that deletes a graph of associated entities from their respective entity sets.
        /// The graph of entities is defined by associations which are marked with an entity graph attribute of type 'GraphType' and
        /// with name 'entityGraphName'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="GraphType"></typeparam>
        /// <param name="entitySet"></param>
        /// <param name="entity"></param>
        /// <param name="entityGraphName">Only entity graphs with given name are considered</param>
        public static void DeleteEntityGraph<T, GraphType>(this EntitySet<T> entitySet, Entity entity, string entityGraphName)
            where T : Entity
            where GraphType : EntityGraphAttribute
        {
            var entityContainer = entitySet.EntityContainer;
            entity.GraphOperation<Entity, GraphType>(entityGraphName, e => DeleteAction(e, entityContainer));
        }

        private static Entity DeleteAction(Entity entity, EntityContainer entityContainer)
        {
            var entitySetToDetachFrom = entityContainer.GetEntitySet(entity.GetType());
            entitySetToDetachFrom.Detach(entity);
            return entity;
        }
        #endregion

        #region Implementation methods of Entity Graph Operations

        /// <summary>
        /// Returns the entity graph as defined by associations that are marked with the 'EntityGraphAttribute' attribute.
        /// The resulting graph consists of a list of GraphNodes. Each GraphNode has an element 'Node' of type 'T', 
        /// which represents the actual node, a set, SingleEdges, which correspond to EntityRefs, and a
        /// a set, ListEdges, which correspond to EntityCollections. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="graph"></param>
        private static void GetEntityGraph(Entity entity, Func<EntityGraphAttribute, bool> EntityGraphMatch, EntityGraph<Entity> graph)
        {
            if (graph.Nodes.Any(n => n.Node == entity))
                return;
            GraphNode<Entity> node = new GraphNode<Entity>() { Node = entity };
            graph.Nodes.Add(node);

            foreach (PropertyInfo association in GetAssociations(entity).Where(a => HasEntityGraphAttribute(a, EntityGraphMatch)))
            {
                if (typeof(IEnumerable).IsAssignableFrom(association.PropertyType))
                {
                    IEnumerable assocList = (IEnumerable)association.GetValue(entity, null);
                    node.ListEdges.Add(association, new List<Entity>());
                    foreach (Entity e in assocList)
                    {
                        if (e != null)
                        {
                            node.ListEdges[association].Add(e);
                            GetEntityGraph(e, EntityGraphMatch, graph);
                        }
                    }
                }
                else
                {
                    Entity e = (Entity)association.GetValue(entity, null);
                    if (e != null)
                    {
                        node.SingleEdges.Add(association, e);
                        GetEntityGraph(e, EntityGraphMatch, graph);
                    }
                }
            }
        }

        /// <summary>
        /// (Re-)builds the assocacitions between the nodes of the graph.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes"></param>
        /// <param name="graph"></param>
        private static void BuildEntityGraph(Dictionary<Entity, Entity> nodes, EntityGraph<Entity> graph)
        {
            foreach (var n in graph.Nodes)
            {
                var newEntity = nodes[n.Node];
                foreach (var association in n.SingleEdges.Keys)
                {
                    var oldAssociationEntity = n.SingleEdges[association];
                    var newAssociationEntity = nodes[oldAssociationEntity];
                    association.SetValue(newEntity, newAssociationEntity, null);
                }
                foreach (var association in n.ListEdges.Keys)
                {
                    IEnumerable assocList = (IEnumerable)association.GetValue(newEntity, null);
                    Type assocListType = assocList.GetType();
                    var addMethod = assocListType.GetMethod("Add");

                    foreach (var oldAssociationEntity in n.ListEdges[association])
                    {
                        var newAssociationEntity = nodes[oldAssociationEntity];
                        addMethod.Invoke(assocList, new object[] { newAssociationEntity });
                    }
                }
            }
        }
        /// <summary>
        /// Returns true of the property has the "EntityGraphAttribute" (or a subclass), false otherwise.
        /// </summary>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        private static bool HasEntityGraphAttribute(PropertyInfo propInfo, Func<EntityGraphAttribute, bool> EntityGraphMatch)
        {
            return propInfo.GetCustomAttributes(true).OfType<EntityGraphAttribute>().Any(EntityGraphMatch);
        }

        /// <summary>
        /// Returns an array of PropertyInfo  objects for properties which have the "DataMemberAttribute"
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetDataMembers(Entity entity)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qry = from p in entity.GetType().GetProperties(bindingAttr)
                      where
                        p.GetCustomAttributes(typeof(DataMemberAttribute), true).Length > 0
                      && p.GetCustomAttributes(typeof(KeyAttribute), true).Length == 0
                      && p.GetSetMethod() != null
                      select p;
            return qry.ToArray();
        }
        /// <summary>
        /// Returns an array of PropertyInfo  objects for properties which have the "AssociationAttribute"
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetAssociations(Entity entity)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qry = from p in entity.GetType().GetProperties(bindingAttr)
                      where p.GetCustomAttributes(typeof(AssociationAttribute), true).Length > 0 
                      select p;
            return qry.ToArray();
        }

        private class GraphNode<T>
        {
            public GraphNode()
            {
                SingleEdges = new Dictionary<PropertyInfo, T>();
                ListEdges = new Dictionary<PropertyInfo, List<T>>();
            }
            public T Node { get; set; }
            public Dictionary<PropertyInfo, T> SingleEdges { get; set; }
            public Dictionary<PropertyInfo, List<T>> ListEdges { get; set; }
        }

        private class EntityGraph<T>
        {
            public EntityGraph()
            {
                Nodes = new List<GraphNode<T>>();
            }
            public List<GraphNode<T>> Nodes { get; private set; }
        }
        #endregion
    }
}