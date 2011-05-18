using System.ComponentModel;
using System.Linq;
using System.ServiceModel.DomainServices.Client;

namespace RiaServicesContrib.DomainServices.Client
{
    public partial class EntityGraph 
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
            if(sender != this)
            {
                if(((Entity)sender).HasChanges == true)
                {
                    HasChanges = true;
                }
                else
                if(HasChanges == true)
                {
                    HasChanges = EntityRelationGraph.Aggregate(false, (result, entity) => result |= entity.HasChanges);
                }
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
