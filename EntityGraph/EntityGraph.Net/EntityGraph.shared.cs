using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RiaServicesContrib
{
    internal class InitializeAttribute : Attribute
    {
    }
    internal class DisposeAttribute : Attribute
    {
    }

    public partial class EntityGraph<TEntity>
        where TEntity : class
    {
        public TEntity Source { get; private set; }
        private IEntityGraphShape GraphShape { get; set; }

        public EntityGraph(TEntity source, IEntityGraphShape graphShape)
        {
            if(source == null)
            {
                throw new ArgumentNullException("source", "The cosntructor argument 'source' can't be null");
            }
            if(graphShape == null)
            {
                throw new ArgumentNullException("graphShape", "The constructor argument 'graphShape' can't be null");
            }
            this.Source = source;
            this.GraphShape = graphShape;

            var type = this.GetType();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var constructors = type.GetMethods(flags).Where(m => m.IsDefined(typeof(InitializeAttribute), true));

            foreach(var constructor in constructors)
            {
                constructor.Invoke(this, new object[] { });
            }
        }

        private EntityRelationGraph<TEntity> _entityRelationGraph;
        protected EntityRelationGraph<TEntity> EntityRelationGraph
        {
            get
            {
                if(_entityRelationGraph == null)
                {
                    _entityRelationGraph = new EntityRelationGraph<TEntity>();
                    BuildEntityGraph(Source, _entityRelationGraph);
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
        public TEntity GraphMap(Func<TEntity, TEntity> action)
        {
            var nodeMap = new Dictionary<TEntity, TEntity>();

            nodeMap = EntityRelationGraph.Nodes.Aggregate(nodeMap, (nm, graphNode) =>
            {
                nm.Add(graphNode.Node, action(graphNode.Node));
                return nm;
            }
            );
            BuildEntityGraph(nodeMap, EntityRelationGraph);
            return nodeMap[Source];
        }

        /// <summary>
        /// Returns the entity graph as defined by associations that are marked with the 'EntityGraphAttribute' attribute.
        /// The resulting graph consists of a list of GraphNodes. Each GraphNode has an element 'Node' of type 'T', 
        /// which represents the actual node, a set, SingleEdges, which correspond to EntityRefs, and a
        /// a set, ListEdges, which correspond to EntityCollections. 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="graph"></param>
        private void BuildEntityGraph(TEntity entity, EntityRelationGraph<TEntity> graph)
        {
            if(graph.Nodes.Any(n => n.Node == entity))
                return;
            EntityRelation<TEntity> node = new EntityRelation<TEntity>() { Node = entity };
            graph.Nodes.Add(node);

            foreach(PropertyInfo edge in GraphShape.OutEdges(entity))
            {
                if(typeof(IEnumerable).IsAssignableFrom(edge.PropertyType))
                {
                    var assocList = GraphShape.GetNodes(entity, edge);
                    node.ListEdges.Add(edge, new List<TEntity>());
                    foreach(TEntity e in assocList)
                    {
                        if(e != null)
                        {
                            node.ListEdges[edge].Add(e);
                            BuildEntityGraph(e, graph);
                        }
                    }
                }
                else
                {
                    TEntity e = (TEntity)GraphShape.GetNode(entity, edge);
                    if(e != null)
                    {
                        node.SingleEdges.Add(edge, e);
                        BuildEntityGraph(e, graph);
                    }
                }
            }
        }

        /// <summary>
        /// (Re-)builds the associations between the nodes of the graph.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="graph"></param>
        private static void BuildEntityGraph(Dictionary<TEntity, TEntity> nodes, EntityRelationGraph<TEntity> graph)
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
                // the 'newEntity' entity would include the foreing key to that entity, but since no relationship span
                // takes place, the corresponding association is not bound to that entity.
                // Below we set these association properties ourselves. We detect newly created entities by
                // the heuristic that they don't have an origional state. We only set the association if the
                // corresponding entity is detached from the context, otherwise the 'newEntity' entity would be
                // added to the context as a side effect.
                foreach(PropertyInfo association in GetAssociations(newEntity))
                {
                    if(association.PropertyType.IsSubclassOf(typeof(TEntity)))
                    {
                        TEntity e = (TEntity)association.GetValue(n.Node, null);
                        if(e != null && e is System.ServiceModel.DomainServices.Client.Entity)
                        {
                            if ((e as System.ServiceModel.DomainServices.Client.Entity).GetOriginal() == null)
                            {
                                if ((e as System.ServiceModel.DomainServices.Client.Entity).EntityState == System.ServiceModel.DomainServices.Client.EntityState.Detached)
                                {
                                    association.SetValue(newEntity, nodes.ContainsKey(e) ? nodes[e] : e, null);
                                }
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
        private static IEnumerable<PropertyInfo> GetAssociations(TEntity obj)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qry = from p in obj.GetType().GetProperties(bindingAttr)
                      where p.IsDefined(typeof(System.ComponentModel.DataAnnotations.AssociationAttribute), true)
                      select p;
            return qry;
        }
#endif
        private static IEnumerable<PropertyInfo> GetNeigborsByStringEdges(TEntity entity, string[] edges)
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