using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if SILVERLIGHT
using System.Reflection;
#endif

namespace RiaServicesContrib
{
    public partial class EntityGraph<TEntity>
    {
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
    }
}