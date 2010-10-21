using System.ComponentModel;
using System.ServiceModel.DomainServices.Client;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity> where TEntity : Entity
    {
        private void SetupHasChanges()
        {
            this.PropertyChanged += EntityGraph_PropertyChanged;
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
                    PropertyChanged(this, new PropertyChangedEventArgs("HasChanges"));
                }
            }
        }
    }
}
