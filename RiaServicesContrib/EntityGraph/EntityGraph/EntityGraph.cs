using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.ServiceModel.DomainServices.Client;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity> : IEnumerable<Entity>, IDisposable where TEntity : Entity
    {
        public TEntity Source { get; private set; }
        public string Name { get; private set; }

        public EntityGraph(TEntity Source) : this(Source, null) { }

        public EntityGraph(TEntity Source, string Name)
        {
            this.Source = Source;
            this.Name = Name;

            SetupNotifyPropertyChangedHandlers();
            SetupNotifyCollectionChangedHandlers();
            SetupHasChanges();
        }

        private EntityRelationGraph<Entity> _entityRelationGraph;
        private EntityRelationGraph<Entity> EntityRelationGraph
        {
            get
            {
                if (_entityRelationGraph == null)
                {
                    _entityRelationGraph = new EntityRelationGraph<Entity>();

                    GetEntityGraph(Source, _entityRelationGraph);
                }
                return _entityRelationGraph;
            }
            set
            {
                _entityRelationGraph = value;
            }
        }

        /// <summary>
        /// Method that implementes a generic traversal over an entity graph (defined by 
        /// associations marked with an entity graph attibute and applies 'action' to each visited node.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TEntity GraphOperation(Func<Entity, Entity> action)
        {
            var nodeMap = new Dictionary<Entity, Entity>();

            nodeMap = EntityRelationGraph.Nodes.Aggregate(nodeMap, (nm, graphNode) =>
            {
                nm.Add(graphNode.Node, action(graphNode.Node));
                return nm;
            }
            );
            BuildEntityGraph(nodeMap, EntityRelationGraph);
            return nodeMap[Source] as TEntity;
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return EntityRelationGraph.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        /// Returns the entity graph as defined by associations that are marked with the 'EntityGraphAttribute' attribute.
        /// The resulting graph consists of a list of GraphNodes. Each GraphNode has an element 'Node' of type 'T', 
        /// which represents the actual node, a set, SingleEdges, which correspond to EntityRefs, and a
        /// a set, ListEdges, which correspond to EntityCollections. 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="graph"></param>
        private void GetEntityGraph(Entity entity, EntityRelationGraph<Entity> graph)
        {
            if (graph.Nodes.Any(n => n.Node == entity))
                return;
            EntityRelation<Entity> node = new EntityRelation<Entity>() { Node = entity };
            graph.Nodes.Add(node);

            foreach (PropertyInfo association in GetAssociations(entity).Where(a => HasEntityGraphAttribute(a)))
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
                            GetEntityGraph(e, graph);
                        }
                    }
                }
                else
                {
                    Entity e = (Entity)association.GetValue(entity, null);
                    if (e != null)
                    {
                        node.SingleEdges.Add(association, e);
                        GetEntityGraph(e, graph);
                    }
                }
            }
        }

        /// <summary>
        /// (Re-)builds the assocacitions between the nodes of the graph.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="graph"></param>
        private static void BuildEntityGraph(Dictionary<Entity, Entity> nodes, EntityRelationGraph<Entity> graph)
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
        private bool HasEntityGraphAttribute(PropertyInfo propInfo)
        {
            Func<EntityGraphAttribute, bool> match = 
                entityGraph => entityGraph is EntityGraphAttribute && (Name == null || Name == entityGraph.Name);

            return propInfo.GetCustomAttributes(true).OfType<EntityGraphAttribute>().Any(match);
        }

        /// <summary>
        /// Returns an array of PropertyInfo  objects for properties which have the "AssociationAttribute"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetAssociations(Entity obj)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qry = from p in obj.GetType().GetProperties(bindingAttr)
                      where p.GetCustomAttributes(typeof(AssociationAttribute), true).Length > 0
                      select p;
            return qry.ToArray();
        }

        private void EntityRelationGraphReset()
        {
            RemoveNotifyPropertyChangedHandlers();
            RemoveNotifyCollectionChangedHandlers();
            EntityRelationGraph = null;
            SetupNotifyPropertyChangedHandlers();
            SetupNotifyCollectionChangedHandlers();
        }
        public void Dispose()
        {
            RemoveNotifyPropertyChangedHandlers();
            RemoveNotifyCollectionChangedHandlers();
        }
    }
}