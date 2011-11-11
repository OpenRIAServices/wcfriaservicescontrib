using System.ComponentModel;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using System;

namespace RiaServicesContrib.DomainServices.Client
{
    public partial class EntityGraph 
    {
        [Initialize]
        internal void SetupHasChanges()
        {
            this.PropertyChanged += EntityGraphHasChanges_PropertyChanged;
            this.EntityRelationGraphResetted += EntityGraphHasChanges_EntityRelationGraphResetted;
            DetectHasChanges();
        }
        [Dispose]
        internal void CleanHasChanges()
        {
            this.PropertyChanged -= EntityGraphHasChanges_PropertyChanged;
            this.EntityRelationGraphResetted -= EntityGraphHasChanges_EntityRelationGraphResetted;
        }

        void EntityGraphHasChanges_EntityRelationGraphResetted(object sender, EventArgs e)
        {
            DetectHasChanges();
        }
        private void DetectHasChanges()
        {
            Func<Entity, bool> hasChanges = entity =>
                entity.HasChanges |
                entity.EntityState == EntityState.New |
                entity.EntityState == EntityState.Deleted;
            HasChanges = EntityRelationGraph.Aggregate(false, (result, entity) => result |= hasChanges(entity));
        }
        void EntityGraphHasChanges_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(sender != this)
            {
                if(((Entity)sender).HasChanges == true)
                {
                    HasChanges = true;
                }
                else
                {
                    DetectHasChanges();
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
