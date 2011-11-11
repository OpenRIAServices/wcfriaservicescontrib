using System.Collections.Specialized;
using System.Reflection;
using System.Linq;

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
        internal void DisposeINotifyCollectionChanged()
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
    }
}
