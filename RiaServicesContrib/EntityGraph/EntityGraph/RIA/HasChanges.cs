using System.ComponentModel;

namespace EntityGraph.RIA
{
    public partial class EntityGraph<TEntity> 
    {
        [Initialize]
        internal void SetupHasChanges()
        {
            this.PropertyChanged += EntityGraph_PropertyChanged;
        }
        [Dispose]
        internal void CleanHasChanges()
        {
            this.PropertyChanged -= EntityGraph_PropertyChanged;
        }

        void EntityGraph_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != this)
            {
                bool hasChanges = false;
                foreach (var node in EntityRelationGraph)
                {
                    hasChanges |= node.HasChanges;
                }
                HasChanges = hasChanges;
            }
        }

        private bool _HasChanges;
        public bool HasChanges
        {
            get
            {
                return this._HasChanges;
            }
            set
            {
                if ((this._HasChanges != value))
                {
                    this._HasChanges = value;
                    NotifyPropertyChanged(new PropertyChangedEventArgs("HasChanges"));
                }
            }
        }
    }
}
