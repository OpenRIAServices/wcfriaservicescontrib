using System.ComponentModel;
using System.Reflection;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity> : INotifyPropertyChanged
    {
        /// <summary>
        /// handler to receive property changed events.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        [Initialize]
        internal void SetupINotifyPropertyChanged()
        {
            this.EntityRelationGraphResetting += (sender, args) => RemoveNotifyCollectionChangedHandlers();
            this.EntityRelationGraphResetted += (sender, args) => SetupNotifyPropertyChangedHandlers();
            SetupNotifyPropertyChangedHandlers();
        }
        [Dispose]
        internal void DisposeINotifyPropertyChanged()
        {
            RemoveNotifyPropertyChangedHandlers();
        }
        private void SetupNotifyPropertyChangedHandlers()
        {
            foreach (var node in EntityRelationGraph)
            {
                node.PropertyChanged += node_PropertyChanged;
            }
        }
        private void RemoveNotifyPropertyChangedHandlers()
        {
            foreach (var node in EntityRelationGraph)
            {
                node.PropertyChanged -= node_PropertyChanged;
            }
        }
        void node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyInfo propInfo;
            propInfo = sender.GetType().GetProperty(e.PropertyName);
            if (HasEntityGraphAttribute(propInfo))
            {
                EntityRelationGraphReset();
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, e);
            }
        }
    }
}
