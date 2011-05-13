using System.Collections.Specialized;
using System.Reflection;
using RiaServicesContrib.DeveloperUtilities;
using System.Linq;

namespace RiaServicesContrib.DeveloperUtilities
{
    public class CollectionOwnerInfo<TEntity>
        where TEntity : class
    {
        public TEntity Owner { get; set; }
        public PropertyInfo Edge { get; set; }
    }
}
namespace RiaServicesContrib
{
    public partial class EntityGraph<TEntity> : INotifyCollectionChanged 
    {
        [Initialize]
        internal void SetupINotifyCollectionChanged() {
            this.EntityRelationGraphResetting += (sender, args) => RemoveNotifyCollectionChangedHandlers();
            this.EntityRelationGraphResetted += (sender, args) =>
            {
                SetupNotifyCollectionChangedHandlers();
                if(CollectionChanged != null)
                {
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            };
            SetupNotifyCollectionChangedHandlers();
        }
        [Dispose]
        internal void DisploseINotifyCollectionChanged()
        {
            RemoveNotifyCollectionChangedHandlers();
        }
        
        private void SetupNotifyCollectionChangedHandlers()
        {
            foreach(var node in EntityRelationGraph.Nodes)
            {
                foreach (var list in node.ListEdges)
                {
                    if (typeof(INotifyCollectionChanged).IsAssignableFrom(list.Key.PropertyType))
                    {
                        INotifyCollectionChanged collection = (INotifyCollectionChanged)list.Key.GetValue(node.Node, null);
                        collection.CollectionChanged += collection_CollectionChanged;
                    }                    
                }
            }
        }
        
        private void RemoveNotifyCollectionChangedHandlers()
        {
            foreach (var node in EntityRelationGraph.Nodes)
            {
                foreach (var list in node.ListEdges)
                {
                    if (typeof(INotifyCollectionChanged).IsAssignableFrom(list.Key.PropertyType))
                    {
                        INotifyCollectionChanged collection = (INotifyCollectionChanged)list.Key.GetValue(node.Node, null);
                        collection.CollectionChanged -= collection_CollectionChanged;
                    }
                }
            }
        }

        private void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            EntityRelationGraphReset();
            if(CollectionChanged != null)
            {
                CollectionChanged(sender, e);
            }
        }

        /// <summary>
        /// Handler to receive collection changed events
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Method that returns an CollectionOwnerInfo object that containes the owning
        /// entity of the given collection, and the property name for that collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public CollectionOwnerInfo<TEntity> GetCollectionOwnerInfo(INotifyCollectionChanged collection)
        {
            var senderType = collection.GetType();
            var CollectionOwnerInfo =
                from node in this.EntityRelationGraph.Nodes
                from edge in node.ListEdges
                where
                    edge.Key.PropertyType == senderType &&
                    edge.Key.GetValue(node.Node, null) == collection
                select
                    new CollectionOwnerInfo<TEntity> { Owner = node.Node, Edge = edge.Key };
            return CollectionOwnerInfo.Single();
        }
    }
}
