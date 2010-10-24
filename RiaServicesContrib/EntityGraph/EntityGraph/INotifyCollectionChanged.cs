using System.Collections.Specialized;
using System.ComponentModel;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity> : INotifyCollectionChanged 
    {
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
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    if(EditStarted)
                        ((IEditableObject)e.OldItems[0]).EndEdit();
                    break;
            }
            if (CollectionChanged != null)
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
