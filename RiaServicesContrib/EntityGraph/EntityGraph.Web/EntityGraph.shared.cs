using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace EntityGraph
{
    internal class InitializeAttribute : Attribute
    {
    }
    internal class DisposeAttribute : Attribute
    {
    }

    public abstract partial class EntityGraph<TEntity, TBase, TValidationResult>
        where TEntity : class, TBase
        where TBase : class
        where TValidationResult : class
    {
        public TEntity Source { get; private set; }
        public string Name { get; private set; }

        private Func<TBase, string, IEnumerable<PropertyInfo>> GetNeighbors;

        public EntityGraph(TEntity Source) : this(Source, default(string)) { }

        public EntityGraph(TEntity Source, string Name)
            : this(Source, Name, (entity, path) => GetEntityGraphAssociations(entity, Name)) 
        {
        }
        
        public EntityGraph(TEntity Source, string[] paths)
            : this(Source, null, (entity, path) => GetNeigborsByStringPaths(entity, path, paths))
        {
        }

        public EntityGraph(TEntity Source, EntityGraphShape shape)
            : this(Source, null, (entity, path) => shape.GetAssociations(entity))
        {
        }
        private EntityGraph(TEntity Source, string Name, Func<TBase, string, IEnumerable<PropertyInfo>> GetNeighbors)
        {
            this.Source = Source;
            this.Name = Name;
            this.GetNeighbors = GetNeighbors;

            var type = this.GetType();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var constructors = type.GetMethods(flags).Where(m => m.IsDefined(typeof(InitializeAttribute), true));

            foreach(var constructor in constructors)
            {
                constructor.Invoke(this, new object[] { });
            }
        }

        private EntityRelationGraph<TBase> _entityRelationGraph;
        protected EntityRelationGraph<TBase> EntityRelationGraph
        {
            get
            {
                if(_entityRelationGraph == null)
                {
                    _entityRelationGraph = new EntityRelationGraph<TBase>();
                    BuildEntityGraph(Source, _entityRelationGraph, Source.GetType().Name, GetNeighbors);
                }
                return _entityRelationGraph;
            }
            set
            {
                _entityRelationGraph = value;
            }
        }

        /// <summary>
        /// Method that implements a generic traversal over an entity graph (defined by 
        /// associations marked with an entity graph attibute and applies 'action' to each visited node.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TEntity GraphOperation(Func<TBase, TBase> action)
        {
            var nodeMap = new Dictionary<TBase, TBase>();

            nodeMap = EntityRelationGraph.Nodes.Aggregate(nodeMap, (nm, graphNode) =>
            {
                nm.Add(graphNode.Node, action(graphNode.Node));
                return nm;
            }
            );
            BuildEntityGraph(nodeMap, EntityRelationGraph);
            return nodeMap[Source] as TEntity;
        }

        /// <summary>
        /// Returns the entity graph as defined by associations that are marked with the 'EntityGraphAttribute' attribute.
        /// The resulting graph consists of a list of GraphNodes. Each GraphNode has an element 'Node' of type 'T', 
        /// which represents the actual node, a set, SingleEdges, which correspond to EntityRefs, and a
        /// a set, ListEdges, which correspond to EntityCollections. 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="graph"></param>
        private void BuildEntityGraph(TBase entity, EntityRelationGraph<TBase> graph, string path, Func<TBase,string, IEnumerable<PropertyInfo>> OutEdges)
        {
            if(graph.Nodes.Any(n => n.Node == entity))
                return;
            EntityRelation<TBase> node = new EntityRelation<TBase>() { Node = entity };
            graph.Nodes.Add(node);

            foreach(PropertyInfo association in OutEdges(entity, path))
            {
                if(typeof(IEnumerable).IsAssignableFrom(association.PropertyType))
                {
                    IEnumerable assocList = (IEnumerable)association.GetValue(entity, null);
                    node.ListEdges.Add(association, new List<TBase>());
                    foreach(TBase e in assocList)
                    {
                        if(e != null)
                        {
                            node.ListEdges[association].Add(e);
                            BuildEntityGraph(e, graph, path + "." + association.Name, OutEdges);
                        }
                    }
                }
                else
                {
                    TBase e = (TBase)association.GetValue(entity, null);
                    if(e != null)
                    {
                        node.SingleEdges.Add(association, e);
                        BuildEntityGraph(e, graph, path + "." + association.Name, OutEdges);
                    }
                }
            }
        }

        /// <summary>
        /// (Re-)builds the associations between the nodes of the graph.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="graph"></param>
        private static void BuildEntityGraph(Dictionary<TBase, TBase> nodes, EntityRelationGraph<TBase> graph)
        {
            foreach(var n in graph.Nodes)
            {
                var newEntity = nodes[n.Node];
                foreach(var association in n.SingleEdges.Keys)
                {
                    var oldAssociationEntity = n.SingleEdges[association];
                    var newAssociationEntity = nodes[oldAssociationEntity];
                    association.SetValue(newEntity, newAssociationEntity, null);
                }
                foreach(var association in n.ListEdges.Keys)
                {
                    IEnumerable assocList = (IEnumerable)association.GetValue(newEntity, null);
                    Type assocListType = assocList.GetType();
                    var addMethod = assocListType.GetMethod("Add");

                    foreach(var oldAssociationEntity in n.ListEdges[association])
                    {
                        var newAssociationEntity = nodes[oldAssociationEntity];
                        addMethod.Invoke(assocList, new object[] { newAssociationEntity });
                    }
                }
#if SILVERLIGHT
                // The code below is to fix an error in RIA where relationship span is not performed
                // for newly created entities. 
                // This means that for an association that is not included in the entity graph,
                // the intity would include the foreing key to that entity, but since no relationship span
                // takes place, the corresponding association is not bound to that entity.
                // Below we set these association properties ourselves. We detect newly created entities by
                // the heuristic that they don't have an origional state.
                foreach(PropertyInfo association in GetAssociations(newEntity))
                {
                    if(association.PropertyType.IsSubclassOf(typeof(TBase)))
                    {
                        TBase e = (TBase)association.GetValue(n.Node, null);
                        if(e != null && e is  System.ServiceModel.DomainServices.Client.Entity)
                        {
                            if((e as System.ServiceModel.DomainServices.Client.Entity).GetOriginal() == null)
                            {
                                association.SetValue(newEntity, nodes.ContainsKey(e) ? nodes[e] : e, null);
                            }
                        }
                    }
                }
#endif
            }
        }
#if SILVERLIGHT
        /// <summary>
        /// This is a helper class for reconstructing associations in Silverlight (see explanation above).
        /// Returns an IEnumerable of PropertyInfo objects for properties which have the "AssociationAttribute"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetAssociations(TBase obj)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qry = from p in obj.GetType().GetProperties(bindingAttr)
                      where p.IsDefined(typeof(AssociationAttribute), true)
                      select p;
            return qry;
        }
#endif
        private static IEnumerable<PropertyInfo> GetNeigborsByStringEdges(TBase entity, string[] edges)
        {
            string name = entity.GetType().Name;

            foreach(var edge in edges)
            {
                var edgeComponents = edge.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);

                if(edgeComponents.Count() != 2)
                {
                    Console.Error.WriteLine("Invalid edge expression: {0}", edge);
                    continue;
                }
                edgeComponents[0] = edgeComponents[0].Trim();
                edgeComponents[1] = edgeComponents[1].Trim();
                if(edgeComponents[0] == name)                
                {
                    var propName = edgeComponents[1];
                    var propInfo = entity.GetType().GetProperty(propName);
                    if(propInfo == null)
                    {
                        Console.Error.WriteLine("Invalid property name '{0}' in path for entity '{1}'", propName, name); ;
                        continue;
                    }
                    yield return propInfo;
                }
            }
        }

        /// <summary>
        /// This method returns all neighbor nodes for path.Entity with given collection of paths
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="visitedPath"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetNeigborsByStringPaths(TBase entity, string visitedPath, string[] paths)
        {
            var entityType = entity.GetType();
            foreach(var path in paths.Where(p => p.StartsWith(visitedPath) && p != visitedPath))
            {
                if(path[visitedPath.Length] != '.')
                    continue;
                var visitedPathComponents = visitedPath.Split('.');
                var pathComponents = path.Substring(visitedPath.Length + 1).Split('.');
                int pathComponentsCount = visitedPathComponents.Count();

                var propName = pathComponents[0];
                var propInfo = entityType.GetProperty(propName);
                if(propInfo == null)
                {
                    Console.Error.WriteLine("Invalid property name '{0}' in path for entity '{1}'", propName, entityType.Name); ;
                    continue;
                }
                yield return propInfo;
            }
        }

        /// <summary>
        /// Returns true if the property has the "EntityGraphAttribute" (or a subclass), false otherwise.
        /// </summary>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        private static bool HasEntityGraphAttribute(PropertyInfo propInfo, string Name)
        {
            Func<EntityGraphAttribute, bool> match =
                entityGraph => entityGraph is EntityGraphAttribute && (Name == null || Name == entityGraph.Name);

            return propInfo.GetCustomAttributes(true).OfType<EntityGraphAttribute>().Any(match);
        }
        /// <summary>
        /// Returns an IEnumerable of PropertyInfo objects for properties which have the "EntityGraph". If entityGraphname
        /// is not null, the name of the entity graph should match entityGraphname.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="entityGraphName"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetEntityGraphAssociations(TBase obj, string entityGraphName)
        {
            Func<EntityGraphAttribute, bool> match =
                entityGraph => entityGraph is EntityGraphAttribute && (entityGraphName == null || entityGraphName == entityGraph.Name);
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qry = from p in obj.GetType().GetProperties(bindingAttr)
                      where p.GetCustomAttributes(true).OfType<EntityGraphAttribute>().Any(match)
                      select p;
            return qry;
        }

        /// <summary>
        /// Eventhandler that is called just before the EntityRelationGraph is reset.
        /// </summary>
        protected EventHandler<EventArgs> EntityRelationGraphResetting;
        /// <summary>
        /// Eventhandler that is called right after the EntityRelationGraph is reset.
        /// </summary>
        protected EventHandler<EventArgs> EntityRelationGraphResetted;

        protected void EntityRelationGraphReset()
        {
            if(EntityRelationGraphResetting != null)
            {
                EntityRelationGraphResetting(this, new EventArgs());
            }
            EntityRelationGraph = null;
            if(EntityRelationGraphResetted != null)
            {
                EntityRelationGraphResetted(this, new EventArgs());
            }
        }
    }
}